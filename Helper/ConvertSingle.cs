using System;

namespace OkayCloudSearch.Helper
{
    class ConvertSingle
    {
        public int StringToInt(string entry)
        {
            int value;
            Int32.TryParse(entry, out value);

            return value;
        }

        public int? StringToIntNull(string entry)
        {
            int value;
            if (int.TryParse(entry, out value))
                return value;

            return null;
        }

        public DateTime StringToDate(string entry)
        {
            return Convert.ToDateTime(entry);
        }


        public string IntToString(int entry)
        {
            return entry.ToString();
        }
    }
}
