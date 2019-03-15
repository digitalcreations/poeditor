namespace POEditorAPI
{
    using System.Collections.Generic;

    public static class FileTypeExtensions
    {
        private static Dictionary<FileType, string> LookupTable = new Dictionary<FileType, string>
                                                                      {
                                                                          { FileType.PO, "po" },
                                                                          { FileType.POT, "pot"},
                                                                          { FileType.MO, "mo" },
                                                                          { FileType.XLS, "xls" },
                                                                          { FileType.CSV, "csv" },
                                                                          { FileType.RESW, "resw" },
                                                                          { FileType.RESX, "resx" },
                                                                          { FileType.AndroidStrings, "android_strings" },
                                                                          { FileType.AppleStrings, "apple_strings" },
                                                                          { FileType.XLIFF, "xliff" },
                                                                          { FileType.Properties, "properties" },
                                                                          { FileType.KeyValueJSON, "key_value_json" },
                                                                          { FileType.JSON, "json" },
                                                                          { FileType.XMB, "xmb" },
                                                                          { FileType.XTB, "xtb" }
                                                                      };

        public static string ToPOEditorString(this FileType type)
        {
            return LookupTable[type];
        }
    }
}