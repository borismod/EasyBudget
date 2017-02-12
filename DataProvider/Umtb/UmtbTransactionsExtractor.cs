using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DataProvider.Interfaces;
using DataProvider.Model;
using HtmlAgilityPack;

namespace DataProvider.Umtb
{
    public class UmtbTransactionsExtractor
    {
        public List<BankTransaction> ExtractTransactions(Stream stream)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(stream);
            double lastBallance = 0;
            return htmlDocument.DocumentNode.SelectNodes(@"/html/body/table[2]/tr")
                .Select(item => ExtractBankTransaction(item, ref lastBallance))
                .Where(x => x != null)
                .ToList();
        }

        private static BankTransaction ExtractBankTransaction(HtmlNode item, ref double lastBallance)
        {
            var date = item.ChildNodes[1].InnerText;
            DateTime eventDate;
            if (!TryParseExactDate(date, out eventDate))
            {
                return null;
            }
            var eventDescription = item.ChildNodes[5].InnerText;
            var incomeAmount = item.ChildNodes[7].InnerText;
            var expenseAmount = item.ChildNodes[9].InnerText;
            string amount;
            TransactionType transactonType;
            if (string.IsNullOrEmpty(incomeAmount))
            {
                amount = expenseAmount;
                transactonType = TransactionType.Expense;
            }
            else
            {
                amount = incomeAmount;
                transactonType = TransactionType.Income;
            }
            var eventAmount = double.Parse(amount);
            var ballanceString = item.ChildNodes[11].InnerText;
            double currentBalance;
            if (string.IsNullOrEmpty(ballanceString))
            {
                if (transactonType == TransactionType.Income)
                {
                    currentBalance = lastBallance + eventAmount;
                }
                else
                {
                    currentBalance = lastBallance - eventAmount;
                }
            }
            else
            {
                currentBalance = double.Parse(ballanceString);
            }
            lastBallance = currentBalance;
            var eventId = long.Parse(item.ChildNodes[13].InnerText);

            var bankTransaction = new BankTransaction
            {
                EventDate = eventDate,
                EventDescription = eventDescription,
                EventAmount = eventAmount, 
                CurrentBalance = currentBalance,
                EventId = eventId,
                Type = transactonType
            };
            return bankTransaction;
        }

        private static bool TryParseExactDate(string date, out DateTime eventDate)
        {
            return DateTime.TryParseExact(date, @"dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces,
                out eventDate);
        }
    }
}