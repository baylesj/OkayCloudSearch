using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OkayCloudSearch.Query.Boolean;

namespace OkayCloudSearch.Query.Boolean
{
    public class StringBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }

        public StringBooleanCondition(string field, string condition)
        {
            Field = field;
            Condition = condition;
        }

        public string GetParam()
        {
            return Field + "%3A" + "'" + Condition + "'";
        }

		public bool IsOrCondition()
		{
			return false;
		}

		public bool IsList()
		{
			return false;
		}
    }
}