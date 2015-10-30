using System.Collections.Generic;

namespace OkayCloudSearch.Query.Facets
{
    public interface IFacetConstraint
    {
        string GetRequestParam();
    }
}