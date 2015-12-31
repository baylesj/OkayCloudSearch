namespace OkayCloudSearch.Query.Boolean
{
    public class StringBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }
        public bool IsOrCondition { get { return false; } }

        public StringBooleanCondition(string field, string condition)
        {
            Field = field;
            Condition = condition;
        }

        public string GetQueryString()
        {
            return "(" + Field + ":" + "'" + Condition + "')";
        }

        public bool IsList()
        {
            return false;
        }
    }
}