using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonToUITable.Models
{
    public class WebCall
    {
        public enum HttpMethod
        {
            Get = 0,
            Post = 1
        }

        public string Url { get; set; }

        public HttpMethod Method { get; set; }

        public string Headers { get; set; }

        public string PostData { get; set; }
    }
}
