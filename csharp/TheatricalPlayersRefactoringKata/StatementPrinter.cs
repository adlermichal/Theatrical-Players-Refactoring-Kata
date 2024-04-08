using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        private int CalculatePlayVolumeCredits(int perfAudience, string playType)
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

        public (int, int) CalculatePlay(int perfAudience, string playType)
        {
            var amount = CalculatePlayAmount(perfAudience, playType);

            // add volume credits
            var volumeCredits = CalculatePlayVolumeCredits(perfAudience, playType);

            return (amount, volumeCredits);

        }

        private int CalculatePlayAmount(int perfAudience, string playType)
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
            public int TotalAmount { get; set; }
            public int VolumeCredits { get; set; }
            public Dictionary<Play, int> PlayAmounts { get; set; }
        }

        public PerformanceSummary CalculatePerformance(Invoice invoice, Dictionary<string, Play> plays)
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

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            var totalAmount = 0;
            var volumeCredits = 0;
            var summary = CalculatePerformance(invoice, plays);
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var (amount, playVolumeCredits) = CalculatePlay(perf.Audience, play.Type);
                volumeCredits += playVolumeCredits;
                // print line for this order
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(amount / 100), perf.Audience);
                totalAmount += amount;
            }

            var newResult = string.Format("Statement for {0}\n", invoice.Customer);

            foreach (var play in summary.PlayAmounts)
            {
                var amount = Convert.ToDecimal(play.Value / 100);
                newResult += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Key.Name, Convert.ToDecimal(amount / 100));

            }
            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(summary.TotalAmount / 100));
            result += String.Format("You earned {0} credits\n", summary.VolumeCredits);
            return result;
        }
    }
}
