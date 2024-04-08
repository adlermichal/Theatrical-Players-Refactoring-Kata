using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        private static int CalculatePlayVolumeCredits(int perfAudience, string playType)
        {
            // add volume credits
            var volumeCredits = Math.Max(perfAudience - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == playType)
            {
                volumeCredits += (int)Math.Floor((decimal)perfAudience / 5);
            }

            return volumeCredits;
        }

        private (int, int) CalculatePlay(int perfAudience, string playType)
        {
            var amount = CalculatePlayAmount(perfAudience, playType);

            // add volume credits
            var volumeCredits = CalculatePlayVolumeCredits(perfAudience, playType);

            return (amount, volumeCredits);

        }

        private static int CalculatePlayAmount(int perfAudience, string playType)
        {
            var amount = 0;
            switch (playType)
            {
                case "tragedy":
                    amount = 40000;
                    if (perfAudience > 30)
                    {
                        amount += 1000 * (perfAudience - 30);
                    }
                    break;
                case "comedy":
                    amount = 30000;
                    if (perfAudience > 20)
                    {
                        amount += 10000 + 500 * (perfAudience - 20);
                    }
                    amount += 300 * perfAudience;
                    break;
                default:
                    throw new Exception("unknown type: " + playType);
            }

            return amount;
        }

        public class PerformanceSummary
        {
            public int TotalAmount { get; init; }
            public int VolumeCredits { get; init; }
            public Dictionary<Play, int> PlayAmounts { get; init; }
        }

        private PerformanceSummary CalculatePerformance(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var playDict = new Dictionary<Play, int>();
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var (amount, playVolumeCredits) = CalculatePlay(perf.Audience, play.Type);
                volumeCredits += playVolumeCredits;
                totalAmount += amount;
                playDict.Add(play, amount);
            }

            return new PerformanceSummary
            {
                TotalAmount = totalAmount,
                VolumeCredits = volumeCredits,
                PlayAmounts = playDict
            };
        }

        private static decimal GetCurrencyValue(int amount)
        {
            return Convert.ToDecimal(amount / 100);
        }

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            var summary = CalculatePerformance(invoice, plays);

            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var playAmount = summary.PlayAmounts.GetValueOrDefault(play);
                // print line for this order
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, GetCurrencyValue(playAmount), perf.Audience);
            }

            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", GetCurrencyValue(summary.TotalAmount));
            result += String.Format("You earned {0} credits\n", summary.VolumeCredits);
            return result;
        }

        public string PrintHtml(Invoice invoice, Dictionary<string, Play> plays)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<body>");
            sb.AppendLine(string.Format("<h1>Statement for {0}</h1>", invoice.Customer));
            CultureInfo cultureInfo = new CultureInfo("en-US");

            var summary = CalculatePerformance(invoice, plays);

            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>play</th><th>seats</th><th>cost</th></tr>");
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var playAmount = summary.PlayAmounts.GetValueOrDefault(play);
                // print line for this order
                sb.AppendLine(String.Format(cultureInfo, "<tr><td>{0}</td><td>{2}</td><td>{1:C}</td></tr>", play.Name, GetCurrencyValue(playAmount), perf.Audience));
            }
            sb.AppendLine("</table>");

            sb.AppendLine(String.Format(cultureInfo, "<p>Amount owed is <em>{0:C}</em></p>", GetCurrencyValue(summary.TotalAmount)));
            sb.AppendLine(String.Format("<p>You earned <em>{0}</em> credits</p>", summary.VolumeCredits));
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }
}
