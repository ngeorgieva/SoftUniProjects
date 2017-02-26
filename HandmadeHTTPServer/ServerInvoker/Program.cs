namespace ServerInvoker
{
    using System.Collections.Generic;
    using SimpleHttpServer;
    using SimpleHttpServer.Enums;
    using SimpleHttpServer.Models;

    public class Program
    {
        public static void Main()
        {
            var routes = new List<Route>()
            {
                new Route
                {
                    Name = "Hello Handler",
                    UrlRegex = @"^/hello$",
                    RequestMethod = RequestMethod.GET,
                    Callable = (HttpRequest request) =>
                    {
                        return new HttpResponse()
                        {
                            ContentAsUTF8 = "<h3>Hello from HttpServer :)</h3>",
                            StatusCode = ResponseStatusCode.OK
                        };
                    }
                }
            };

            HttpServer httpServer = new HttpServer(8081, routes);
            httpServer.Listen();
        }
    }
}
