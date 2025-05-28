using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Encodings.Web;

namespace RenderMentor.Utility
{
    public class PaymentGateway
    {
        private readonly HttpClient client;
        private static readonly IConfiguration _configuration;

        static PaymentGateway()
        {
            // Encoding provider kaydı yapıyoruz (ISO-8859-9 için gerekli)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public PaymentGateway(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }
        
        private static string GetSHA1(string text)
        {
            var provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            using var sha1 = SHA1.Create();
            var inputbytes = sha1.ComputeHash(Encoding.GetEncoding("ISO-8859-9").GetBytes(text));

            var builder = new StringBuilder();
            for (int i = 0; i < inputbytes.Length; i++)
            {
                builder.Append(string.Format("{0,2:x}", inputbytes[i]).Replace(" ", "0"));
            }

            return builder.ToString().ToUpper();
        }
        
        private static string GetSHA512(string text)
        {
            var provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            using var sha512 = SHA512.Create();
            var inputbytes = sha512.ComputeHash(Encoding.GetEncoding("ISO-8859-9").GetBytes(text));

            var builder = new StringBuilder();
            for (int i = 0; i < inputbytes.Length; i++)
            {
                builder.Append(string.Format("{0,2:x}", inputbytes[i]).Replace(" ", "0"));
            }

            return builder.ToString().ToUpper();
        }
        
        // Garanti bankasının örnek kodundaki GetHashData ile aynı mantıkta çalışan fonksiyon
        private static string CalculateGarantiHash(string provisionPassword, string terminalId, string orderId, 
            string amount, int currencyCode, string successUrl, string errorUrl, string type, string installmentCount, string storeKey, string rnd)
        {
            // TerminalId'nin 9 basamaklı olduğundan emin ol
            string terminalIdPadded = terminalId.PadLeft(9, '0');
            
            // hashedPassword = SHA1(Password + "0" + TerminalID)
            string hashedPassword = GetSHA1(provisionPassword + "0" + terminalId);
            
            // HashData hesaplama - Garanti Bankası'nın formundaki sıralama
            // SHA512(TerminalID + OrderID + Amount + CurrencyCode + SuccessURL + ErrorURL + Type + InstallmentCount + StoreKey + HashedPassword)
            StringBuilder hashBuilder = new StringBuilder();
            hashBuilder.Append(terminalId);
            hashBuilder.Append(orderId);
            hashBuilder.Append(amount);
            hashBuilder.Append(currencyCode);
            hashBuilder.Append(successUrl);
            hashBuilder.Append(errorUrl);
            hashBuilder.Append(type);
            hashBuilder.Append(installmentCount); // Boş string olarak gönderilecek
            hashBuilder.Append(storeKey);
            hashBuilder.Append(hashedPassword);
            
            string hashStr = hashBuilder.ToString();
            
            return GetSHA512(hashStr);
        }

        private static Dictionary<string, string> BankParameters
        {
            get
            {
                // Terminal bilgileri için yapılandırma kontrolü
                if (string.IsNullOrEmpty(_configuration["PaymentGateway:TerminalId"]) ||
                    string.IsNullOrEmpty(_configuration["PaymentGateway:TerminalMerchantId"]) ||
                    string.IsNullOrEmpty(_configuration["PaymentGateway:TerminalProvPassword"]))
                {
                    throw new ArgumentNullException("PaymentGateway:Terminal*", "Terminal bilgileri yapılandırma dosyasında eksik veya hatalı. Lütfen kontrol ediniz.");
                }
                
                return new Dictionary<string, string>
                {
                    { "TerminalUserId", _configuration["PaymentGateway:TerminalUserId"] ?? throw new ArgumentNullException("PaymentGateway:TerminalUserId") },
                    { "TerminalId", _configuration["PaymentGateway:TerminalId"] ?? throw new ArgumentNullException("PaymentGateway:TerminalId") },
                    { "TerminalMerchantId", _configuration["PaymentGateway:TerminalMerchantId"] ?? throw new ArgumentNullException("PaymentGateway:TerminalMerchantId") },
                    { "TerminalProvUserId", _configuration["PaymentGateway:TerminalProvUserId"] ?? throw new ArgumentNullException("PaymentGateway:TerminalProvUserId") },
                    { "TerminalProvPassword", _configuration["PaymentGateway:TerminalProvPassword"] ?? throw new ArgumentNullException("PaymentGateway:TerminalProvPassword") },
                    { "StoreKey", _configuration["PaymentGateway:StoreKey"] ?? throw new ArgumentNullException("PaymentGateway:StoreKey") },
                    { "Mode", _configuration["PaymentGateway:Mode"] ?? throw new ArgumentNullException("PaymentGateway:Mode") },
                    { "GatewayUrl", _configuration["PaymentGateway:GatewayUrl"] ?? throw new ArgumentNullException("PaymentGateway:GatewayUrl") },
                    { "VerifyUrl", _configuration["PaymentGateway:VerifyUrl"] ?? throw new ArgumentNullException("PaymentGateway:VerifyUrl") }
                };
            }
        }

        public static ThreeDGatewayResult ThreeDGatewayRequest(PaymentGatewayRequest request)
        {
            try
            {
                // Terminal ve temel parametre kontrolü
                if (string.IsNullOrEmpty(BankParameters["TerminalId"]) || 
                    string.IsNullOrEmpty(BankParameters["TerminalMerchantId"]) ||
                    string.IsNullOrEmpty(BankParameters["TerminalProvPassword"]))
                {
                    return new ThreeDGatewayResult
                    {
                        Success = false,
                        ErrorMessage = "Terminal bilgileri tanımlanmamış veya eksik. Lütfen appsettings.json dosyasındaki PaymentGateway ayarlarını kontrol ediniz."
                    };
                }

                // CallbackUrl kontrolü
                if (request.CallbackUrl == null)
                {
                    return new ThreeDGatewayResult
                    {
                        Success = false,
                        ErrorMessage = "Geri dönüş URL'i tanımlanmamış. CallbackUrl alanını kontrol ediniz."
                    };
                }
                
                // Normal URL'lerini kullan, test için yönlendirmeyi kaldırıyoruz
                string successUrl = request.CallbackUrl.ToString();
                string errorUrl = request.CallbackUrl.ToString();
                
                // IP adresi kontrolü - boş ise hata döndür
                if (string.IsNullOrEmpty(request.CustomerIpAddress))
                {
                    return new ThreeDGatewayResult
                    {
                        Success = false,
                        ErrorMessage = "Müşteri IP adresi bulunamadı. Güvenlik nedeniyle işlem devam edemiyor."
                    };
                }

                // E-posta adresi kontrolü
                if (string.IsNullOrEmpty(request.CustomerEmailAddress))
                {
                    return new ThreeDGatewayResult
                    {
                        Success = false,
                        ErrorMessage = "Müşteri e-posta adresi bulunamadı. Lütfen e-posta adresinizi kontrol ediniz."
                    };
                }

                var parameters = new Dictionary<string, string>();
                
                // Hash hesaplama - Garanti Bankası'nın dokümantasyonu ile uyumlu
                string terminalId = BankParameters["TerminalId"];
                string storeKey = BankParameters["StoreKey"];
                string amount = (request.TotalAmount * 100).ToString("0", CultureInfo.InvariantCulture); // Kuruş cinsinden, virgülsüz
                int currencyCode = 949; // Türk Lirası
                string type = "sales";
                string rnd = DateTime.Now.ToString("yyyyMMddHHmmss"); // İşlem zamanını rnd değeri olarak kullan

                // Garanti Bankası'nın beklediği parametreleri ekleyelim - 3D doğrulaması için
                parameters.Add("apiversion", "512"); // Garanti Bankası tarafından önerilen değer
                parameters.Add("mode", BankParameters["Mode"]);
                parameters.Add("terminalprovuserid", "PROVAUT"); // Garanti Bankası'nın formundan
                parameters.Add("terminaluserid", "SANALUSER"); // Garanti Bankası'nın formundan
                parameters.Add("terminalmerchantid", "1359751"); // Garanti Bankası'nın bildirdiği değer
                parameters.Add("terminalid", BankParameters["TerminalId"]);
                parameters.Add("orderid", request.OrderNumber);
                parameters.Add("customeripaddress", request.CustomerIpAddress);
                parameters.Add("customeremailaddress", string.IsNullOrEmpty(request.CustomerEmailAddress) ? "info@rendermentor.com" : request.CustomerEmailAddress);
                parameters.Add("txntimestamp", rnd);
                parameters.Add("refreshtime", "5"); // Garanti Bankası'nın formundan
                parameters.Add("lang", "tr");
                parameters.Add("txnamount", amount); // Garanti formuna göre txnamount kullanıyoruz
                parameters.Add("txncurrencycode", "949"); // Garanti formuna göre txncurrencycode kullanıyoruz
                parameters.Add("successurl", successUrl);
                parameters.Add("errorurl", errorUrl);
                parameters.Add("txntype", "sales");
                // Taksit parametresini boş gönderiyoruz - peşin işlem için
                parameters.Add("txninstallmentcount", "");
                parameters.Add("rnd", rnd);
                parameters.Add("secure3dsecuritylevel", "3D_PAY");
                parameters.Add("storekey", BankParameters["StoreKey"]);
                parameters.Add("companyname", "RenderMentor");
                parameters.Add("mddirect", "1"); // 3D Secure'un işlem sonrası doğrudan Garanti'ye yönlendirilmesi
                parameters.Add("addauthenticationdata", "1"); // İşlem doğrulama verilerini ekle

                // Kart bilgileri
                if (!string.IsNullOrEmpty(request.CardHolderName))
                {
                    parameters.Add("cardholderName", request.CardHolderName);
                    parameters.Add("orderaddressline1", request.CardHolderName);
                }

                // Kredi kartı numarası ve güvenlik kodu (CVV)
                if (!string.IsNullOrEmpty(request.CardNumber))
                {
                    parameters.Add("cardnumber", request.CardNumber);
                }
                
                if (!string.IsNullOrEmpty(request.ExpireMonth) && !string.IsNullOrEmpty(request.ExpireYear))
                {
                    parameters.Add("cardexpiredatemonth", request.ExpireMonth);
                    parameters.Add("cardexpiredateyear", request.ExpireYear);
                }
                
                if (!string.IsNullOrEmpty(request.CvvCode))
                {
                    parameters.Add("cardcvv2", request.CvvCode);
                }

                // Garanti Bankası örnek kodlarındaki mantık ile hash hesaplama
                string hashData = CalculateGarantiHash(
                    BankParameters["TerminalProvPassword"], 
                    terminalId, 
                    request.OrderNumber, 
                    amount, 
                    currencyCode, 
                    successUrl, 
                    errorUrl,
                    type,
                    "", // Boş taksit parametresi
                    storeKey,
                    rnd
                );
                
                // Garanti BBVA, 3D işlemlerinde secure3dhash parametresini bekliyor
                parameters.Add("secure3dhash", hashData);
                parameters.Add("storetype", "3D_PAY");

                return new ThreeDGatewayResult
                {
                    Success = true,
                    GatewayUrl = new Uri(BankParameters["GatewayUrl"]),
                    Parameters = parameters
                };
            }
            catch (Exception ex)
            {
                return new ThreeDGatewayResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public static VerifyGatewayResult VerifyGateway(IFormCollection form)
        {
            try
            {
                if (form == null || !form.Keys.Any())
                {
                    return VerifyGatewayResult.Failed("Form verisi alınamadı.");
                }

                // Debug amaçlı tüm form değerlerini kontrol et
                var formData = new StringBuilder();
                foreach (var key in form.Keys)
                {
                    formData.AppendLine($"{key}: {form[key]}");
                }
                
                // 1. ÖNEMLI KONTROL: mesajın bankadan geldiğini hash ile doğrula
                bool isValidHash = false;
                if (form.ContainsKey("hash") && form.ContainsKey("hashparams"))
                {
                    string responseHash = form["hash"].ToString();
                    string responseHashparams = form["hashparams"].ToString();
                    
                    if (string.IsNullOrEmpty(responseHashparams))
                    {
                        return VerifyGatewayResult.Failed("Ödeme işlemi sırasında bir doğrulama hatası oluştu. Lütfen daha sonra tekrar deneyiniz.");
                    }
                    
                    string digestData = "";
                    char[] separator = new char[] { ':' };
                    string[] paramList = responseHashparams.Split(separator);
                    
                    foreach (string param in paramList)
                    {
                        if (!string.IsNullOrEmpty(param))
                        {
                            digestData += form.ContainsKey(param) ? form[param].ToString() : "";
                        }
                    }
                    
                    // Sonuna StoreKey eklenir
                    digestData += BankParameters["StoreKey"];
                    
                    // Hash hesaplama
                    string hashCalculated = GetSHA512(digestData);
                    
                    // Hesaplanan hash ile dönen hash karşılaştırması
                    isValidHash = responseHash.Equals(hashCalculated, StringComparison.OrdinalIgnoreCase);
                    
                    if (!isValidHash)
                    {
                        // Kullanıcı dostu hata mesajı
                        if (form.ContainsKey("errmsg") && !string.IsNullOrEmpty(form["errmsg"]))
                        {
                            return VerifyGatewayResult.Failed(form["errmsg"].ToString());
                        }
                        else if (form.ContainsKey("mderrormessage") && !string.IsNullOrEmpty(form["mderrormessage"]))
                        {
                            return VerifyGatewayResult.Failed(form["mderrormessage"].ToString());
                        }
                        else
                        {
                            return VerifyGatewayResult.Failed("Ödeme işlemi bankadan onay alamadı. Lütfen kart bilgilerinizi kontrol ediniz veya farklı bir ödeme yöntemi deneyiniz.");
                        }
                    }
                }
                
                // 2. ÖNEMLI KONTROL: mdStatus kontrolü
                if (!form.ContainsKey("mdstatus"))
                {
                    return VerifyGatewayResult.Failed("3D doğrulama durumu (mdstatus) bulunamadı.");
                }
                
                var mdStatus = form["mdstatus"].ToString();
                if (string.IsNullOrEmpty(mdStatus))
                {
                    return VerifyGatewayResult.Failed("3D doğrulama durumu (mdstatus) alınamadı.");
                }

                // 3. ÖNEMLI KONTROL: procreturncode kontrolü - sadece "00" başarılı kabul edilmeli
                if (!form.ContainsKey("procreturncode"))
                {
                    return VerifyGatewayResult.Failed("İşlem sonuç kodu (procreturncode) bulunamadı.");
                }
                
                var procreturncode = form["procreturncode"].ToString();
                if (procreturncode != "00")
                {
                    string errorMessage = $"İşlem başarısız. İşlem sonuç kodu: {procreturncode}";
                    
                    if (form.ContainsKey("errmsg") && !string.IsNullOrEmpty(form["errmsg"]))
                    {
                        errorMessage += $" Hata: {form["errmsg"]}";
                    }
                    
                    return VerifyGatewayResult.Failed(errorMessage, procreturncode);
                }

                // 4. ÖNEMLI KONTROL: Response kontrolü
                if (!form.ContainsKey("response"))
                {
                    return VerifyGatewayResult.Failed("Banka yanıtı (response) bulunamadı.");
                }
                
                var response = form["response"].ToString();
                if (response != "Approved")
                {
                    return VerifyGatewayResult.Failed($"İşlem onaylanmadı. Yanıt: {response}");
                }
                
                // MdStatus değerleri:
                // 1: Tam Doğrulama
                // 2: Kart Sahibi veya bankası sisteme kayıtlı değil
                // 3: Kartın bankası sisteme kayıtlı değil
                // 4: Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş
                // 5: Doğrulama yapılamıyor
                // 6: 3-D Secure hatası
                // 7: Sistem hatası
                // 8: Bilinmeyen kart no
                if (!mdStatusCodes.Contains(mdStatus))
                {
                    string errorMsg = "Ödeme işlemi sırasında bir hata oluştu";
                    
                    if (form.ContainsKey("mderrormessage") && !string.IsNullOrEmpty(form["mderrormessage"]))
                    {
                        errorMsg = form["mderrormessage"].ToString();
                    }
                    else if (form.ContainsKey("errmsg") && !string.IsNullOrEmpty(form["errmsg"]))
                    {
                        errorMsg = form["errmsg"].ToString();
                    }
                    
                    return VerifyGatewayResult.Failed(errorMsg, mdStatus, response);
                }
                
                // İşlem referans numaraları alınıyor
                string transid = form.ContainsKey("transid") ? form["transid"].ToString() : "";
                string hostrefnum = form.ContainsKey("hostrefnum") ? form["hostrefnum"].ToString() : "";
                string authcode = form.ContainsKey("authcode") ? form["authcode"].ToString() : "";
                
                // Tüm kontroller başarılı, işlem onaylandı
                return VerifyGatewayResult.Successed(
                    transid, 
                    hostrefnum, 
                    response, 
                    procreturncode,
                    authcode
                );
            }
            catch (Exception ex)
            {
                return VerifyGatewayResult.Failed($"Ödeme doğrulama sırasında hata oluştu: {ex.Message}");
            }
        }

        private static readonly string[] mdStatusCodes = new[] { "1", "2", "3", "4" };
    }

    public class PaymentGatewayRequest
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
        public string CvvCode { get; set; }

        public double TotalAmount { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerIpAddress { get; set; }
        public string CustomerEmailAddress { get; set; }
        public string TimeStamp { get; set; }
        public Uri CallbackUrl { get; set; }

        public string BankErrorMessage { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int StatusId { get; set; }
        public bool Deleted { get; set; }
        public string BankRequest { get; set; }
        public string BankResponse { get; set; }
        public string UserAgent { get; set; }
    }

    public class PaymentGatewayResult
    {
        public Uri GatewayUrl { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public string HtmlFormContent { get; set; }
        public bool HtmlContent => !string.IsNullOrEmpty(HtmlFormContent);
        public static PaymentGatewayResult Successed(string htmlFormContent,
            string message = null)
        {
            return new PaymentGatewayResult
            {
                Success = true,
                HtmlFormContent = htmlFormContent,
                Message = message
            };
        }

        public static PaymentGatewayResult Successed(IDictionary<string, object> parameters,
            string gatewayUrl,
            string message = null)
        {
            return new PaymentGatewayResult
            {
                Success = true,
                Parameters = parameters,
                GatewayUrl = new Uri(gatewayUrl),
                Message = message
            };
        }

        public static PaymentGatewayResult Failed(string errorMessage, string errorCode = null)
        {
            return new PaymentGatewayResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }
    }

    public class VerifyGatewayResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public string Response { get; set; }
        public string TransactionId { get; set; }
        public string HostReferenceNumber { get; set; }
        public string AuthCode { get; set; }
        public string ReturnCode { get; set; }
        public string OrderNumber { get; set; }
        public double Amount { get; set; }

        public static VerifyGatewayResult Successed(string transactionId, string hostRefNum, string response, string returnCode, string authCode = "")
        {
            return new VerifyGatewayResult
            {
                Success = true,
                TransactionId = transactionId,
                HostReferenceNumber = hostRefNum,
                Response = response,
                ReturnCode = returnCode,
                AuthCode = authCode,
                Message = $"İşlem başarıyla tamamlandı. TransactionId: {transactionId}, RefNum: {hostRefNum}, Code: {returnCode}"
            };
        }

        public static VerifyGatewayResult Failed(string errorMessage, string errorCode = null, string response = null)
        {
            return new VerifyGatewayResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                Response = response
            };
        }
    }

    public class ThreeDGatewayResult
    {
        public bool Success { get; set; }
        public Uri GatewayUrl { get; set; }
        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        public string ErrorMessage { get; set; }
    }
}
