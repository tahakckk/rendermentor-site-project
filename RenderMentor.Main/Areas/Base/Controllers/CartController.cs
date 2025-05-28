using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Base.Controllers
{
    [Area("Base")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailOptions emailOptions;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager, IOptions<EmailOptions> options)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            emailOptions = options.Value;
        }

        [Route("sepet")]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    OrderHeader = new Models.OrderHeader(),
                    ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Course,Subscription")
                };

                ShoppingCartVM.OrderHeader.OrderTotal = 0;
                ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

                foreach (var list in ShoppingCartVM.ListCart)
                {
                    if (list.PackageId == 2)
                    {
                        ShoppingCartVM.OrderHeader.OrderTotal += list.Course.Price;
                    }
                    else
                    {
                        ShoppingCartVM.OrderHeader.OrderTotal += list.Subscription.Price;
                    }
                };
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId, includeProperties: "Course");

            var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);

            return RedirectToAction(nameof(Index));
        }

        [Route("odeme")]
        public IActionResult Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Course,Subscription")
            };

            if (claim != null && ShoppingCartVM.ListCart.Count() > 0)
            {
                ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);

                foreach (var list in ShoppingCartVM.ListCart)
                {
                    if (list.PackageId == 2)
                    {
                        list.Course.CourseCategory = new Models.CourseCategory()
                        {
                            Name = _unitOfWork.CourseCategory.GetFirstOrDefault(c => c.Id == list.Course.CourseCategoryId).Name
                        };
                        ShoppingCartVM.OrderHeader.OrderTotal += list.Course.Price;
                    }
                    else
                    {
                        ShoppingCartVM.OrderHeader.OrderTotal += list.Subscription.Price;
                    }
                };
                ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
                ShoppingCartVM.OrderHeader.Email = ShoppingCartVM.OrderHeader.ApplicationUser.Email;
                ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

                return View(ShoppingCartVM);
            }

            return RedirectToAction("Index", "CourseList");

        }

        [HttpPost]
        [Route("odeme")]
        [ActionName("Checkout")]
        [ValidateAntiForgeryToken]
        public IActionResult CheckoutPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value, includeProperties: "Course,Subscription");
            foreach (var list in ShoppingCartVM.ListCart)
            {
                if (list.PackageId == 2)
                {
                    ShoppingCartVM.OrderHeader.OrderTotal += list.Course.Price;
                }
                else
                {
                    ShoppingCartVM.OrderHeader.OrderTotal += list.Subscription.Price;
                }
            };

            if (claim != null && ShoppingCartVM.ListCart.Count() > 0)
            {
                // Payment Gateway
                PaymentGatewayRequest gatewayRequest = new PaymentGatewayRequest
                {
                    CardHolderName = ShoppingCartVM.CardHolderName,
                    CardNumber = ShoppingCartVM.CardNumber?.Replace(" ", string.Empty).Replace(" ", string.Empty),
                    ExpireMonth = ShoppingCartVM.ExpireMonth,
                    ExpireYear = ShoppingCartVM.ExpireYear,
                    CvvCode = ShoppingCartVM.CvvCode,
                    TotalAmount = ShoppingCartVM.OrderHeader.OrderTotal, // TotalAmount artık normal tutar, PaymentGateway.cs içinde çevriliyor
                    OrderNumber = Guid.NewGuid().ToString().Replace("-", ""),
                    CustomerIpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    CustomerEmailAddress = ShoppingCartVM.OrderHeader.Email,
                    TimeStamp = DateTime.Now.ToString()
                };

                PaymentTransaction payment = new PaymentTransaction
                {
                    OrderNumber = gatewayRequest.OrderNumber,
                    CustomerIpAddress = gatewayRequest.CustomerIpAddress,
                    UserAgent = HttpContext.Request.Headers[HeaderNames.UserAgent],
                    CardPrefix = gatewayRequest.CardNumber?.Substring(0, 6),
                    CardHolderName = gatewayRequest.CardHolderName,
                    TotalAmount = ShoppingCartVM.OrderHeader.OrderTotal,
                    BankRequest = JsonConvert.SerializeObject(gatewayRequest),
                    ApplicationUserId = claim.Value,
                    Name = ShoppingCartVM.OrderHeader.Name,
                    Email = ShoppingCartVM.OrderHeader.Email,
                    PhoneNumber = ShoppingCartVM.OrderHeader.PhoneNumber
                };
                payment.MarkAsCreated();
                _unitOfWork.PaymentTransaction.Add(payment);
                _unitOfWork.Save();

                var responseModel = new
                {
                    GatewayUrl = new Uri($"{Request.GetHostUrl(false)}/siparis-onay/{gatewayRequest.OrderNumber}"),
                    Message = "Ödeme kapısına yönlendiriliyor..."
                };
                // Payment Gateway END                

                return Ok(responseModel);
            }

            return RedirectToAction("Index", "CourseList");
        }

        [HttpPost]
        public IActionResult Checkout(PaymentGatewayRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.OrderNumber))
                {
                    return Json(new { success = false, message = "Geçersiz ödeme isteği." });
                }

                var gatewayResult = PaymentGateway.ThreeDGatewayRequest(request);
                if (!gatewayResult.Success)
                {
                    return Json(new { success = false, message = "Ödeme işlemi başlatılamadı." });
                }

                return Json(new { success = true, gatewayUrl = gatewayResult.GatewayUrl });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ödeme işlemi sırasında bir hata oluştu." });
            }
        }

        [HttpPost]
        public IActionResult Confirm(IFormCollection form)
        {
            try
            {
                if (form == null || !form.Keys.Any())
                {
                    return Json(new { success = false, message = "Geçersiz form verisi." });
                }

                var verifyResult = PaymentGateway.VerifyGateway(form);
                if (!verifyResult.Success)
                {
                    return Json(new { success = false, message = "Ödeme doğrulanamadı." });
                }

                return Json(new { success = true, message = "Ödeme başarıyla tamamlandı." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Ödeme doğrulama sırasında bir hata oluştu." });
            }
        }

        [Route("siparis-onay")]
        [Route("siparis-onay/{orderNumber}")]
        public IActionResult Confirm(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return View("Fail", VerifyGatewayResult.Failed("Geçersiz sipariş numarası."));
            }

            var payment = _unitOfWork.PaymentTransaction.GetFirstOrDefault(i => i.OrderNumber == orderNumber);
            if (payment == null)
            {
                return View("Fail", VerifyGatewayResult.Failed("Ödeme bilgisi bulunamadı."));
            }

            var bankRequest = JsonConvert.DeserializeObject<PaymentGatewayRequest>(payment.BankRequest);
            if (bankRequest == null)
            {
                return View("Fail", VerifyGatewayResult.Failed("Ödeme bilgisi geçersiz."));
            }

            if (!IPAddress.TryParse(bankRequest.CustomerIpAddress, out IPAddress ipAddress))
            {
                bankRequest.CustomerIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            }

            // Localhost ve geçersiz IP kontrolü
            if (bankRequest.CustomerIpAddress == "::1" || 
                bankRequest.CustomerIpAddress == "localhost" || 
                !IPAddress.TryParse(bankRequest.CustomerIpAddress, out _))
            {
                bankRequest.CustomerIpAddress = "127.0.0.1";
            }

            // IP adresinin geçerli olduğundan emin ol
            if (!IsValidIpAddress(bankRequest.CustomerIpAddress))
            {
                return View("Fail", VerifyGatewayResult.Failed("Geçersiz IP adresi."));
            }

            bankRequest.CallbackUrl = new Uri($"{Request.GetHostUrl(false)}/islem-sonuc/{orderNumber}");

            var gatewayResult = PaymentGateway.ThreeDGatewayRequest(bankRequest);
            if (!gatewayResult.Success)
            {
                return View("Fail", VerifyGatewayResult.Failed("Ödeme işlemi başlatılamadı."));
            }

            // Form içeriğini oluştur
            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<!DOCTYPE html>");
            htmlBuilder.Append("<html lang='tr'>");
            htmlBuilder.Append("<head>");
            htmlBuilder.Append("<meta charset='utf-8'>");
            htmlBuilder.Append("<meta name='viewport' content='width=device-width, initial-scale=1'>");
            htmlBuilder.Append("<title>Ödeme Sayfasına Yönlendiriliyorsunuz</title>");
            htmlBuilder.Append("<style>");
            htmlBuilder.Append("body { display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; font-family: Arial, sans-serif; background-color: #f8f9fa; }");
            htmlBuilder.Append(".loading { text-align: center; padding: 20px; background: white; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); max-width: 90%; }");
            htmlBuilder.Append(".spinner { border: 4px solid #f3f3f3; border-top: 4px solid #3498db; border-radius: 50%; width: 40px; height: 40px; animation: spin 1s linear infinite; margin: 0 auto 20px; }");
            htmlBuilder.Append("@keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }");
            htmlBuilder.Append(".error { color: red; margin-top: 15px; display: none; }");
            htmlBuilder.Append(".btn { display: inline-block; background: #3498db; color: white; padding: 8px 15px; border-radius: 4px; text-decoration: none; margin-top: 15px; }");
            htmlBuilder.Append("</style>");
            
            // Daha basit JavaScript - sözdizimi hatası riskini azaltır
            htmlBuilder.Append("<script>");
            htmlBuilder.Append("function submitPaymentForm() {");
            htmlBuilder.Append("  document.getElementById('loading-text').innerText = 'Garanti Bankası ödeme sayfasına yönlendiriliyorsunuz...';");
            htmlBuilder.Append("  try {");
            htmlBuilder.Append("    document.getElementById('payform').submit();");
            htmlBuilder.Append("  } catch(e) {");
            htmlBuilder.Append("    document.getElementById('error-message').style.display = 'block';");
            htmlBuilder.Append("    document.getElementById('error-message').innerText = 'Hata: ' + e.message;");
            htmlBuilder.Append("    document.getElementById('retry-button').style.display = 'inline-block';");
            htmlBuilder.Append("  }");
            htmlBuilder.Append("}");
            htmlBuilder.Append("function retrySubmit() {");
            htmlBuilder.Append("  document.getElementById('error-message').style.display = 'none';");
            htmlBuilder.Append("  document.getElementById('retry-button').style.display = 'none';");
            htmlBuilder.Append("  setTimeout(submitPaymentForm, 500);");
            htmlBuilder.Append("}");
            htmlBuilder.Append("window.onload = function() {");
            htmlBuilder.Append("  setTimeout(submitPaymentForm, 1000);");
            htmlBuilder.Append("  setTimeout(function() {");
            htmlBuilder.Append("    if (document.getElementById('payform')) {");
            htmlBuilder.Append("      document.getElementById('error-message').style.display = 'block';");
            htmlBuilder.Append("      document.getElementById('error-message').innerText = 'Yönlendirme beklenenden uzun sürüyor. Lütfen tekrar deneyin.';");
            htmlBuilder.Append("      document.getElementById('retry-button').style.display = 'inline-block';");
            htmlBuilder.Append("    }");
            htmlBuilder.Append("  }, 10000);");
            htmlBuilder.Append("};");
            htmlBuilder.Append("</script>");
            
            htmlBuilder.Append("</head>");
            htmlBuilder.Append("<body>");
            htmlBuilder.Append("<div class='loading'>");
            htmlBuilder.Append("<div id='loading-spin' class='spinner'></div>");
            htmlBuilder.Append("<p id='loading-text'>Ödeme sayfası hazırlanıyor...</p>");
            htmlBuilder.Append("<div id='error-message' class='error'></div>");
            htmlBuilder.Append("<button id='retry-button' onclick='retrySubmit()' style='display:none;' class='btn'>Tekrar Dene</button>");
            htmlBuilder.Append("</div>");
            
            // Ödeme formunu oluştur
            htmlBuilder.Append($"<form id='payform' name='payform' method='POST' action='{gatewayResult.GatewayUrl}'>");
            
            // Form parametrelerini ekle
            foreach (var param in gatewayResult.Parameters)
            {
                htmlBuilder.Append($"<input type='hidden' name='{param.Key}' value='{param.Value}'>");
            }
            
            htmlBuilder.Append("</form>");
            htmlBuilder.Append("</body>");
            htmlBuilder.Append("</html>");

            return Content(htmlBuilder.ToString(), "text/html; charset=utf-8");
        }

        [Route("islem-sonuc")]
        [Route("islem-sonuc/{orderNumber}")]
        public IActionResult Callback(string orderNumber, IFormCollection form)
        {
            try
            {
                // Form içeriğini detaylı logla
                var formLogBuilder = new StringBuilder();
                formLogBuilder.AppendLine("Garanti Bankası Form Yanıtı:");
                foreach (var key in form.Keys)
                {
                    formLogBuilder.AppendLine($"{key}: {form[key]}");
                }
                var formLog = formLogBuilder.ToString();
                
                // Sipariş numarası kontrolü
                if (string.IsNullOrEmpty(orderNumber))
                {
                    return View("Fail", new VerifyGatewayResult 
                    { 
                        Success = false, 
                        ErrorMessage = "Sipariş numarası bulunamadı.",
                        Message = formLog
                    });
                }

                // Ödeme işlemini bul
                PaymentTransaction payment = _unitOfWork.PaymentTransaction.GetFirstOrDefault(i => i.OrderNumber == orderNumber);
                if (payment == null)
                {
                    return View("Fail", new VerifyGatewayResult 
                    { 
                        Success = false, 
                        ErrorMessage = $"Ödeme bilgisi bulunamadı. Sipariş No: {orderNumber}",
                        Message = formLog
                    });
                }

                // Banka isteğini yeniden oluştur
                PaymentGatewayRequest bankRequest;
                try
                {
                    bankRequest = JsonConvert.DeserializeObject<PaymentGatewayRequest>(payment.BankRequest);
                    if (bankRequest == null)
                    {
                        return View("Fail", new VerifyGatewayResult 
                        { 
                            Success = false, 
                            ErrorMessage = $"Ödeme isteği ayrıştırılamadı. Sipariş No: {orderNumber}",
                            Message = formLog
                        });
                    }
                }
                catch (Exception ex)
                {
                    return View("Fail", new VerifyGatewayResult 
                    { 
                        Success = false, 
                        ErrorMessage = $"Ödeme isteği ayrıştırma hatası: {ex.Message}",
                        Message = formLog
                    });
                }
                
                // Yanıtı doğrula
                VerifyGatewayResult verifyResult;
                try
                {
                    // Kullanıcıya daha iyi bir hata gösterebilmek için mdStatus'u kontrol et
                    if (form.ContainsKey("mdstatus"))
                    {
                        var mdStatus = form["mdstatus"].ToString();
                        if (mdStatus == "0")
                        {
                            payment.MarkAsFailed("3D doğrulama başarısız", "MdStatus=0");
                            _unitOfWork.PaymentTransaction.Update(payment);
                            _unitOfWork.Save();
                            
                            return View("Fail", new VerifyGatewayResult 
                            { 
                                Success = false, 
                                ErrorMessage = "3D doğrulaması başarısız oldu. Lütfen kart bilgilerinizi kontrol edin ve tekrar deneyin.",
                                Message = formLog
                            });
                        }
                    }
                    
                    // Garanti Bankası yanıtını doğrula
                    verifyResult = PaymentGateway.VerifyGateway(form);
                    verifyResult.OrderNumber = orderNumber;
                    
                    // ÖNEMLİ: Ödeme tutarı kontrolü
                    if (form.ContainsKey("txnamount") || form.ContainsKey("amount"))
                    {
                        string amountStr = form.ContainsKey("txnamount") ? form["txnamount"].ToString() : form["amount"].ToString();
                        if (!string.IsNullOrEmpty(amountStr) && double.TryParse(amountStr, out double bankAmount))
                        {
                            // Banka tutarı kuruş cinsinden geliyor, TL'ye çevir
                            bankAmount = bankAmount / 100;
                            
                            // Tutar kontrolü
                            if (Math.Abs(bankAmount - payment.TotalAmount) > 0.01) // 1 kuruştan fazla fark varsa
                            {
                                payment.MarkAsFailed($"Ödeme tutarı uyuşmazlığı. Beklenen: {payment.TotalAmount}, Gelen: {bankAmount}", "AMOUNT_MISMATCH");
                                _unitOfWork.PaymentTransaction.Update(payment);
                                _unitOfWork.Save();
                                
                                return View("Fail", new VerifyGatewayResult 
                                { 
                                    Success = false, 
                                    ErrorMessage = "Ödeme tutarı doğrulanamadı. Lütfen müşteri hizmetleri ile iletişime geçin.",
                                    Message = formLog
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    payment.MarkAsFailed($"Doğrulama hatası: {ex.Message}", "Exception");
                    _unitOfWork.PaymentTransaction.Update(payment);
                    _unitOfWork.Save();
                    
                    return View("Fail", new VerifyGatewayResult 
                    { 
                        Success = false, 
                        ErrorMessage = $"Ödeme doğrulama sırasında bir hata oluştu: {ex.Message}",
                        Message = formLog
                    });
                }

                // Yanıt bilgilerini kaydet
                payment.BankResponse = JsonConvert.SerializeObject(new
                {
                    OrderNumber = orderNumber,
                    FormData = formLog,
                    VerifyResult = verifyResult
                });

                if (!verifyResult.Success)
                {
                    payment.MarkAsFailed(verifyResult.ErrorMessage, verifyResult.ErrorCode);
                    _unitOfWork.PaymentTransaction.Update(payment);
                    _unitOfWork.Save();
                    
                    return View("Fail", verifyResult);
                }

                // İşlem başarılı, ödeme kayıtlarını güncelle
                payment.MarkAsPaid(verifyResult.TransactionId, verifyResult.HostReferenceNumber);
                payment.ReferenceNumber = verifyResult.HostReferenceNumber;
                payment.TransactionNumber = verifyResult.TransactionId;
                _unitOfWork.PaymentTransaction.Update(payment);
                _unitOfWork.Save();

                // Başarı sayfasına yönlendir
                return View("Success", verifyResult);
            }
            catch (Exception ex)
            {
                return View("Fail", new VerifyGatewayResult 
                { 
                    Success = false, 
                    ErrorMessage = $"İşlem sırasında beklenmedik bir hata oluştu: {ex.Message}"
                });
            }
        }

        [Route("odeme-tamamlandi")]
        [Route("odeme-tamamlandi/{orderNumber}")]
        public IActionResult Completed(string orderNumber)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            PaymentTransaction payment = _unitOfWork.PaymentTransaction.GetFirstOrDefault(i => i.OrderNumber == orderNumber);
            if (payment == null || payment.Status == PaymentStatus.Shipped || claim == null || claim.Value != payment.ApplicationUserId)
            {
                return RedirectToAction("Index", "CourseList");
            }

            ShoppingCartVM model = new ShoppingCartVM
            {

                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == payment.ApplicationUserId, includeProperties: "Course,Subscription"),
                OrderHeader = new Models.OrderHeader()
                {
                    ApplicationUserId = payment.ApplicationUserId,
                    OrderNumber = orderNumber,
                    Name = payment.Name,
                    Email = payment.Email,
                    PhoneNumber = payment.PhoneNumber
                },
                OrderNumber = payment.OrderNumber,
                TransactionNumber = payment.TransactionNumber,
                ReferenceNumber = payment.ReferenceNumber,
                CardHolderName = payment.CardHolderName,
                TotalAmount = payment.TotalAmount,
                PaidDate = payment.PaidDate,
                CreateDate = payment.CreateDate
            };

            foreach (var list in model.ListCart)
            {
                if (list.PackageId == 2)
                {
                    list.Course.CourseCategory = _unitOfWork.CourseCategory.GetFirstOrDefault(c => c.Id == list.Course.CourseCategoryId);
                    model.OrderHeader.OrderTotal += list.Course.Price;
                }
                else
                {
                    model.OrderHeader.OrderTotal += list.Subscription.Price;
                }
            };
            model.OrderHeader.OrderStatus = SD.StatusPending;
            model.OrderHeader.OrderDate = model.CreateDate;

            if (payment.Status == PaymentStatus.Paid)
            {

                model.OrderHeader.OrderStatus = SD.StatusApproved;
                _unitOfWork.OrderHeader.Add(model.OrderHeader);
                _unitOfWork.Save();

                StringBuilder htmlMessage = new StringBuilder();
                htmlMessage.Append("<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html lang='tr' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'><head><style>table{width:600px;margin-top:15px;}table, th, td {border: 1px solid #333;border-collapse: collapse;}</style></head><body><table><tr><th align='center'>Paket/Kurs Adı</th><th align='center'>Kategori</th><th align='center'>Fiyat</th></tr>");


                foreach (var item in model.ListCart)
                {
                    OrderDetails orderDetails = new OrderDetails()
                    {
                        CourseId = item.CourseId,
                        PackageId = item.PackageId,
                        OrderId = model.OrderHeader.Id,
                        Price = item.Course.Price
                    };
                    _unitOfWork.OrderDetails.Add(orderDetails);
                    if (item.PackageId == 2)
                    {
                        htmlMessage.Append($"<tr><td align='center'>{item.Course.Title}</td><td align='center'>{item.Course.CourseCategory.Name}</td><td align='center'>{item.Course.Price} TL</td></tr>");
                    }
                    else
                    {
                        htmlMessage.Append($"<tr><td align='center'>{item.Subscription.Name}</td><td align='center'>Üyelik Pakedi</td><td align='center'>{item.Subscription.Price} TL</td></tr>");
                    }


                    //We empty CourseCategory object attached to Shopping Cart in order not to delete its properties while using RemoveRange below.
                    //item.Course.CourseCategory = new CourseCategory();
                }
                htmlMessage.Append($"<tr><td align='center'><b>Toplam Fiyat</b></td><td align='center'></td><td align='center'>{payment.TotalAmount} TL</td></tr></table><br /></body></html>");
                _unitOfWork.ShoppingCart.RemoveRange(model.ListCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, 0);

                if (payment.TotalAmount == model.OrderHeader.OrderTotal)
                {
                    var userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == payment.ApplicationUserId);
                    userStudent.StudentCourses = _unitOfWork.StudentCourse.GetAll(i => i.StudentId == userStudent.Id).ToList();
                    IEnumerable<OrderDetails> orderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == model.OrderHeader.Id);
                    foreach (var courseToAdd in orderDetails)
                    {
                        if (courseToAdd.PackageId == 2)
                        {
                            if (!userStudent.StudentCourses.Exists(i => i.CourseId == courseToAdd.CourseId))
                            {
                                userStudent.StudentCourses.Add(new StudentCourse()
                                {
                                    StudentId = userStudent.Id,
                                    CourseId = courseToAdd.CourseId,
                                    isPaid = true
                                });
                            }
                        }
                        else
                        {
                            userStudent.SubscribeActive = true;
                            userStudent.SubscribeStarted = true;
                            userStudent.SubscribeStartDate = DateTime.Now;
                            userStudent.SubscribeExpireDate = userStudent.SubscribeStartDate.AddDays(courseToAdd.Subscription.ExpirationDays);
                        }

                    }
                    model.OrderHeader.OrderStatus = SD.StatusShipped;
                    payment.Status = PaymentStatus.Shipped;
                    _unitOfWork.OrderHeader.Update(model.OrderHeader);
                    _unitOfWork.PaymentTransaction.Update(payment);
                    _unitOfWork.Save();

                    StringBuilder customerMessage = new StringBuilder();
                    customerMessage.Append($"Sayın {model.OrderHeader.Name}, toplam {payment.TotalAmount} TL'lık siparişiniz onaylanmıştır. RenderMentor'u tercih ettiğiniz için teşekkür ederiz. <br /> Sipariş Numaranız: {orderNumber}<br/>Aşağıdaki ürünleri satın aldınız;<br/>");
                    customerMessage.Append($"{htmlMessage}");
                    customerMessage.Append("Kurslarınızı Öğrenci Paneli'nizden izlemeye başlayabilirsiniz. İyi dersler dileriz.");
                    _emailSender.SendEmailAsync(model.OrderHeader.Email, "RenderMentor kurs siparişiniz onaylandı.",
                        customerMessage.ToString());

                    StringBuilder adminMessage = new StringBuilder();
                    adminMessage.Append($"{model.OrderHeader.Name} adlı ve {model.OrderHeader.Email} e-posta adresi ile kayıtlı öğrenci toplam {payment.TotalAmount} TL'lık sipariş gerçekleştirdi.<br /> Sipariş Numarası: {orderNumber}<br/>Referans Numarası: {payment.ReferenceNumber}<br/>Karttaki İsim: {payment.CardHolderName}<br/>Kartın İlk 6 Hanesi: {payment.CardPrefix}<br/>Satın alınan kurslar;<br/>");
                    adminMessage.Append($"{htmlMessage}");
                    _emailSender.SendEmailAsync(emailOptions.mailAddress, "Yeni sipariş bilgilendirmesi.",
                        adminMessage.ToString());

                }
            }

            return View(model);
        }

        private bool IsValidIpAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            if (IPAddress.TryParse(ipAddress, out IPAddress address))
            {
                // Private IP adreslerini kontrol et
                byte[] bytes = address.GetAddressBytes();
                if (bytes[0] == 10 || 
                    (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) || 
                    (bytes[0] == 192 && bytes[1] == 168))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}