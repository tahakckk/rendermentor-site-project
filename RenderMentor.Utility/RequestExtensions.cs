using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.Utility
{
    public static class RequestExtensions
    {
        public static string GetHostUrl(this HttpRequest request, bool appendSlash = true)
        {
            string hostUrl = request.Headers[HeaderNames.Host];

            if (StringValues.IsNullOrEmpty(hostUrl))
                hostUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            if (!hostUrl.StartsWith("http://") || !hostUrl.StartsWith("https://"))
                hostUrl = $"{request.Scheme}://{hostUrl}";

            if (appendSlash)
                hostUrl = $"{hostUrl.Trim('/')}/";

            return hostUrl;
        }

        public static string CreatePaymentFormHtml(IDictionary<string, object> parameters, Uri actionUrl, bool appendSubmitScript = true)
        {
            if (parameters == null || !parameters.Any())
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (actionUrl == null)
            {
                throw new ArgumentNullException(nameof(actionUrl));
            }

            string formId = "PaymentForm";
            StringBuilder formBuilder = new StringBuilder();
            formBuilder.Append($"<form id=\"{formId}\" name=\"{formId}\" action=\"{actionUrl}\" role=\"form\" method=\"POST\">");

            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                formBuilder.Append($"<input type=\"hidden\" name=\"{parameter.Key}\" value=\"{parameter.Value}\">");
            }

            formBuilder.Append("</form>");

            if (appendSubmitScript)
            {
                StringBuilder scriptBuilder = new StringBuilder();
                scriptBuilder.Append("<script>");
                scriptBuilder.Append($"document.{formId}.submit();");
                scriptBuilder.Append("</script>");
                formBuilder.Append(scriptBuilder.ToString());
            }

            return formBuilder.ToString();
        }
    }
}
