namespace POEditorAPI
{
    using System;

    public class Translation
    {
        public string Content { get; set; }

        public bool Fuzzy { get; set; }

        public bool ProofRead { get; set; }

        public DateTime? Updated { get; set; }
    }
}