namespace OkayCloudSearch.Query.Boolean
{
    public abstract class BooleanCondition
    {
        protected virtual string UrlEncodeCondition(string condition)
        {
            return condition.Replace(" ", "+");
        }

        public abstract string GetQueryString();
        public abstract bool IsOrCondition { get; set; }
        public abstract bool IsList();
    }
}