﻿using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using OkayCloudSearch.Contract.Facet;
using OkayCloudSearch.Contract.Result;

namespace OkayCloudSearch.Serialization
{
    class FacetBuilder
    {
        private readonly JavaScriptSerializer _serializer;

        public FacetBuilder()
        {
            _serializer = new JavaScriptSerializer();
        }

        public List<FacetResult> BuildFacet(dynamic jsonDynamic)
        {
            try
            {
                return BuildFacetWithException(jsonDynamic);
            }
            catch(Exception)
            {
                return new List<FacetResult>();
            }
        }

        private List<FacetResult> BuildFacetWithException(dynamic jsonDynamic)
        {
            dynamic facets = jsonDynamic.facets;
            if (facets == null)
                return null;

            string jsonFacet = facets.ToString();

            var facetDictionary = _serializer.Deserialize<Dictionary<string, object>>(jsonFacet);

            var liFacet = new List<FacetResult>();

            foreach (KeyValuePair<string, object> pair in facetDictionary)
            {
                Constraints constraints = CreateConstraint(pair);
                liFacet.Add(new FacetResult { Name = pair.Key, Constraint = constraints.constraints });
            }

            return liFacet;
        }

        private Constraints CreateConstraint(KeyValuePair<string, object> pair)
        {
            try
            {
                var tmpConstraints = JsonConvert.SerializeObject(pair.Value);
                var constraints = JsonConvert.DeserializeObject<Constraints>(tmpConstraints);
                return constraints;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
