using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OkayCloudSearch.Contract.Facet
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Constraints
    {
        public List<Constraint> constraints { get; set; }
    }
}
