﻿namespace SimpleHttpServer.Models
{
    using System;
    using Enums;

    public class Route
    {
        public string Name { get; set; }
        public string UrlRegex { get; set; }
        public RequestMethod RequestMethod { get; set; }

        public Func<HttpRequest, HttpResponse> Callable { get; set; }
    }
}
