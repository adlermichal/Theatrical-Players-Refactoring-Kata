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



        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            var totalAmount = 0;
            var volumeCredits = 0;
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var (amount, playVolumeCredits) = CalculatePlay(perf.Audience, play.Type);
                volumeCredits += playVolumeCredits;
                // print line for this order
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(amount / 100), perf.Audience);
                totalAmount += amount;
            }
            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += String.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }
    }
}
