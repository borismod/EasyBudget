using System;
using System.Collections.Generic;
using DataProvider.Umtb;
using EasyBudgetApp.Models.Credentials;
using FluentAssertions;
using NUnit.Framework;

namespace EasyBudgetApp.Tests
{
    [TestFixture]
    public class UmtbDataProviderTests
    {
        [Test]
        public void MyMethod()
        {
            var umtbDataProvider = new UmtbDataProvider(new ProviderDescriptor("", new Dictionary<string, string> { { "username", "4556064022"}, {"password", "9fT8UPObVgDx" } }, null));
            var transactions = umtbDataProvider.GetTransactions(1,DateTime.UtcNow, DateTime.UtcNow);

            transactions.Should().BeNull();
        }    
    }

}