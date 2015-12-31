using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using OkayCloudSearch.Builder;
using OkayCloudSearch.Contract;
using OkayCloudSearch.Query;
using OkayCloudSearch.Query.Boolean;
using Xunit;

namespace OkayCloudSearch.Tests.Builder
{
    public class QueryBuilderTests : IDisposable
    {
        private const string BaseUri = "http://test.com/";

        public SearchDocument TestDocument;
        public SearchQuery<SearchDocument> TestQuery; 
        public QueryBuilder<SearchDocument> QueryBuilder;

        public QueryBuilderTests()
        {
            TestDocument = new SearchDocument();
            TestQuery = new SearchQuery<SearchDocument>();
            QueryBuilder = new QueryBuilder<SearchDocument>(BaseUri);
        }

        [Fact]
        public void NullQueryReturnsEmptyString()
        {
            Assert.Throws<ArgumentNullException>(
                () => QueryBuilder.BuildSearchQuery(null));
        }

        [Fact]
        public void NullBooleanQueryAddsNoParameter()
        {
            TestQuery.BooleanQuery = null;
            var parsed = AssertValidQuery(TestQuery);

            Assert.False(parsed.AllKeys.ToList().Contains("q"));
        }

        [Fact]
        public void EmptyBooleanQueryAddsNoParameter()
        {
            TestQuery.BooleanQuery = new BooleanQuery();
            var parsed = AssertValidQuery(TestQuery);

            Assert.False(parsed.AllKeys.ToList().Contains("q"));
        }

        [Fact]
        public void IntBooleanQueryAddsIntParameter()
        {
            TestQuery.BooleanQuery = new BooleanQuery();
            TestQuery.BooleanQuery.Conditions.Add(
                new IntBooleanCondition("ThriftyThree", 33));
            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("(ThriftyThree:33)", parsed["q"]);
            Assert.Equal("lucene", parsed["q.parser"]);
        }

        [Fact]
        public void MultipleBooleanQueryAddsCompoundParameter()
        {
            TestQuery.BooleanQuery = new BooleanQuery
            {
                Conditions = new List<IBooleanCondition>
                {
                    new IntBooleanCondition("ThriftyThree", 33),
                    new IntBooleanCondition("FourthyFour", 44)
                }
            };

            var parsed = AssertValidQuery(TestQuery);

            Assert.True(
                ("((FourthyFour:44) AND (ThriftyThree:33))" == parsed["q"])
             || ("((ThriftyThree:33) AND (FourthyFour:44))" == parsed["q"])
                ); 
            Assert.Equal("lucene", parsed["q.parser"]);
        }

        [Fact]
        public void MultipleTypeBooleanQueryAddsCompoundParameter()
        {
            TestQuery.BooleanQuery = new BooleanQuery
            {
                Conditions = new List<IBooleanCondition>
                {
                    new IntBooleanCondition("ThriftyThree", 33),
                    new StringBooleanCondition("FourthyFour", "Number")
                }
            };

            var parsed = AssertValidQuery(TestQuery);

            Assert.True(
                ("((FourthyFour:'Number') AND (ThriftyThree:33))" == parsed["q"])
              || ("((ThriftyThree:33) AND (FourthyFour:'Number'))" == parsed["q"])
                );
            Assert.Equal("lucene", parsed["q.parser"]);
        }

        [Fact]
        public void IntListBooleanQueryAddsIntParameter()
        {
            TestQuery.BooleanQuery = new BooleanQuery();
            TestQuery.BooleanQuery.Conditions.Add(
                new IntListBooleanCondition("ThriftyThree", 
                    new List<int>{33, 44, 55}));
            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("(ThriftyThree:(33 OR 44 OR 55))", parsed["q"]);
            Assert.Equal("lucene", parsed["q.parser"]);
        }

        [Fact]
        public void StringBooleanQueryAddsStringParameter()
        {
            TestQuery.BooleanQuery = new BooleanQuery();
            TestQuery.BooleanQuery.Conditions.Add(
                new StringBooleanCondition("ThriftyThree", "Turtle"));
            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("(ThriftyThree:'Turtle')", parsed["q"]);
            Assert.Equal("lucene", parsed["q.parser"]);
        }

