using System.Diagnostics.CodeAnalysis;

namespace OkayCloudSearch.Contract
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class BasicDocumentAction
    {
        public string id { get; set; }
        public string type { get; set; }
        public int version { get; set; }
    }
}