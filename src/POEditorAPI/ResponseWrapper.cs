namespace POEditorAPI
{
    public class ResponseWrapper<T>
    {
        public ResponseMeta Response { get; set; }

        public T Result { get; set; }
    }
}