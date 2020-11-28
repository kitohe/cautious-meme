using System.Text.RegularExpressions;

namespace Identity.API.Services
{
    public class RedirectService : IRedirectService
    {
        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            var decodedUrl = System.Net.WebUtility.HtmlDecode(url);
            var results = Regex.Split(decodedUrl, "redirect_uri=");
            if (results.Length < 2)
                return results[0];

            var result = results[1];

            var splitKey = result.Contains("signin-oidc") ? "signin-oidc" : "scope";

            results = Regex.Split(result, splitKey);
            if (results.Length < 2)
                return "";

            result = results[0];

            return result.Replace("%3A", ":").Replace("%2F", "/")
                .Replace("&", "");
        }
    }
}
