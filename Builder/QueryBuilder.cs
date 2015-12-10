using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OkayCloudSearch.Contract;
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

            var url = new StringBuilder(_searchUri).Append("?");

            FeedBooleanCriteria(query.Keyword, query.BooleanQuery, url);

            FeedReturnFields(query.Fields, url);

            FeedMaxResults(query.Size, url);

            FeedStartResultFrom(query.Start, url);

            return url.ToString();
        }

        private void FeedBooleanCriteria(string keyword, BooleanQuery booleanQuery, StringBuilder url)
        {
            bool hasConditions = booleanQuery != null 
                && booleanQuery.Conditions != null 
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

        private static void AppendConditionsToBuilder(StringBuilder url, List<string> booleanConditions)
        {
            AppendSeparator(url);
            url.Append("q.parser=lucene&q=");
            string query = JoinConditionsIntoQuery(booleanConditions);
            url.Append(query);
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
                var conditions = SplitKeywordIntoConditions(keyword);
                var conditionsList = JoinConditionsList(conditions);

                booleanConditions.Add(conditionsList);
            }
        }

        private List<string> SplitKeywordIntoConditions(string keyword)
        {
            var words = keyword.Split(' ').ToList();
            var conditions = words.Select(x =>
                "(" + x + Constants.Operators.Or.ToQueryString()
                + x + "~" + MaxLevenshteinDistance + ")").ToList();
            return conditions;
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

        private void FeedStartResultFrom(int? start, StringBuilder url)
        {
            if (start.HasValue)
            {
                if (start.Value < 0)
                    throw new InvalidOperationException();

                AppendSeparator(url);
                url.Append("start=");
                url.Append(start.Value);
            }
        }

        private void FeedMaxResults(int? size, StringBuilder url)
        {
            if (size.HasValue)
            {
                if (size <= 0)
                    throw new InvalidOperationException();

                AppendSeparator(url);
                url.Append("size=");
                url.Append(size);
            }
        }


        private void FeedReturnFields(List<string> fields, StringBuilder url)
        {
            if (fields == null || fields.Count == 0)
                return;

            AppendSeparator(url);
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

        private static void AppendSeparator(StringBuilder url)
        {
            var hasParameters = CheckIfStringBuilderHasQueryParameters(url);
            if (hasParameters)
            {
                url.Append("&");
            }
        }

        private static bool CheckIfStringBuilderHasQueryParameters(StringBuilder url)
        {
            bool hasParameters = false;
            string soFar = url.ToString();
            string[] portions = soFar.Split('?');
            if (portions.Length == 2 && portions[1].Length > 0)
                hasParameters = true;
            return hasParameters;
        }
    }
}