namespace OkayCloudSearch.Query.Boolean
{
    public class IntBooleanCondition : BooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }
        public override bool IsOrCondition { get; set; }

        public IntBooleanCondition(string field)
        {
            Field = field;
        }

        public IntBooleanCondition(string field, int condition)
            : this(field, condition, true)
        {
        }

        public IntBooleanCondition(string field, int condition, bool isOrConditionParam)
        {
            Field = field;
            Condition = condition.ToString();
            IsOrCondition = isOrConditionParam;
        }

        public void SetInterval(int from, int to)
        {
            Condition = IntegerRange.GetInterval(from, to);
        }

        public void SetFrom(int from)
        {
            Condition = IntegerRange.GetInterval(from, null);
        }

        public void SetTo(int to)
        {
            Condition = IntegerRange.GetInterval(null, to);
        }

        public override string GetQueryString()
        {
            return "(" + Field + ":" + Condition.Replace(" ", "+") + ")";
        }

        public override bool IsList()
        {
            return false;
        }
    }
}