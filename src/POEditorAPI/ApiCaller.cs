namespace POEditorAPI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    class ApiCaller
    {
        private readonly string _apiToken;

        private readonly HttpClient _httpClient;

        public JsonSerializerSettings JsonSerializerSettings { get; }

        public ApiCaller(string apiToken, HttpClient httpClient, JsonSerializerSettings jsonSerializerSettings)
        {
            this._apiToken = apiToken;
            this._httpClient = httpClient;
            this.JsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task<T> Upload<T>(string relativeUrl, IDictionary<string, string> parameters, Stream stream)
        {
            parameters.Add("api_token", this._apiToken);

            var parts = new MultipartFormDataContent();
            foreach (var parameter in parameters)
            {
                parts.Add(new StringContent(parameter.Value), parameter.Key);
            }

            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml");
            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                                           {
                                                               Name = "file",
                                                               FileName = "foo.resx"
                                                           };
            parts.Add(streamContent, "file");

            var response = await this._httpClient.PostAsync(relativeUrl, parts);
            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ResponseWrapper<T>>(json);

            if (obj.Response.Code != "200")
            {
                throw new Exception(obj.Response.Message);
            }

            return obj.Result;
        }

        public async Task<T> Request<T>(string relativeUrl, IDictionary<string, string> parameters)
        {
            parameters.Add("api_token", this._apiToken);
            var content = new FormUrlEncodedContent(parameters);

            var response = await this._httpClient.PostAsync(relativeUrl, content);
            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ResponseWrapper<T>>(json, this.JsonSerializerSettings);

            if (obj.Response.Code != "200")
            {
                throw new Exception(obj.Response.Message);
            }

            return obj.Result;
        }
    }
}