using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayCloudSearch.Helper
{
    public class ConvertArray
    {

        public List<int> StringToInt(List<string> newValues)
        {
            var r = new List<int>();

            foreach (var entry in newValues)
            {
                int value;
                if (int.TryParse(entry, out value))
                    r.Add(value);
            }

            return r;
        }

        public List<int?> StringToIntNull(List<string> newValues)
        {
            var r = new List<int?>();

            foreach (var entry in newValues)
            {
                int value;
                if (int.TryParse(entry, out value))
                    r.Add(value);
                else
                    r.Add(null);
            }

            return r;
        }

        public List<DateTime> StringToDate(List<string> newValues)
        {
            return newValues.Select(Convert.ToDateTime).ToList();
        }


        public List<string> IntToString(List<int> contraints)
        {
            return contraints.Select(entry => entry.ToString()).ToList();
        }
    }
}