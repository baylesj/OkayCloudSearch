using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayCloudSearch.Query.Boolean
{
    public class ListBooleanCondition<T> : BooleanCondition
    {
        public string Field { get; set; }
        public List<T> Conditions { get; protected set; }
        public override bool IsOrCondition { get; set; }

        public override string GetQueryString()
        {
            List<string> stringConditions = Conditions
                .Select(x => x is string ? "\"" + (x as string).Replace(" ", "+") + "\""
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

        public override bool IsList()
        {
            return true;
        }
    }
}