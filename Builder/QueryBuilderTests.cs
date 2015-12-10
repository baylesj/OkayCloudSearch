using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using OkayCloudSearch.Contract;
using OkayCloudSearch.Query;
using OkayCloudSearch.Query.Boolean;
using Xunit;

namespace OkayCloudSearch.Builder
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

            Assert.Equal("((Fuzzy OR Fuzzy~0.3) AND (Was OR Was~0.3))", 
                parsed["q"]);
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
            var parsed = GetQueryCollection(queryString);
            return parsed;
        }

        private static NameValueCollection GetQueryCollection(string queryString)
        {
            var splitString = SplitUri(queryString);

            NameValueCollection parsed = HttpUtility.ParseQueryString(splitString[1]);
            return parsed;
        }

        private static List<string> SplitUri(string queryString)
        {
            var splitString = queryString.Split('?').ToList();
            Assert.Equal(2, splitString.Count);
            Assert.Equal(BaseUri, splitString[0]);
            return splitString;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
