using System.Diagnostics.CodeAnalysis;

namespace OkayCloudSearch.Contract.Facet
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Constraint
    {
        public string value { get; set; }
        public int count { get; set; }
    }
}
