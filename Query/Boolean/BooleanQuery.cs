using System.Collections.Generic;

namespace OkayCloudSearch.Query.Boolean
{
    public class BooleanQuery
    {

        public List<BooleanCondition> Conditions;

        public BooleanQuery()
        {
            Conditions = new List<BooleanCondition>();
        }
    }
}