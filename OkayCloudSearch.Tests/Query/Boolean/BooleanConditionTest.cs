using OkayCloudSearch.Query.Boolean;
using Xunit;

namespace OkayCloudSearch.Tests.Query.Boolean
{
    public class BooleanConditionTest
    {
        private class AbstractTest : BooleanCondition
        {
            public override string GetQueryString()
            {
                throw new System.NotImplementedException();
            }

            public override bool IsOrCondition { get; set; }
            public override bool IsList()
            {
                throw new System.NotImplementedException();
            }

            public string EncodeTest(string s)
            {
                return EncodeCondition(s);
            }
        }

        [Fact]
        public void SpacesInFacetNamesAreEncodedAsSpaces()
        {
            AbstractTest condition = new AbstractTest();
            string result = condition.EncodeTest("James Bond");

            Assert.Equal("\"James+Bond\"", result);
        }
    }
}
