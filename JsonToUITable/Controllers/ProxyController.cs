using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JsonToUITable.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace JsonToUITable.Controllers
{
    [Produces("application/json")]
    [Route("api/Proxy")]
    public class ProxyController : Controller
    {
        // GET: api/Proxy
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Proxy/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Proxy
        [HttpPost]
        public void Post([FromBody]WebCall value)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                            delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                                                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
                            {
                                return true; // **** Always accept
                            };

                WebRequest request = WebRequest.Create(value.Url);
                request.Method = value.Method == WebCall.HttpMethod.Get ? "GET" : "POST";
                request.ContentType = "application/json";

                if (!string.IsNullOrEmpty(value.Headers))
                {
                    var lHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(value.Headers);

                    foreach (KeyValuePair<string, string> lHeader in lHeaders)
                    {
                        request.Headers.Add(lHeader.Key.Trim(), lHeader.Value.Trim());
                    }
                }

                if (value.Method == WebCall.HttpMethod.Post)
                {
                    byte[] lDataByteArray = Encoding.UTF8.GetBytes(value.PostData);
                    Stream lRequestStream = request.GetRequestStream();
                    lRequestStream.Write(lDataByteArray, 0, lDataByteArray.Length);
                    request.ContentLength = lDataByteArray.Length;
                    lRequestStream.Close();
                }

                // Get the response.
                WebResponse response = request.GetResponse();

                //Response.ContentLength = 5041;
                Response.ContentType = response.ContentType;
                //Response.Headers.Clear();

                for (int i = 0; i < response.Headers.Count; ++i)
                {
                    string lKey = response.Headers.GetKey(i);
                    foreach (string lHeadVal in response.Headers.GetValues(i))
                    {
                        if (lKey.ToLower().StartsWith("content"))
                        {
                            if (Response.Headers.Keys.Contains(lKey))
                            {
                                Response.Headers[lKey] = lHeadVal;
                            }
                            else
                            {
                                Response.Headers.Add(lKey, lHeadVal);
                            }
                        }
                    }
                }

                response.GetResponseStream().CopyTo(Response.Body);

                //// Get the stream containing content returned by the server.
                //Stream dataStream = response.GetResponseStream();
                //// Open the stream using a StreamReader for easy access.
                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //StreamReader reader = new StreamReader(dataStream,Encoding.GetEncoding("windows-1253"), true);
                //// Read the content.
                //string responseFromServer = reader.ReadToEnd();

                //lExit = JsonConvert.DeserializeObject(responseFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
        }

        //// POST: api/Proxy
        //[HttpPost]
        //public Object Post([FromBody]WebCall value)
        //{
        //    Object lExit = null;

        //    try
        //    {
        //        var lClient = new HttpClient();
        //        HttpMethod lHttpMet = HttpMethod.Get;
        //        if (value.Method == WebCall.HttpMethod.Post)
        //        {
        //            lHttpMet = HttpMethod.Post;
        //        }

        //        lClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header


        //        HttpRequestMessage request = new HttpRequestMessage(lHttpMet, value.Url);

        //        if (value.Method == WebCall.HttpMethod.Post)
        //        {
        //            request.Content = new StringContent(value.PostData,
        //                                            Encoding.UTF8,
        //                                            "application/json");//CONTENT-TYPE header
        //        }


        //        if (!string.IsNullOrEmpty(value.Headers))
        //        {
        //            var lHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(value.Headers);


        //            foreach (KeyValuePair<string, string> lHeader in lHeaders)
        //            {
        //                if (lHeader.Key != null && lHeader.Value != null)
        //                {
        //                    try
        //                    {
        //                        lClient.DefaultRequestHeaders.Add(lHeader.Key.Trim(), lHeader.Value.Trim());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine(ex.Message);
        //                    }
        //                }
        //            }
        //        }

        //        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //        HttpResponseMessage lResponse = lClient.SendAsync(request).GetAwaiter().GetResult();
        //        var lBytes = lResponse.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

        //        var lStr = Encoding.GetEncoding(1253).GetString(lBytes, 0, lBytes.Length);

        //        lExit = JsonConvert.DeserializeObject(lStr);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    return lExit;
        //}

        // PUT: api/Proxy/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
