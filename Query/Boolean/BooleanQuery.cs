using System.Collections.Generic;

namespace OkayCloudSearch.Query.Boolean
{
    public class BooleanQuery
    {

        public List<IBooleanCondition> Conditions;

        public BooleanQuery()
        {
            Conditions = new List<IBooleanCondition>();
        }
    }
}