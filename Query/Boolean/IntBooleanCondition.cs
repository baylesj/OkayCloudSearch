namespace OkayCloudSearch.Query.Boolean
{
    public class IntBooleanCondition : IntegerRange, IBooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }
        public bool IsOrCondition { get; set; }

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
            Condition = GetInterval(from, to);
        }

        public void SetFrom(int from)
        {
            Condition = GetInterval(from, null);
        }

        public void SetTo(int to)
        {
            Condition = GetInterval(null, to);
        }

        public string GetQueryString()
        {
            return "(" + Field + ":" + Condition + ")";
        }

        public bool IsList()
        {
            return false;
        }
    }
}