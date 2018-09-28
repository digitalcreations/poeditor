namespace POEditorAPI
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Term
    {
        [JsonProperty("term")]
        public string Name { get; set; }

        public string Context { get; set; }

        public string Plural { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public Translation Translation { get; set; }

        public ICollection<string> Tags { get; set; }

        public string Comment { get; set; }
    }
}