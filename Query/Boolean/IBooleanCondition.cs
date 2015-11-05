namespace OkayCloudSearch.Query.Boolean
{
    public interface IBooleanCondition
    {
        string GetQueryString();
        bool IsOrCondition { get; }
        bool IsList();
    }
}