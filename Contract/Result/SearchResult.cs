using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using OkayCloudSearch.Contract.Facet;

namespace OkayCloudSearch.Contract.Result
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SearchResult<T> where T : SearchDocument, new()
    {
        public bool IsError { get; set; }

        public string rank { get; set; }

        [JsonProperty("match-expr")]
        public string matchExpr { get; set; }

        public Hits<T> hits { get; set; }
        public Info info { get; set; }
        public string error { get; set; }
        public List<Message> messages { get; set; }

        public List<FacetResult> facetsResults { get; set; }

        public class Message
        {
            public string severity { get; set; }
            public string code { get; set; }
            public string message { get; set; }
        }

        public class Hits<TD>
        {
            public int found { get; set; }
            public int start { get; set; }
            public List<Hit<TD>> hit { get; set; }
        }

        public class Hit<TD>
        {
            public string id { get; set; }
            public TD data { get; set; }
        }

        public class Info
        {
            public string rid { get; set; }

            [JsonProperty("time-ms")]
            public int timeMs { get; set; }

            [JsonProperty("cpu-time-ms")]
            public int cpuTimeMs { get; set; }
        }

        public List<Constraint> GetFacetResults(string name)
        {
            List<Constraint> constraints = null;

            if (facetsResults != null && facetsResults.Count > 0)
            {
                foreach (FacetResult facetResult in facetsResults)
                {
                    if (facetResult.Name == name)
                    {
                        constraints = facetResult.Constraint;
                        break;
                    }
                }
            }

            return constraints;
        }
    }
}
