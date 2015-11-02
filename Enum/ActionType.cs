namespace OkayCloudSearch.Enum
{
    public enum ActionType
    {
        Add,
        Delete
    }

    public class ActionTypeFunction
    {
        public static string ActionTypeToString(ActionType type)
        {
            return type.ToString().ToLower();
        }
    }
}
