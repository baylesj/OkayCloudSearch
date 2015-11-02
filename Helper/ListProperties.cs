using System.Collections.Generic;
using System.Reflection;

namespace OkayCloudSearch.Helper
{
    class ListProperties<T>
    {
        private List<PropertyInfo> _properties;

        // http://stackoverflow.com/questions/8411838/c-sharp-lock-in-generic-function
        // ReSharper disable once UnusedTypeParameter
        static class TypeLock<TD>
        {
            // ReSharper disable once StaticMemberInGenericType
            public static readonly object SyncLock = new object();
        }

        public List<PropertyInfo> GetProperties()
        {
            if (_properties != null)
                return _properties;

            lock (TypeLock<T>.SyncLock)
            {
                return _properties ?? (_properties = ToList());
            }
        }

        public List<PropertyInfo> ToList()
        {
            _properties = new List<PropertyInfo>();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                if (!p.CanWrite || !p.CanRead)
                    continue;

                if (!AreGetterAndSetterBothPublic(p))
                    continue;

                _properties.Add(p);
            }
            return _properties;
        }

        private bool AreGetterAndSetterBothPublic(PropertyInfo prop)
        {
            if (prop.GetGetMethod(false) == null || prop.GetSetMethod(false) == null)
                return false;

            return true;
        }
    }
}
