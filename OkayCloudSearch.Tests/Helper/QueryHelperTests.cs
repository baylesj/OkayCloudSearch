using System;
using OkayCloudSearch.Helper;
using Xunit;

namespace OkayCloudSearch.Tests.Helper
{
    public class QueryHelperTests
    {
        private const string ValidHostname = "test.com";

        [Fact]
        public void NullBaseStringThrowsException()
        {
            Assert.Throws(typeof (ArgumentNullException), () => new QueryHelper(null));
        }

        [Fact]
        public void EmptyStringReturnsEmpty()
        {
            var result = new QueryHelper(String.Empty);

            Assert.Equal(String.Empty, result.ToString());
        }

        [Fact]
        public void ProperConstructionReturnsBaseString()
        {
            var result = new QueryHelper(ValidHostname);

            Assert.Equal(ValidHostname, result.ToString());
        }

        [Fact]
        public void AddingNullFieldThrowsException()
        {
            var result = new QueryHelper(ValidHostname);

            Assert.Throws(typeof (ArgumentOutOfRangeException), () =>
            {
                result.AppendField(null, "Anything");
            });
        }

        [Fact]
        public void AddingNullFieldValueAddsEmptyField()
        {
            var helper = new QueryHelper(ValidHostname);
            helper.AppendField("name", null);

            Assert.Equal("test.com?name=", helper.ToString());
        }


        [Fact]
        public void AddingEmptyFieldValueAddsEmptyField()
        {
            var helper = new QueryHelper(ValidHostname);
            helper.AppendField("name", String.Empty);

            Assert.Equal("test.com?name=", helper.ToString());
        }

        [Fact]
        public void AddingValidFieldValueAddsField()
        {
            var helper = new QueryHelper(ValidHostname);
            helper.AppendField("name", "indiana-jones");

            Assert.Equal("test.com?name=indiana-jones", helper.ToString());
        }

        [Fact]
        public void MultipleFieldsAreSeparated()
        {
            var helper = new QueryHelper(ValidHostname);
            helper.AppendField("name", "indiana-jones");
            helper.AppendField("occupation", "archaeologist");

            Assert.Equal("test.com?name=indiana-jones&occupation=archaeologist",
                helper.ToString());

        }
    }
}
