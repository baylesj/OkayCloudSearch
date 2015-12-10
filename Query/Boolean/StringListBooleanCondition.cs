using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayCloudSearch.Query.Boolean
{
    public class StringListBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public List<string> Conditions { get; private set; }
        public bool IsOrCondition { get; set; }

        private string Conditional
        {
            get
            {
                return IsOrCondition ?
                    Constants.Operators.Or.ToQueryString() : Constants.Operators.And.ToQueryString();
            }
        }

        public StringListBooleanCondition(string field, List<string> conditions)
        : this(field, conditions, true)
        {
        }

        public StringListBooleanCondition(string field, List<string> conditions, bool isOrCondition)
        {
            Field = field;
            Conditions = conditions;
            IsOrCondition = isOrCondition;
        }

        public string GetQueryString()
        {
            return "(" + Field + ":(" + String.Join(Conditional, Conditions.Select(x => "\"" + x + "\"")) + "))";
        }

        public bool IsList()
        {
            return true;
        }
    }
}
