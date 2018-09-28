namespace POEditor
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using McMaster.Extensions.CommandLineUtils;

    using POEditorAPI;

    [HelpOption("--help")]
    abstract class POEditorCommandBase
    {
        private POEditorAPI _api;

        public POEditorAPI API
        {
            get
            {
                if (this._api == null)
                {
                    var client = new HttpClient();
                    POEditorAPI.ConfigureHttpClient(client);
                    var token = this.ApiToken ?? Environment.GetEnvironmentVariable("POEDITOR_API_TOKEN");
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        throw new ArgumentException("Missing API token. Not found as parameter or environment variable.");
                    }

                    this._api = new POEditorAPI(token, client);
                }

                return this._api;
            }
        }

        [Option(CommandOptionType.SingleValue, Description = "API token. If option is not set, I'll try reading the POEDITOR_API_TOKEN environment variable.", ShortName="t")]
        public string ApiToken { get; set; }

        public virtual Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.FromResult(0);
        }
    }
}