        [Fact]
        public void StringListBooleanQueryAddsStringParameter()
        {
            TestQuery.BooleanQuery = new BooleanQuery();
            TestQuery.BooleanQuery.Conditions.Add(
                new StringListBooleanCondition("ThriftyThree", 
                    new List<string>{"Turtle", "Rabbit", "Squirrel"}));
            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("(ThriftyThree:('Turtle' OR 'Rabbit' OR 'Squirrel'))",
                parsed["q"]);
            Assert.Equal("lucene", parsed["q.parser"]);
        }

        [Fact]
        public void SingleKeywordSearchDoesLevenshteinAndAbsoluteSearch()
        {
            TestQuery.Keyword = "Fuzzy";

            QueryBuilder.MaxLevenshteinDistance = 0.3;
            var parsed = AssertValidQuery(TestQuery);
            Assert.Equal("(Fuzzy OR Fuzzy~0.3)", parsed["q"]);
        }

        [Fact]
        public void KeywordSearchUsesLevenshteinParameter()
        {
            TestQuery.Keyword = "Param";

            QueryBuilder.MaxLevenshteinDistance = 0.6;
            var parsed = AssertValidQuery(TestQuery);
            Assert.Equal(3, parsed.Count);
            Assert.Equal("(Param OR Param~0.6)", parsed["q"]);

            QueryBuilder.MaxLevenshteinDistance = 0.9;
            parsed = AssertValidQuery(TestQuery);
            Assert.Equal("(Param OR Param~0.9)", parsed["q"]);
        }

        [Fact]
        public void MultipleKeywordSearchDoesLevenshteinAndAbsoluteSearch()
        {
            TestQuery.Keyword = "Fuzzy Was";

            var parsed = AssertValidQuery(TestQuery);
            Assert.Equal(3, parsed.Count);
            QueryBuilder.MaxLevenshteinDistance = 0.3;

            Assert.Equal("(Fuzzy Was OR ((Fuzzy OR Fuzzy~0.3) AND (Was OR Was~0.3)))", 
                parsed["q"]);
        }

        [Fact]
        public void MultipleKeywordsAreBrokenUpOnUrlEncodedWhiteSpace()
        {
            TestQuery.Keyword = "Navy%20Academy";

            var parsed = AssertValidQuery(TestQuery);
            Assert.Equal(3, parsed.Count);
            QueryBuilder.MaxLevenshteinDistance = 0.3;

            Assert.Equal("(Navy Academy OR ((Navy OR Navy~0.3) AND (Academy OR Academy~0.3)))",
                parsed["q"]);
        }

        [Fact]
        public void ColonsAreEncodedToNotBreakUrl()
        {
            TestQuery.Keyword = "";
            TestQuery.BooleanQuery = new BooleanQuery
            {
                Conditions = new List<IBooleanCondition>
                {
                    new IntBooleanCondition("ThriftyThree", 33),
                    new StringBooleanCondition("FourthyFour", "Number")
                }
            };

            string rawQuery = QueryBuilder.BuildSearchQuery(TestQuery);

            Match query = Regex.Match(rawQuery, "q=%28([^&]*)%29");

            Assert.Equal("%28FourthyFour%3A%27Number%27%29%20AND%20%28ThriftyThree%3A33%29", query.Groups[1].Value);
        }

        [Fact]
        public void ForwardSlashesAreEncodedToNotBreakUrl()
        {
            TestQuery.Keyword = "Model/Template";

            string rawQuery = QueryBuilder.BuildSearchQuery(TestQuery);

            Match query = Regex.Match(rawQuery, "q=%28([^&]*)%29");

            Assert.Equal("Model%2FTemplate%20OR%20Model%2FTemplate~0.3", query.Groups[1].Value);
        }

        [Fact]
        public void KeywordSearchUsesLuceneParser()
        {
            TestQuery.Keyword = "Fuzzy";
            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("lucene", parsed["q.parser"]);
        }

        [Fact]
        public void AddingEmptyReturnFieldsAddsNoReturnParameter()
        {
            TestQuery.Fields.Clear();
            var parsed = AssertValidQuery(TestQuery);

            Assert.False(parsed.AllKeys.ToList().Contains("return"));
        }

        [Fact]
        public void AddingSingleReturnFieldAddsSingleParameter()
        {
            TestQuery.Fields.Clear();
            TestQuery.Fields.Add("orange");

            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("orange", parsed["return"]);            
        }

        [Fact]
        public void AddingNullSortFieldAddsNoSortParameter()
        {
            TestQuery.SortField = null;

            var parsed = AssertValidQuery(TestQuery);

            Assert.False(parsed.AllKeys.ToList().Contains("sort"));
        }

