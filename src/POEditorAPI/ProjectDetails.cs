namespace POEditorAPI
{
    public class ProjectDetails : ProjectSummary
    {
        public string Description { get; set; }

        public string ReferenceLanguage { get; set; }

        public int Terms { get; set; }
    }
}