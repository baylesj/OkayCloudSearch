using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using OkayCloudSearch.Contract;
using OkayCloudSearch.Contract.Result;
using OkayCloudSearch.Helper;

namespace OkayCloudSearch.Serialization
{
    class HitFeeder<T> where T : SearchDocument, new()
    {
        private readonly JavaScriptSerializer _serializer;
        private readonly ConvertSingle _convertSingle;
        private readonly ListProperties<T> _listProperties;

        public HitFeeder()
        {
            _convertSingle = new ConvertSingle();
            _serializer = new JavaScriptSerializer();
            _listProperties = new ListProperties<T>();
        }

        public void Feed(SearchResult<T> searchResult, dynamic dyHit)
        {
            searchResult.hits.hit = new List<SearchResult<T>.Hit<T>>();

            foreach(var hitDocument in dyHit)
            {
                Dictionary<string, string> jsonHitField = _serializer.Deserialize<Dictionary<string, string>>(hitDocument.fields.ToString());

                T hit = Map(jsonHitField);

                searchResult.hits.hit.Add(new SearchResult<T>.Hit<T> { id = hitDocument.id, data = hit });
            }
        }

        private T Map(Dictionary<string, string> data)
        {
            var hit = new T();

            foreach (var p in _listProperties.GetProperties())
            {
                string field = FindField(p.Name, data);

                if (field == null)
                    continue;

                if (p.PropertyType == typeof(string))
                    p.SetValue(hit, field, null);

                else if (p.PropertyType == typeof(List<int?>))
                    p.SetValue(hit, _convertSingle.StringToIntNull(field), null);

                else if(p.PropertyType == typeof(List<int>))
                    p.SetValue(hit, _convertSingle.StringToInt(field), null);

                else if (p.PropertyType == typeof(List<DateTime>))
                    p.SetValue(hit, _convertSingle.StringToDate(field), null);

                else if (p.PropertyType == typeof(string))
                    p.SetValue(hit, field.FirstOrDefault(), null);

                else if (p.PropertyType == typeof(int?))
                    p.SetValue(hit, _convertSingle.StringToIntNull(field), null);

                else if (p.PropertyType == typeof(int))
                    p.SetValue(hit, _convertSingle.StringToInt(field), null);

                else if (p.PropertyType == typeof(DateTime))
                    p.SetValue(hit, _convertSingle.StringToDate(field), null);
            }

            return hit;
        }

        private string FindField(string propertyName, Dictionary<string, string> data)
        {
            return data.FirstOrDefault(d => d.Key.ToLowerInvariant() == propertyName.ToLowerInvariant()).Value;
        }
    }
}