        [Fact]
        public void AddingEmptySortFieldAddsNoSortParameter()
        {
            TestQuery.SortField = "";

            var parsed = AssertValidQuery(TestQuery);

            Assert.False(parsed.AllKeys.ToList().Contains("sort"));
        }

        [Fact]
        public void DefaultSortOrderIsDescending()
        {
            TestQuery.SortField = "x";

            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("x desc", parsed["sort"]);           
        }

        [Fact]
        public void AddingSortFieldAddsSortParameter()
        {
            TestQuery.SortField = "grapefruit";

            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("grapefruit desc", parsed["sort"]);
        }

        [Fact]
        public void SettingAscendingParameterChangesSortToAscending()
        {
            TestQuery.ShouldSortAscending = true;
            TestQuery.SortField = "pineapple";

            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("pineapple asc", parsed["sort"]);
        }

        [Fact]
        public void AddingReturnFieldsAddsCommaSeparatedListParameter()
        {
            TestQuery.Fields.Clear();
            TestQuery.Fields.Add("orange");
            TestQuery.Fields.Add("banana");
            TestQuery.Fields.Add("gumdrops");

            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal("orange,banana,gumdrops", parsed["return"]);

            TestQuery.Fields.Clear();
            TestQuery.Fields.Add("cherry");
            TestQuery.Fields.Add("banana");

            parsed = AssertValidQuery(TestQuery);

            Assert.Equal("cherry,banana", parsed["return"]);
        }

        [Fact]
        public void EmptyQueryReturnsDefaultSearchDocumentFieldQuery()
        {
            var parsed = AssertValidQuery(TestQuery);

            Assert.Equal(1, parsed.Count);
            Assert.Equal("id", parsed["return"]);
        }

        [Fact]
        public void AddingStartResultAddsStartQueryParameter()
        {
            TestQuery.Start = 30;
            var parsed = AssertValidQuery(TestQuery);
            Assert.Equal("30", parsed["start"]);

            TestQuery.Start = 0;
            parsed = AssertValidQuery(TestQuery);
            Assert.Equal("0", parsed["start"]);
        }

        [Fact]
        public void NotAddingStartResultDoesNotAddQueryParameter()
        {
            TestQuery.Start = null;
            var parsed = AssertValidQuery(TestQuery);
            Assert.False(parsed.AllKeys.ToList().Contains("start"));
        }

        [Fact]
        public void AddingNegativeStartResultThrowsException()
        {
            TestQuery.Start = -30;
            Assert.Throws<InvalidOperationException>(() =>
                QueryBuilder.BuildSearchQuery(TestQuery));

        }

        [Fact]
        public void AddingSizeAddsStartQueryParameter()
        {
            TestQuery.Size = 30;
            var parsed = AssertValidQuery(TestQuery);
            Assert.Equal("30", parsed["size"]);
        }

        [Fact]
        public void NotAddingSizeDoesNotAddQueryParameter()
        {
            TestQuery.Size = null;
            var parsed = AssertValidQuery(TestQuery);
            Assert.False(parsed.AllKeys.ToList().Contains("size"));
        }

        [Fact]
        public void AddingInvalidSizeThrowsException()
        {
            TestQuery.Size = -30;
            Assert.Throws<InvalidOperationException>(() =>
                QueryBuilder.BuildSearchQuery(TestQuery));
            TestQuery.Size = 0;
            Assert.Throws<InvalidOperationException>(() =>
                QueryBuilder.BuildSearchQuery(TestQuery));
        }

        #region HelperMethods
        private NameValueCollection AssertValidQuery(SearchQuery<SearchDocument> query)
        {
            string queryString = QueryBuilder.BuildSearchQuery(query);

            // API rule is query string can be empty, but not null
            Assert.NotEqual(null, queryString);
            if (queryString != String.Empty)
            {
                var parsed = GetQueryCollection(queryString);
                return parsed;
            }

            return new NameValueCollection();
        }

        private static NameValueCollection GetQueryCollection(string queryString)
        {
            var splitString = SplitUri(queryString);

            NameValueCollection parsed = new NameValueCollection();
            if (splitString.Count > 1)
            {
                parsed = HttpUtility.ParseQueryString(splitString[1]);
            }
            return parsed;
        }

        private static List<string> SplitUri(string queryString)
        {
            var splitString = queryString.Split('?').ToList();
            Assert.Equal(BaseUri, splitString[0]);
            return splitString;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool final)
        {
        }
        #endregion
    }
}
