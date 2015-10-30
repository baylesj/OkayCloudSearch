using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OkayCloudSearch.Enum
{
    public enum ActionType
    {
        ADD,
        DELETE
    }

    public class ActionTypeFunction
    {
        public static string ActionTypeToString(ActionType type)
        {
            return type.ToString().ToLower();
        }
    }
}
