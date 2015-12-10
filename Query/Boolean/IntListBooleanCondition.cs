using System.Collections.Generic;
using System.Text;

namespace OkayCloudSearch.Query.Boolean
{
    public class IntListBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public List<int> Conditions { get; private set; }
        public bool IsOrCondition { get; set; }

        public IntListBooleanCondition(string field, List<int> conditions)
        :this (field, conditions, true)
        {
        }

        public IntListBooleanCondition(string field, List<int> conditions, bool isOrCondition)
        {
            Field = field;
            Conditions = conditions;
            IsOrCondition = isOrCondition;
        }

        public string GetQueryString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (int c in Conditions)
            {
                builder.Append(Field + "%3A" + c);
                builder.Append("+");
            }

            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return "(" + builder + ")";
        }

        public bool IsList()
        {
            return true;
        }
    }
}
