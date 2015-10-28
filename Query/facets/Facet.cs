using AmazingCloudSearch.Query;

namespace AmazingCloudSearch.Query.Facets
{
    public class Facet
    {
        public string Name { get; set; }
        public IFacetConstraint FacetConstraint { get; set; }
        public int? TopResult { get; set; }
    }
}