namespace POEditorAPI
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public class LanguageAPI
    {
        private readonly ApiCaller _apiCaller;

        internal LanguageAPI(ApiCaller apiCaller)
        {
            this._apiCaller = apiCaller;
        }

        /// <summary>
        /// Returns a comprehensive list of all languages supported by POEditor.
        /// </summary>
        /// <returns>Comprehensive list of all languages supported by POEditor.</returns>
        public async Task<ICollection<Language>> Available()
        {
            var response = await this._apiCaller
                               .Request<LanguageResponse>("/v2/languages/available", new Dictionary<string, string>())
                               .ConfigureAwait(false);
            return response.Languages;
        }

        /// <summary>
        /// Returns project languages, percentage of translation done for each and the datetime (UTC - ISO 8601) when the last change was made.
        /// </summary>
        /// <param name="id">The id of project</param>
        /// <returns></returns>
        public async Task<ICollection<LanguageDetails>> List(int id)
        {
            var response = await this._apiCaller
                               .Request<LanguageDetailsResponse>("/v2/languages/list", new Dictionary<string, string> { { "id", id.ToString(CultureInfo.InvariantCulture) } })
                               .ConfigureAwait(false);
            return response.Languages;
        }

        /// <summary>
        /// Adds a new language to project.
        /// </summary>
        /// <param name="id">The id of project</param>
        /// <param name="language">The language code</param>
        /// <returns></returns>
        public Task Add(int id, string language)
        {
            return this._apiCaller
                .Request<object>("/v2/languages/add", new Dictionary<string, string> { { "id", id.ToString(CultureInfo.InvariantCulture) } });
        }

        /// <summary>
        /// Inserts/overwrites translations
        /// </summary>
        /// <param name="id">The id of project</param>
        /// <param name="language">The language code</param>
        /// <param name="fuzzyTrigger">Set to true to mark corresponding translations from the other languages as fuzzy for the updated values.</param>
        /// <param name="terms">Terms to update</param>
        /// <returns></returns>
        public async Task<TranslationSummary> Update(int id, string language, bool fuzzyTrigger, ICollection<Term> terms)
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     { "id", id.ToString(CultureInfo.InvariantCulture) },
                                     { "language", language },
                                     { "data", JsonConvert.SerializeObject(terms, this._apiCaller.JsonSerializerSettings) }
                                 };

            if (fuzzyTrigger)
            {
                parameters.Add("fuzzy_trigger", "1");
            }

            var response = await this._apiCaller
                .Request<LanguageUpdateResponse>("/v2/languages/update", parameters)
                .ConfigureAwait(false);

            return response.Translations;
        }
    }
}