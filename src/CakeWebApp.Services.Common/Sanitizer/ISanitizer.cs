using System;
using System.Collections.Generic;
using System.Text;

namespace CakeWebApp.Services.Common.Sanitizer
{
    public interface ISanitizer
    {
        string Sanitize(string content);
    }
}
