using System.Reflection;
using DataProvider.Umtb;
using FluentAssertions;
using NUnit.Framework;

namespace DataProvider.Tests.Umtb
{
    [TestFixture]
    public class UmtbTransactionsExtractorTests
    {
        [Test]
        public void ExtractTransactions_StreamWithSomeTransactions_Extracted()
        {
            var umtbTransactionsExtractor = new UmtbTransactionsExtractor();

            var activityStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DataProvider.Tests.Umtb.AccountActivity.xls");
            var bankTransactions = umtbTransactionsExtractor.ExtractTransactions(activityStream);

            bankTransactions.Should().HaveCount(5);
        }        
    }
}