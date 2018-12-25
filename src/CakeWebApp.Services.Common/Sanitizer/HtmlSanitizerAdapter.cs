using Ganss.XSS;

namespace CakeWebApp.Services.Common.Sanitizer
{
    public class HtmlSanitizerAdapter : ISanitizer
    {
        public string Sanitize(string content)
        {
            var sanitizer = new HtmlSanitizer();
            var sanitizedContent = sanitizer.Sanitize(content);
            return sanitizedContent;
        }
    }
}
