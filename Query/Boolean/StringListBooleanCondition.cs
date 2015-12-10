using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayCloudSearch.Query.Boolean
{
    public class StringListBooleanCondition : ListBooleanCondition<string>
    {
        public StringListBooleanCondition(string field, List<string> conditions,
            bool isOrCondition = true)
        {
            Field = field;
            Conditions = new List<string> (conditions);
            IsOrCondition = isOrCondition;
        }
    }
}
