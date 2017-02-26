namespace SimpleHttpServer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Enums;

    public class Header
    {
        public Header(HeaderType type)
        {
            this.Type = type;
            this.ContentType = "text/html";
            this.Cookies = new CookieCollection();
            this.OtherParameters = new Dictionary<string, string>();
        }

        public HeaderType Type { get; set; }

        public string ContentType { get; set; }

        public string ContentLength { get; set; }

        public IDictionary<string, string> OtherParameters { get; set; }

        public CookieCollection Cookies { get; private set; }

        public void AddCookie(Cookie cookie)
        {
            if (!this.Cookies.Contains(cookie.Name))
            {
                this.Cookies.AddCookie(cookie);
            }
        }

        public override string ToString()
        {
            StringBuilder header = new StringBuilder();
            header.AppendLine("Content-type: " + this.ContentType);
            if (this.Cookies.Count > 0)
            {
                if (this.Type == HeaderType.HttpRequest)
                {
                    header.AppendLine("Cookie: " + this.Cookies.ToString());
                }
                else if (this.Type == HeaderType.HttpResponse)
                {
                    foreach (var cookie in this.Cookies)
                    {
                        header.AppendLine("Set-Cookie: " + cookie);
                    }
                }
            }

            if (this.ContentLength != null)
            {
                header.AppendLine("Content-Length: " + this.ContentLength);
            }
            foreach (var other in this.OtherParameters)
            {
                header.AppendLine($"{other.Key}: {other.Value}");
            }
            header.AppendLine();

            return header.ToString();
        }
    }
}
