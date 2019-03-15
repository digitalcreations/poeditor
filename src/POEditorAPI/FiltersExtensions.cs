namespace POEditorAPI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class FiltersExtensions
    {
        public static string ToPOEditorString(this IEnumerable<Filters> filters)
        {
            return JsonConvert.SerializeObject(filters.Select(f => ToPOEditorString((Filters)f)).ToArray());
        }

        public static string ToPOEditorString(this Filters filter)
        {
            return new SnakeCaseNamingStrategy().GetPropertyName(Enum.GetName(typeof(Filters), filter), false);
        }
        
    }
}