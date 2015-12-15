using System.Diagnostics.CodeAnalysis;

namespace OkayCloudSearch.Contract
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class AddUpdateBasicDocumentAction<T> : BasicDocumentAction where T : SearchDocument
    {
        public string lang { get; set; }
        public T fields { get; set; }
    }
}
