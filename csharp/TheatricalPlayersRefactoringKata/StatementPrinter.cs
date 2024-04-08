using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        private int CalculatePlayVolumeCredits(Performance perf, Play play)
        {
            // add volume credits
            var volumeCredits = Math.Max(perf.Audience - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == play.Type)
            {
                volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);
            }

            return volumeCredits;
        }

        private int CalculatePlayAmount(Performance perf, Play play)
        {
            var amount = 0;
            switch (play.Type) 
            {
                case "tragedy":
                    amount = 40000;
                    if (perf.Audience > 30) {
                        amount += 1000 * (perf.Audience - 30);
                    }
                    break;
                case "comedy":
                    amount = 30000;
                    if (perf.Audience > 20) {
                        amount += 10000 + 500 * (perf.Audience - 20);
                    }
                    amount += 300 * perf.Audience;
                    break;
                default:
                    throw new Exception("unknown type: " + play.Type);
            }

            return amount;
        }
        
        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            foreach(var perf in invoice.Performances) 
            {
                var play = plays[perf.PlayID];
                var amount = CalculatePlayAmount(perf, play);

                // add volume credits
                volumeCredits += CalculatePlayVolumeCredits(perf, play);

                // print line for this order
                result += String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(amount / 100), perf.Audience);
                totalAmount += amount;
            }
            result += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += String.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }

        public string PrintAsHTML(Invoice invoice, Dictionary<string, Play> plays)
        {
            return "";
        }
    }
}
