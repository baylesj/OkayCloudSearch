using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using OkayCloudSearch.Contract;
using OkayCloudSearch.Helper;
using OkayCloudSearch.Query;
using OkayCloudSearch.Query.Boolean;


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
            if (query == null)
                throw new ArgumentNullException();

            QueryHelper helper = new QueryHelper(_searchUri);

            FeedQuery(query, helper);

            return helper.ToString();
        }

        private void FeedQuery(SearchQuery<T> query, QueryHelper helper)
        {

            FeedBooleanCriteria(query.Keyword, query.BooleanQuery, helper);
            FeedReturnFields(query.Fields, helper);
            FeedMaxResults(query.Size, helper);
            FeedStartResultFrom(query.Start, helper);
            FeedSortParameter(query.SortField, query.ShouldSortAscending, helper);
        }

        private void FeedSortParameter(string sortField, bool ascending, QueryHelper url)
        {
            if (!String.IsNullOrEmpty(sortField))
            {
                string value = sortField + " " + (ascending ? "asc" : "desc");
                url.AppendField("sort", value);
            }
        }

        private void FeedBooleanCriteria(string keyword, BooleanQuery booleanQuery, QueryHelper url)
        {
            bool hasConditions = (booleanQuery != null) 
                && (booleanQuery.Conditions != null) 
                && booleanQuery.Conditions.Any();
            if(String.IsNullOrWhiteSpace(keyword) && !hasConditions)
                return;

            var booleanConditions = GenerateBooleanConditions(booleanQuery);

            TurnKeywordIntoCondition(keyword, booleanConditions);

            AppendConditionsToBuilder(url, booleanConditions);
        }

        private static List<string> GenerateBooleanConditions(BooleanQuery booleanQuery)
        {
            StringBuilder andConditions = new StringBuilder();
            List<string> orConditions = new List<string>();
            List<string> booleanConditions = new List<string>();

            MoveConditionsToLists(booleanQuery, orConditions, andConditions);
            if (andConditions.Length > 0)
            {
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

            return booleanConditions;
        }

        private static void AppendConditionsToBuilder(QueryHelper url, List<string> booleanConditions)
        {
            url.AppendField("q.parser", "lucene");
            string query = JoinConditionsIntoQuery(booleanConditions);
            url.AppendField("q", EncodeQueryStringSubsection(query));
        }

        private static string JoinConditionsIntoQuery(List<string> conditions)
        {
            if (conditions.Count == 0)
                return "";
            if (conditions.Count == 1)
                return conditions.First();

            return "(" + String.Join(Constants.Operators.And.ToQueryString(), 
                conditions) + ")";
        }

        private void TurnKeywordIntoCondition(string keyword, List<string> booleanConditions)
        {
            if (!String.IsNullOrEmpty(keyword))
            {
                if (keyword.Length > MaxKeywordLength)
                {
                    keyword = TruncateKeyword(keyword);
                }
                keyword = DecodeWhiteSpacesForParsing(keyword);
                var conditions = SplitKeywordIntoConditions(keyword);
                var conditionsList = JoinConditionsList(conditions);

                if (conditions.Count > 1)
                    conditionsList = "(" + keyword + Constants.Operators.Or.ToQueryString() +
                                     conditionsList + ")";

                booleanConditions.Add(conditionsList);
            }
        }

        private List<string> SplitKeywordIntoConditions(string keyword)
        {
            var words = keyword.Split().ToList();
            var conditions = words.Select(x =>
                "(" + x + Constants.Operators.Or.ToQueryString()
                + x + "~" + MaxLevenshteinDistance + ")").ToList();
            return conditions;
        }

        private static string EncodeQueryStringSubsection(string s)
        {
            return Uri.EscapeDataString(s);
            //    .Replace("%20", " ").Replace("%27", "'")
            //    .Replace("%28", "(").Replace("%29", ")").Replace("%3A", ":");

        }

        private static string DecodeWhiteSpacesForParsing(string keyword)
        {
            keyword = keyword.Replace("%20", " ");
            return keyword;
        }

        private static string JoinConditionsList(List<string> conditions)
        {
            if (conditions.Count < 1)
                return "";
            if (conditions.Count == 1)
                return conditions.First();
            return "(" + String.Join
                (Constants.Operators.And.ToQueryString(), conditions) + ")";
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
            List<string> temporaryAndList = new List<string>();
            if (booleanQuery != null)
            {
                foreach (var condition in booleanQuery.Conditions)
                {
                    if (condition.IsOrCondition)
                    {
                        listOrConditions.Add(condition.GetQueryString());
                    }
                    else
                    {
                        temporaryAndList.Add(condition.GetQueryString());
                    }
                }                
            }

            andConditions.Append(String.Join(Constants.Operators.And.ToQueryString(), temporaryAndList));
        }

        private void FeedStartResultFrom(int? start, QueryHelper url)
        {
            if (start.HasValue)
            {
                if (start.Value < 0)
                    throw new InvalidOperationException();

                url.AppendField("start", start.Value.ToString());
            }
        }

        private void FeedMaxResults(int? size, QueryHelper url)
        {
            if (size.HasValue)
            {
                if (size <= 0)
                    throw new InvalidOperationException();

                url.AppendField("size", size.Value.ToString());
            }
        }


        private void FeedReturnFields(List<string> fields, QueryHelper url)
        {
            if (fields == null || fields.Count == 0)
                return;

            string value = String.Join(",", fields);
            url.AppendField("return", value);
        }
    }
}