using System.Collections.Generic;

namespace AmazingCloudSearch.Query.Facets
{
    public interface IFacetConstraint
    {
        string GetRequestParam();
    }
}