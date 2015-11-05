using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OkayCloudSearch.Query.Facets
{
    public class StringFacetConstraints : IFacetConstraint
    {
        public List<string> Constraint { get; set; }

        public StringFacetConstraints()
        {
            Constraint = new List<string>();
        }

        public string AddConstraint(string contrain)
        {
            Constraint.Add(contrain);
            return contrain;
        }

        public string GetRequestParam()
        {
            if (Constraint.Count == 0)
                return null;


            StringBuilder s = new StringBuilder();

            var lastItem = Constraint.Last();
            foreach (var c in Constraint)
            {
                s.Append("'");
                s.Append(c);
                s.Append("'");

                if (!ReferenceEquals(lastItem, c))
                    s.Append(",");
            }

            return s.ToString();
        }

    }
}