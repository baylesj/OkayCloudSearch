using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayCloudSearch.Query.Boolean
{
    public class ListBooleanCondition<T> : IBooleanCondition
    {
        public string Field { get; set; }
        public List<T> Conditions { get; protected set; }
        public bool IsOrCondition { get; set; }

        public string GetQueryString()
        {
            List<string> stringConditions = Conditions
                .Select(x => x is string ? "'" + x + "'"
                    : x.ToString()).ToList();

            return "(" + Field + ":(" + String.Join(Conditional, stringConditions) + "))";
        }

        private string Conditional
        {
            get
            {
                return IsOrCondition ?
                    Constants.Operators.Or.ToQueryString() : Constants.Operators.And.ToQueryString();
            }
        }

        public bool IsList()
        {
            return true;
        }
    }
}