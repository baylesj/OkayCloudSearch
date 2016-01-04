using System;

namespace OkayCloudSearch.Query.Boolean
{
    public class StringBooleanCondition : BooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }
        public override bool IsOrCondition { get { return false; }
            set { throw new NotImplementedException(); }
        }

        public StringBooleanCondition(string field, string condition)
        {
            Field = field;
            Condition = condition;
        }

        public override string GetQueryString()
        {
            return "(" + Field + ":" + "'" + Condition.Replace(" ", "+") + "')";
        }

        public override bool IsList()
        {
            return false;
        }
    }
}