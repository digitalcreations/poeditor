namespace POEditorAPI
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public class TermsAPI
    {
        private readonly ApiCaller _apiCaller;

        internal TermsAPI(ApiCaller apiCaller)
        {
            this._apiCaller = apiCaller;
        }

        /// <summary>
        /// Returns project's terms and translations if the argument language is provided.
        /// </summary>
        /// <param name="id">The id of project</param>
        /// <param name="language">The language code (optional)</param>
        /// <returns>List of terms and translations</returns>
        public async Task<ICollection<Term>> List(int id, string language = null)
        {
            var parameters =
                new Dictionary<string, string> { { "id", id.ToString(CultureInfo.InvariantCulture) } };
            if (!string.IsNullOrWhiteSpace(language))
            {
                parameters.Add("language", language);
            }

            var response = await this._apiCaller.Request<TermsResponse>("/v2/terms/list", parameters).ConfigureAwait(false);
            return response.Terms;
        }

        /// <summary>
        /// Adds terms to project.
        /// </summary>
        /// <param name="id">The id of project</param>
        /// <param name="terms">Terms to add</param>
        /// <returns>Summary of terms added and parsed.</returns>
        public async Task<TermsSummary> Add(int id, ICollection<Term> terms)
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     { "id", id.ToString(CultureInfo.InvariantCulture) },
                                     { "data", JsonConvert.SerializeObject(terms, this._apiCaller.JsonSerializerSettings) }
                                 };

            var response = await this._apiCaller.Request<SyncResponse>("/v2/terms/add", parameters).ConfigureAwait(false);
            return response.Terms;
        }

        /// <summary>
        /// Updates project terms. Lets you change the text, context, reference, plural and tags.
        /// </summary>
        /// <param name="id">The id of project</param>
        /// <param name="terms">Terms to add</param>
        /// <param name="triggerFuzzy">Set to true to mark corresponding translations from all languages as fuzzy for the updated values.</param>
        /// <returns>Summary of terms added and parsed.</returns>
        public async Task<TermsSummary> Update(int id, ICollection<Term> terms, bool triggerFuzzy)
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     { "id", id.ToString(CultureInfo.InvariantCulture) },
                                     { "data", JsonConvert.SerializeObject(terms, this._apiCaller.JsonSerializerSettings) },
                                     { "fuzzy_trigger", triggerFuzzy ? "1" : "0" }
                                 };

            var response = await this._apiCaller.Request<SyncResponse>("/v2/terms/update", parameters).ConfigureAwait(false);
            return response.Terms;
        }
    }
}