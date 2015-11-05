using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayCloudSearch.Helper
{
    public class ConvertArray
    {

        public static List<int> StringToInt(List<string> newValues)
        {
            var list = new List<int>();

            foreach (var entry in newValues)
            {
                int value;
                if (int.TryParse(entry, out value))
                    list.Add(value);
            }

            return list;
        }

        public List<int?> StringToIntNull(List<string> newValues)
        {
            var list = new List<int?>();

            foreach (var entry in newValues)
            {
                int value;
                if (int.TryParse(entry, out value))
                    list.Add(value);
                else
                    list.Add(null);
            }

            return list;
        }

        public List<DateTime> StringToDate(List<string> newValues)
        {
            return newValues.Select(Convert.ToDateTime).ToList();
        }


        public List<string> IntToString(List<int> constraints)
        {
            return constraints.Select(entry => entry.ToString()).ToList();
        }
    }
}