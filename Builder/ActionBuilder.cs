using OkayCloudSearch.Contract;
using OkayCloudSearch.Enum;
using OkayCloudSearch.Helper;

namespace OkayCloudSearch.Builder
{
    public class ActionBuilder<T> where T : SearchDocument
    {
        public AddUpdateBasicDocumentAction<T> BuildAction(T document, ActionType actionType, int version)
        {
            var type = ActionTypeFunction.ActionTypeToString(actionType);

            return new AddUpdateBasicDocumentAction<T> { type = type, id = document.id, lang = "en", fields = document, version = version };
        }

        public AddUpdateBasicDocumentAction<T> BuildAction(T document, ActionType actionType)
        {
            int version = Timestamp.CurrentTimestamp();

            return BuildAction(document, actionType, version);
        }


        public BasicDocumentAction BuildDeleteAction(SearchDocument document, ActionType actionType, int version)
        {
            var type = ActionTypeFunction.ActionTypeToString(actionType);

            return new BasicDocumentAction { type = type, id = document.id, version = version };
        }

        public BasicDocumentAction BuildDeleteAction(SearchDocument document, ActionType actionType)
        {
            int version = Timestamp.CurrentTimestamp();

            return BuildDeleteAction(document, actionType, version);
        }
    }
}