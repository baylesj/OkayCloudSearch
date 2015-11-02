using System.Web.Script.Serialization;

namespace OkayCloudSearch.Contract
{
    public class SearchDocument
    {
        [ScriptIgnore]
        public string id { get; set; }
    }
}