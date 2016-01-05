using System;

namespace OkayCloudSearch.Query.Boolean
{
    public class SingleBooleanCondition : BooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }
        public override bool IsOrCondition { get { return false; }
            set { throw new NotImplementedException(); }
        }

        public SingleBooleanCondition(string field, string condition)
        {
            Field = field;
            Condition = condition;
        }

        public override string GetQueryString()
        {
            return "(" + Field + ":" + EncodeCondition(Condition) + ")";
        }

        public override bool IsList()
        {
            return false;
        }
    }
}