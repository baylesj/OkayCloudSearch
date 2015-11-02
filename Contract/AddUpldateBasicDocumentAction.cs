namespace OkayCloudSearch.Contract
{

    public class AddUpldateBasicDocumentAction<T> : BasicDocumentAction where T : SearchDocument
    {
        public string lang { get; set; }
        public T fields { get; set; }
    }
}
