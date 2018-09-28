namespace POEditorAPI
{
    using System;

    public class ProjectSummary
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Public { get; set; }

        public bool Open { get; set; }

        public DateTime Created { get; set; }
    }
}