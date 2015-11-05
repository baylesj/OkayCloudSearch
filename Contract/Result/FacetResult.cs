using System.Collections.Generic;
using OkayCloudSearch.Contract.Facet;

namespace OkayCloudSearch.Contract.Result
{
    public class FacetResult
    {
        public string Name { get; set; }
        public List<Constraint> Constraint { get; set; }
    }
}