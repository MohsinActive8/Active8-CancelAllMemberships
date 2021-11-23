using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Active8_CancelAllMemberships.Utility
{
    public class ApiCall
    {
        static HttpClient client;
        private static string BaseURL = ConfigurationManager.AppSettings.Get("APIBaseURL");
        //private static string BaseURL = "https://dev.active8staging.com/altitude-membership-termination-plan/Fusion/Api.svc/"; // live
        //private static string BaseURL = "http://localhost/Aurora/Fusion/Api.svc/"; // local

        public static void CallApi(string uri, object model) 
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (client = new HttpClient(handler)) 
            {

                cookieContainer.Add(new Uri(BaseURL),new Cookie("ASP.NET_SessionId", "elth5pllpkwbf05exztivqwz"));
                cookieContainer.Add(new Uri(BaseURL),new Cookie("jbn82nsolajdz", "JbyI8vKRiu8=||JbyI8vKRiu8="));

                HttpResponseMessage response = client.PostAsJsonAsync(BaseURL + uri, model).Result;

                if (response.StatusCode == HttpStatusCode.OK) 
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    // DO nothing
                }
                else
                if(response.StatusCode == HttpStatusCode.Unauthorized) 
                {
                    throw new Exception("User UnAuthorized");
                }
                else
                if(response.StatusCode == HttpStatusCode.InternalServerError) 
                {
                    throw new Exception("Internal server error");
                }
                else 
                {
                    throw new Exception("An Error Occurred\n Status Code:" + response.StatusCode + "\n URL: "+response.Headers.Location+"\n Content"+response.Content.ReadAsStringAsync().Result);
                }
            }
        }

        public static object Custom(string objectType, string action, Dictionary<string, object> criteria, string apiKey)
        {
            HttpRequestor request = new HttpRequestor
            {
                CustomTimeoutInMs = 60000
            };

            

            string error = request.Request
            (
                $"{BaseURL}/Custom",
                "POST",
                "",
                "",
                JsonConvert.SerializeObject
                (
                    new FusionCustomSignature
                    {
                        objectType = objectType,
                        method = action,
                        paramJson = JsonConvert.SerializeObject(criteria),
                        apiKey = apiKey
                    }
                ),
                new Dictionary<string, string>
                {
                    { "Origin", "InternalApi" } // this is just to satisfy our current origin check and will likely need expanded -jjb
                },
                "application/json"
            );

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            // parse the response into a T object and return it
            string responseJson = request.ResponseFull;
            BaseResponse response = JsonConvert.DeserializeObject<BaseResponse>(JsonConvert.DeserializeObject<string>(responseJson));
            if(response.Alerts.Count > 0) 
            {
                throw new Exception(JsonConvert.SerializeObject(response.Alerts));
            }
            else
            if (response.Returned is JArray)
            {
                return ((JArray)response.Returned).ToObject<object>();
            }
            else
            {
                return response.Returned;
            }
        }
    }
    internal class FusionCustomSignature
    {
        public string objectType { get; set; }
        public string dataJson { get; set; } = "{}";
        public string method { get; set; }
        public string paramJson { get; set; } = "{}";
        public string apiKey { get; set; } = "";
    }
}
