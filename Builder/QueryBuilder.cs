using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OkayCloudSearch.Contract;
using OkayCloudSearch.Query;
using OkayCloudSearch.Query.Boolean;
using OkayCloudSearch.Query.Facets;

namespace OkayCloudSearch.Builder
{
    public class QueryBuilder<T> where T : SearchDocument, new()
    {
        private readonly string _searchUri;

        public double MaxLevenshteinDistance = 0.3;
        private const short MaxKeywordLength = 255;

        public QueryBuilder(string searchUri)
        {
            _searchUri = searchUri;
        }

        public string BuildSearchQuery(SearchQuery<T> query)
        {
            if (!string.IsNullOrEmpty(query.PublicSearchQueryString))
            {
                return BuildFromPublicSearchQuery(query.PublicSearchQueryString, query);
            }

            var url = new StringBuilder(_searchUri);
            url.Append("?");

            FeedBooleanCriteria(query.Keyword, query.BooleanQuery, url);

            FeedFacet(query.Facets, url);

            FeedReturnFields(query.Fields, url);

            FeedMaxResults(query.Size, url);

            FeedStartResultFrom(query.Start, url);

            return url.ToString();
        }

        public string BuildFromPublicSearchQuery(string publicSearchQueryString, SearchQuery<T> query)
        {
            var url = new StringBuilder(_searchUri);
            url.Append("?");

            url.Append(publicSearchQueryString);

            FeedFacet(query.Facets, url);

            FeedReturnFields(query.Fields, url);

            FeedMaxResults(query.Size, url);

            FeedStartResultFrom(query.Start, url);

            return url.ToString();
        }

        public string BuildPublicSearchQuery(SearchQuery<T> query)
        {
            var url = new StringBuilder();

            FeedBooleanCriteria(null, query.BooleanQuery, url);

            FeedFacet(query.Facets, url);

            return url.ToString();
        }

        private void FeedBooleanCriteria(string keyword, BooleanQuery booleanQuery, StringBuilder url)
        {
            bool hasConditions = booleanQuery.Conditions != null && booleanQuery.Conditions.Any();
            if(String.IsNullOrWhiteSpace(keyword) && !hasConditions)
                return;

            bool hasParameters = (url.Length > 0);

            StringBuilder andConditions = new StringBuilder();
            List<string> orConditions = new List<string>();

            MoveConditionsToLists(booleanQuery, orConditions, andConditions);

            List<string> booleanConditions = new List<string>();

            if (andConditions.Length > 0)
            {
                // What does this line do??
                andConditions.Remove(andConditions.Length - 1, 1);
                booleanConditions.Add(andConditions.ToString());
            }

            if (orConditions.Count == 1)
            {
                booleanConditions.Add(orConditions[0]);
            }
            else if (orConditions.Count > 1)
            {
                booleanConditions.Add(JoinConditionsIntoQuery(orConditions));
            }

            TurnKeywordIntoCondition(keyword, booleanConditions);

            if (hasParameters)
            {
                url.Append("&");
            }

            url.Append("q.parser=lucene&q=");
            string query = JoinConditionsIntoQuery(booleanConditions);
            url.Append(query);
        }

        private static string JoinConditionsIntoQuery(List<string> conditions)
        {
            return "(" + String.Join(Constants.Operators.And.ToQueryString(), conditions.Select(x => "(" + x + ")").ToList()) + ")";
        }

        private void TurnKeywordIntoCondition(string keyword, List<string> booleanConditions)
        {
            if (!String.IsNullOrEmpty(keyword))
            {
                if (keyword.Length > MaxKeywordLength)
                {
                    keyword = TruncateKeyword(keyword);
                }
                var words = keyword.Split(' ').ToList();
                var conditions = words.Select(x => x + "~" + MaxLevenshteinDistance);
                var keywordConditions = String.Join(Constants.Operators.And.ToQueryString(), conditions);

                booleanConditions.Add(keywordConditions);
            }
        }

        private static string TruncateKeyword(string keyword)
        {
            keyword = keyword.Substring(0, 255);
            var index = keyword.LastIndexOf(' ');
            keyword = keyword.Substring(0, index);
            return keyword;
        }

        private static void MoveConditionsToLists(BooleanQuery booleanQuery, List<string> listOrConditions, StringBuilder andConditions)
        {
            foreach (var condition in booleanQuery.Conditions)
            {
                if (condition.IsOrCondition)
                {
                    listOrConditions.Add(condition.GetQueryString());
                }
                else
                {
                    andConditions.Append(condition.GetQueryString());
                    andConditions.Append(" AND ");
                }
            }
        }

        private void FeedStartResultFrom(int? start, StringBuilder url)
        {
            if (start != null)
            {
                url.Append("&");
                url.Append("start=");
                url.Append(start);
            }
        }

        private void FeedMaxResults(int? size, StringBuilder url)
        {
            if (size != null)
            {
                url.Append("&");
                url.Append("size=");
                url.Append(size);
            }
        }

        private void FeedFacet(List<Facet> facets, StringBuilder url)
        {
            FeedFacetList(facets, url);

            FeedFacetConstraints(facets, url);
        }

        private void FeedFacetList(List<Facet> facets, StringBuilder url)
        {
            if (facets == null || facets.Count==0)
                return;

            bool hasParameters = (url.Length > 0);

            if (hasParameters)
            {
                url.Append("&");
            }

            url.Append("facet=");

            Facet lastItem = facets.Last();
            foreach (var facet in facets)
            {
                url.Append(facet.Name);

                if (!ReferenceEquals(lastItem, facet))
                    url.Append(",");
            }

        }

        private void FeedFacetConstraints(List<Facet> facets, StringBuilder url)
        {
            if (facets == null || facets.Count == 0)
                return;

            foreach (var facet in facets)
            {
                FeedFacet(facet, url);
            }
        }

        private void FeedFacet(Facet facet, StringBuilder url)
        {
            if (string.IsNullOrEmpty(facet.Name))
                return;

            if(facet.TopResult != null)
            {
                AppendTopResult(facet, url);
            }

            if (facet.FacetConstraint != null)
            {
                var param = facet.FacetConstraint.GetRequestParam();
                AppendRequestParam(facet, url, param);
            }
        }

        private static void AppendRequestParam(Facet facet, StringBuilder url, string param)
        {
            if (param != null)
            {
                url.Append("&");
                url.Append("facet-");
                url.Append(facet.Name);
                url.Append("-constraints=");
                url.Append(param);
            }
        }

        private static void AppendTopResult(Facet facet, StringBuilder url)
        {
            url.Append("&");
            url.Append("facet-");
            url.Append(facet.Name);
            url.Append("-top-n=");
            url.Append(facet.TopResult);
        }

        private void FeedReturnFields(List<string> fields, StringBuilder url)
        {
            if (fields == null || fields.Count == 0)
                return;
            
            bool hasParameters = (url.Length > 0);

            if (hasParameters)
            {
                url.Append("&");
            }

            url.Append("return=");

            foreach (var field in fields)
            {
                url.Append(field);
                url.Append(",");
            }

            if (url.Length > 0)
            {
                url.Remove(url.Length - 1, 1);
            }
        }
    }
}