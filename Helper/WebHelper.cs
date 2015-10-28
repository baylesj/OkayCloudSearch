using System;
using System.IO;
using System.Net;
using System.Text;

namespace AmazingCloudSearch.Helper
{
    class WebHelper
    {

        private static string JSON_ERROR = "error";

        public class JsonResult
        {
            public bool IsError { get; set; }
            public string Exception { get; set; }
            public string Json { get; set; }
        }

        public JsonResult PostRequest(string url, string json)
        {
            try
            {
                string rawJsonResult = PostRequestWithException(url, json);

                if (string.IsNullOrEmpty(rawJsonResult) || rawJsonResult.Equals(JSON_ERROR))
                    return new JsonResult {Json = rawJsonResult, Exception = "Unknown Error", IsError = true};

                return new JsonResult { Json = rawJsonResult };
            }
            catch(Exception ex)
            {
                return new JsonResult { Exception = ex.Message, IsError = true };
            }
        }

        public JsonResult GetRequest(string url)
        {
            try
            {
                string rawJsonResult = GetRequestWithException(url);

                if (string.IsNullOrEmpty(rawJsonResult) || rawJsonResult.Equals(JSON_ERROR))
                    return new JsonResult { Json = rawJsonResult, Exception = "Unknown Error", IsError = true };

                return new JsonResult { Json = rawJsonResult };
            }
            catch (Exception ex)
            {
                return new JsonResult { Exception = ex.Message, IsError = true };
            }
        }

        private string PostRequestWithException(string url, string json)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "POST";

            byte[] postBytes = Encoding.ASCII.GetBytes(json);

            request.ContentType = "application/json";
            request.ContentLength = postBytes.Length;

            var requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            return RunResponse(request);
        }



        private string GetRequestWithException(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "GET";
            request.Accept = "application/json";

            return RunResponse(request);
        }

        private string RunResponse(HttpWebRequest request)
        {
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException wex)
            {
                if (wex.Response == null)
                    return JSON_ERROR;

                using (var errorResponse = (HttpWebResponse)wex.Response)
                {
                    var errorStream = errorResponse.GetResponseStream();
                    if (errorStream != null)
                    {
                        using (var reader = new StreamReader(errorStream))
                        {
                            return reader.ReadToEnd();
                        }                        
                    }
                    return "";
                }
            }

            var stream = response.GetResponseStream();
            if (stream != null)
                return new StreamReader(stream).ReadToEnd();

            return "";
        }
    }
}