namespace POEditor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    interface IFolderManager
    {
        string SearchPattern { get; }

        string NameFromFilePath(string relativePath, string entryName);

        IEnumerable<Translation> GetResources(string path, string basePath);

        /// <summary>
        /// Parse name into relative path and entry name.
        /// </summary>
        /// <param name="name">Name, as output from NameFromFilePath</param>
        /// <param name="language">Language code</param>
        /// <returns>Tuple, first is relative path to translation file, second is entry name</returns>
        Tuple<string, string> ParseName(string name, string language);

        void SetResources(string rootPath, ICollection<Translation> translations, string defaultLanguage);
    }
}