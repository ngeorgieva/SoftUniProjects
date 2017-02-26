namespace SimpleHttpServer.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class CookieCollection : IEnumerable<Cookie>
    {
        public CookieCollection()
        {
            this.Cookies = new Dictionary<string, Cookie>();
        }

        public IDictionary<string, Cookie> Cookies { get; private set; }

        public Cookie this[string cookieName]
        {
            get
            {
                return this.Cookies[cookieName];
            }
            set
            {
                this.AddCookie(value);
            }
        }

        public int Count => this.Cookies.Count;

        public IEnumerator<Cookie> GetEnumerator()
        {
            return this.Cookies.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Contains(string cookieName)
        {
            return this.Cookies.ContainsKey(cookieName);
        }

        public void AddCookie(Cookie cookie)
        {
            if (cookie != null && !this.Contains(cookie.Name))
            {
                this.Cookies.Add(cookie.Name, cookie);
            }
        }

        public override string ToString()
        {
            return string.Join("; ", this.Cookies.Values);
        }
    }
}
