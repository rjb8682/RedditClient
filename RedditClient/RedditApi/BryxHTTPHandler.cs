using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Windows.Data.Json;
using Windows.Storage;
using RedditClient;


namespace RedditApi
{
    class BryxHttpHandler
    {
        public enum RequestMethod
        {
            Post,
            Put,
            Delete,
            Get
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqParams"></param>
        /// <returns></returns>

        private static Dictionary<string, bool> BoolDictionary(Dictionary<string, string> reqParams)
        {
            return reqParams.ToDictionary(item => item.Key, item => item.Value == "True");
        }


        public static async Task<string> GetDataV2(RequestMethod method, string command, Dictionary<string, string> reqParams)
        {
            string json;

            if (reqParams != null && (reqParams.ContainsValue("True") || reqParams.ContainsValue("False")))
            {
                json = JsonConvert.SerializeObject(BoolDictionary(reqParams), Formatting.None);
            }
            else
            {
                json = JsonConvert.SerializeObject(reqParams, Formatting.None);
            }
            return await GetDataV2WithJson(method, command, json);
        }

        /// <summary>
        ///     Method is invoked when the application needs to communicate to 
        ///     the server.
        /// </summary>
        /// <param name="method">The type of http call being sent to the server.</param>
        /// <param name="requestUrl">The command being sent to the server.</param>
        /// <param name="json">The parameters being sent to the server.</param>
        /// <returns>The response from the server as a string.</returns>

        public static async Task<string> GetDataV2WithJson(RequestMethod method, string requestUrl, string json)
        {
            //var authKeys = new Dictionary<string, string> { { "key", App.ApiKey }, { "id", BryxHelper.GetUniqueId() } };
            //var authHeader = BryxHelper.EncodeTo64(JsonConvert.SerializeObject(authKeys, Formatting.None));

            var httpClient = new HttpClient(new HttpClientHandler());
            HttpResponseMessage response;
            switch (method)
            {
                case RequestMethod.Post:
                    if (!requestUrl.Contains("authorization"))
                    {
                        httpClient.DefaultRequestHeaders.Add("X-Auth-Token", authHeader);
                    }
                    response = await httpClient.PostAsync(requestUrl, new StringContent(json));
                    if (response.IsSuccessStatusCode == false)
                    {
                        throw new HttpRequestException(((int)response.StatusCode).ToString());
                    }
                    break;
                case RequestMethod.Put:
                    httpClient.DefaultRequestHeaders.Add("X-Auth-Token", authHeader);
                    response =
                        await httpClient.PutAsync(requestUrl, new StringContent(json));
                    if (response.IsSuccessStatusCode == false)
                    {
                        throw new HttpRequestException(((int)response.StatusCode).ToString());
                    }
                    break;
                case RequestMethod.Get:
                    httpClient.DefaultRequestHeaders.Add("X-Auth-Token", authHeader);
                    response =
                        await httpClient.GetAsync(requestUrl);
                    if (response.IsSuccessStatusCode == false)
                    {
                        throw new HttpRequestException(((int)response.StatusCode).ToString());
                    }
                    break;
                case RequestMethod.Delete:
                    httpClient.DefaultRequestHeaders.Add("X-Auth-Token", authHeader);
                    var req = new HttpRequestMessage(HttpMethod.Delete, requestUrl);
                    req.Content = new StringContent(json);
                    response = await httpClient.SendAsync(req);
                    if (response.IsSuccessStatusCode == false)
                    {
                        throw new HttpRequestException(((int)response.StatusCode).ToString());
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}

