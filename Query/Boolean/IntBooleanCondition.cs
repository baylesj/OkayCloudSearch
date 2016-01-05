namespace OkayCloudSearch.Query.Boolean
{
    public sealed class IntBooleanCondition : SingleBooleanCondition
    {
        public override bool IsOrCondition { get; set; }

        public IntBooleanCondition(string field)
            : base(field, "")
        {
            Field = field;
        }

        public IntBooleanCondition(string field, int condition)
            : this(field, condition, true)
        {
        }

        public IntBooleanCondition(string field, int condition, bool isOrConditionParam)
            : base(field, condition.ToString())
        {
            IsOrCondition = isOrConditionParam;
        }

        protected override string EncodeCondition(string condition)
        {
            return condition;
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
    }
}