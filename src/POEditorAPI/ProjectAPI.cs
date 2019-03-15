namespace POEditorAPI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public class ProjectAPI
    {
        private readonly ApiCaller _apiCaller;

        internal ProjectAPI(ApiCaller apiCaller)
        {
            this._apiCaller = apiCaller;
        }

        /// <summary>
        /// Returns the list of projects owned by user.
        /// </summary>
        /// <returns>List of projects owned by user.</returns>
        public async Task<ICollection<ProjectSummary>> List()
        {
            var response = await this._apiCaller.Request<ProjectList>("/v2/projects/list", new Dictionary<string, string>()).ConfigureAwait(false);

            return response.Projects;
        }

        /// <summary>
        /// Returns project's details.
        /// </summary>
        /// <param name="id">The id of project</param>
        /// <returns>Project's details</returns>
        public async Task<ProjectDetails> View(int id)
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     { "id", id.ToString(CultureInfo.InvariantCulture) }
                                 };
            var response = await this._apiCaller.Request<ProjectDetailsWrapper>("/v2/projects/view", parameters).ConfigureAwait(false);

            return response.Project;
        }

        /// <summary>
        /// Syncs your project with the array you send (terms that are not found in the JSON object will be deleted from project and the new ones added).
        ///
        /// Please use with caution.If wrong data is sent, existing terms and their translations might be irreversibly lost.
        /// </summary>
        /// <param name="id">The id of project</param>
        /// <param name="terms">Terms to synchronize</param>
        /// <returns>Summary of number of items changed.</returns>
        public async Task<TermsSummary> Sync(int id, ICollection<Term> terms)
        {
            var parameters = new Dictionary<string, string>()
                                 {
                                     { "id", id.ToString(CultureInfo.InvariantCulture) },
                                     { "data", JsonConvert.SerializeObject(terms, this._apiCaller.JsonSerializerSettings) }
                                 };
            var response = await this._apiCaller.Request<SyncResponse>("/v2/projects/sync", parameters).ConfigureAwait(false);

            return response.Terms;
        }

        /// <summary>
        /// Updates terms / translations by uploading a file.
        /// </summary>
        /// <remarks>No more than one request every 30 seconds.</remarks>
        /// <param name="id">The id of project</param>
        /// <param name="updating">What to update</param>
        /// <param name="stream"></param>
        /// <param name="overwriteTranslations">Set to true if you want to overwrite translations</param>
        /// <param name="syncTerms">Set to true if you want to sync your terms (terms that are not found in the uploaded file will be deleted from project and the new ones added). Ignored if updating = Translations</param>
        /// <param name="language">The language code. Required only if updating is terms_translations or translations.</param>
        /// <returns>Summary of terms and translations that were added or removed.</returns>
        public Task<UploadResponse> Upload(int id, UpdateTypes updating, Stream stream, bool overwriteTranslations = false, bool syncTerms = false, string language = null)
        {
            var typeLookup = new Dictionary<UpdateTypes, string>
                                 {
                                     { UpdateTypes.Terms, "terms" },
                                     {
                                         UpdateTypes.TermsAndTranslations,
                                         "terms_translations"
                                     },
                                     { UpdateTypes.Translations, "translations" }
                                 };

            var parameters = new Dictionary<string, string>();
            parameters.Add("updating", typeLookup[updating]);
            parameters.Add("id", id.ToString(CultureInfo.InvariantCulture));
            if (overwriteTranslations)
            {
                parameters.Add("overwrite", "1");
            }

            if (syncTerms)
            {
                parameters.Add("sync_terms", "1");
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                parameters.Add("language", language);
            }

            return this._apiCaller.Upload<UploadResponse>("/v2/projects/upload", parameters, stream);
        }

        /// <summary>
        /// Returns the link of the file (expires after 10 minutes).
        /// </summary>
        /// <returns>The link of the file (expires after 10 minutes).</returns>
        public async Task<Uri> Export(int id, string language, FileType type, IEnumerable<Filters> filters)
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     { "id", id.ToString(CultureInfo.InvariantCulture) },
                                     { "language", language },
                                     { "type", type.ToPOEditorString() },
                                     { "filters", filters.ToPOEditorString() }
                                 };
            var response = await this._apiCaller
                               .Request<FileExport>("/v2/projects/export", parameters)
                               .ConfigureAwait(false);

            return response.Url;
        }
    }
}