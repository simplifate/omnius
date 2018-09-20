using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3004, "Grid: Generate income chart data", "Result")]
        public static string GenerateIncomeChartData(COREobject core, Dictionary<int, double> IncomeByDay)
        {
            DBConnection db = core.Entitron;
            
            StringBuilder csvStringBuilder = new StringBuilder("Balance_total,Balance_predict,dateTime\\n");
            var currentDayStart = DateTime.Now.Date;
            int dayOfTheWeek = (int)currentDayStart.DayOfWeek;
            dayOfTheWeek = dayOfTheWeek == 0 ? 6 : dayOfTheWeek - 1;
            var mondayStart = currentDayStart.AddDays(-dayOfTheWeek);

            Dictionary<int, double> realProfit = new Dictionary<int, double>();
            NumberFormatInfo withDecimalPoint = new NumberFormatInfo() { NumberDecimalSeparator = "." };

            double cumulativeReal = 0;

            for (int i = 1; i < 7; i++)
            {
                if (i > dayOfTheWeek)
                {
                    realProfit.Add(i, 0);
                }
                else
                {
                    int dayIndex = i + 1;
                    cumulativeReal += IncomeByDay.ContainsKey(dayIndex) ? IncomeByDay[dayIndex] : 0;
                    realProfit.Add(i, cumulativeReal);
                }
            }

            DBTable table = db.Table("MiningHistory", false);
            var historyRows = table.Select().Where(r => r.Column("Time_stamp").GreaterOrEqual(mondayStart)).ToList();

            double futurePrediction = (double)historyRows.OrderByDescending(r => (int)r["id"]).First()["Profit"];
            // double futurePrediction = 100 / 6; // Testing

            double cumulativePrediction = 0;

            int previousDay = -1;
            double real = 0;

            previousDay = mondayStart.Day;
            foreach (var row in historyRows)
            {
                var rowTime = ((DateTime)row["Time_stamp"]);
                cumulativePrediction += System.Math.Round(((double)row["Profit"]) / 6);
                int rowDayOfTheWeek = (int)rowTime.DayOfWeek;
                rowDayOfTheWeek = rowDayOfTheWeek == 0 ? 6 : rowDayOfTheWeek - 1;
                if(rowTime.Day!= previousDay)
                {
                    real = System.Math.Round(realProfit[rowDayOfTheWeek]);
                    previousDay = rowTime.Day;
                }
                else
                {
                    real = 0;
                }
                csvStringBuilder.Append($"{real.ToString(withDecimalPoint)},{cumulativePrediction.ToString(withDecimalPoint)},{rowTime.ToString("yyyy-MM-dd H:mm:ss")}\\n");
            }

            var weekEnd = mondayStart.AddDays(7);
            double minutesTillEndOfWeek = (weekEnd - DateTime.Now).TotalMinutes;
            double endPrediction = cumulativePrediction + System.Math.Round(futurePrediction * (minutesTillEndOfWeek / 10));
            csvStringBuilder.Append($"{0},{endPrediction.ToString(withDecimalPoint)},{weekEnd.ToString("yyyy-MM-dd H:mm:ss")}\\n");

            return csvStringBuilder.ToString();
        }
    }
}
