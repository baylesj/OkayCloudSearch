using System;
using System.Text;

namespace OkayCloudSearch.Helper
{
    public class QueryHelper
    {
        private readonly StringBuilder _url;
        private bool _hasFirstParameter;

        private const char Separator = '&';
        private const char FieldEquals = '=';
        private const char QuerySeparator = '?';

        public QueryHelper(string baseString)
        {
            if (baseString == null)
                throw new ArgumentNullException("baseString", "Must be non-null.");

            _hasFirstParameter = false;
            _url = new StringBuilder(baseString);
        }

        public void AppendField(string field, string value)
        {
            if (String.IsNullOrEmpty(field))
                throw new ArgumentOutOfRangeException("field", "must non zero length.");

            AppendSeparator();

            _url.Append(field).Append(FieldEquals).Append(value);
        }

        private void AppendSeparator()
        {
            if (_hasFirstParameter)
            {
                _url.Append(Separator);
            }
            else
            {
                _url.Append(QuerySeparator);
                _hasFirstParameter = true;
            }
        }

        public override string ToString()
        {
            return _url.ToString();
        }
    }
}
