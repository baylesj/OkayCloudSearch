using System.Diagnostics.CodeAnalysis;
using System.Web.Script.Serialization;

namespace OkayCloudSearch.Contract
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SearchDocument
    {
        [ScriptIgnore]
        public string id { get; set; }
    }
}