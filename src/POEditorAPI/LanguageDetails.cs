namespace POEditorAPI
{
    using System;

    public class LanguageDetails : Language
    {
        public int Translations { get; set; }

        public decimal Percentage { get; set; }

        public DateTime? Updated { get; set; }
    }
}