using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OkayCloudSearch.Contract.Facet;

namespace OkayCloudSearch.Contract.Result
{
    public class FacetResult
    {
        public string Name { get; set; }
        public List<Constraint> Contraint { get; set; }
    }
}