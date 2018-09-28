namespace POEditorAPI
{
    using System;
    using System.Net.Http;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class POEditorAPI
    {
        private readonly JsonSerializerSettings _serializerSettings;

        private readonly ApiCaller _apiCaller;

        public POEditorAPI(string apiToken, HttpClient httpClient)
        {
            this._serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            };

            this._apiCaller = new ApiCaller(apiToken, httpClient, this._serializerSettings);
            this.Projects = new ProjectAPI(this._apiCaller);
            this.Terms = new TermsAPI(this._apiCaller);
            this.Languages = new LanguageAPI(this._apiCaller);
        }

        public static void ConfigureHttpClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://api.poeditor.com/");
        }

        public ProjectAPI Projects { get; }

        public TermsAPI Terms { get; }

        public LanguageAPI Languages { get; }
    }
}