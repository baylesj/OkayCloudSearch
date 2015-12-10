using System.Collections.Generic;

namespace OkayCloudSearch.Query.Boolean
{
    public class IntListBooleanCondition : ListBooleanCondition<int>
    {
        public IntListBooleanCondition(string field, List<int> conditions,
            bool isOrCondition = true)
        {
            Field = field;
            Conditions = new List<int> (conditions);
            IsOrCondition = isOrCondition;
        }
    }
}
