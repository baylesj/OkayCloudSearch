namespace OkayCloudSearch.Query.Boolean
{
    public interface IBooleanCondition
    {
        string GetParam();
        bool IsOrCondition();
        bool IsList();
    }
}