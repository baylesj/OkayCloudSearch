namespace OkayCloudSearch.Query.Boolean
{
    public abstract class BooleanCondition
    {
        public abstract string GetQueryString();
        public abstract bool IsOrCondition { get; set; }
        public abstract bool IsList();

        protected virtual string EncodeCondition(string condition)
        {
            return "\"" + condition.Replace(" ", "+") + "\"";
        }
    }
}