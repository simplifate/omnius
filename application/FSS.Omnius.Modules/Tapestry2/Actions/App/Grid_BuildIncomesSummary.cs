using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3006, "Build income summary Action")]
        public static void BuildIncomesSummary(COREobject core, string WalletSummaryTableName = "TransactionWalletSummary", string HistoryTableName = "TransactionHistory", bool Rebuild = false)
        {
            var db = core.Entitron;
            
            if (Rebuild)
            {
                db.TableTruncate(WalletSummaryTableName);
                db.SaveChanges();
            }

            var walletSummaryTable = db.Table(WalletSummaryTableName, false);
            var historyTable = db.Table(HistoryTableName, false);
            var walletsView = db.Tabloid("DistinctWallets");

            List<DBItem> walletList = walletsView.Select().ToList();
            foreach (DBItem wallet in walletList)
            {
                string address = (string)wallet["Address"];

                foreach (string type in new string[] { "income", "outcome" })
                {
                    DBItem lastRecord = null;
                    int offset = 0;

                    IEnumerable<DBItem> allRecords = walletSummaryTable.Select()
                                                                        .Where(s =>
                                                                            s.Column("Wallet_address").Equal(address).And
                                                                             .Column("Transaction_type").Equal(type))
                                                                        .Order(AscDesc.Desc, "Date")
                                                                        .ToList();

                    while (lastRecord == null)
                    {
                        IEnumerable<DBItem> records = allRecords.Skip(offset).Take(50);
                        if (records.Count() == 0)
                            break;

                        foreach (DBItem record in records)
                        {
                            if ((double)record["Diff_in_currency"] == 0)
                            {
                                walletSummaryTable.Delete(record);
                                continue;
                            }

                            lastRecord = record;
                            break;
                        }
                        offset += 50;
                    }

                    DateTime lastRecordDate = lastRecord == null ? new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) : (DateTime)lastRecord["Date"];

                    var select = historyTable.Select().Where(h =>
                            h.Column("Wallet_Address").Equal(address).And
                            .Column("Transaction_date").Greater(lastRecordDate).And
                            .Column("Transaction_type").Equal(type)
                        ).Order(columnNames: "Transaction_date").ToList();

                    // Pokud nemáme data ani v jedné tabulce, nemůžeme pokračovat
                    if (select.Count == 0 && lastRecord == null)
                    {
                        continue;
                    }

                    DateTime now = FloorDateTime(DateTime.UtcNow, new TimeSpan(1, 0, 0));
                    DateTime timeFrom = lastRecord == null ? FloorDateTime((DateTime)(select[0]["Transaction_date"]), new TimeSpan(1, 0, 0)) : lastRecordDate;
                    DateTime timeTo = timeFrom.AddHours(1);

                    DBItem lastWalletRecord = walletSummaryTable.Select().Where(s => s.Column("Wallet_address").Equal(address).And.Column("Transaction_type").Equal(type)).Order(AscDesc.Desc, "Date").FirstOrDefault();
                    WalletSummaryItem sum = new WalletSummaryItem()
                    {
                        TotalInCurrency = lastWalletRecord == null ? 0 : (double)lastWalletRecord["Total_in_currency"],
                        TotalInUSD = lastWalletRecord == null ? 0 : (double)lastWalletRecord["Total_in_usd"],
                        DiffInCurrency = lastWalletRecord == null ? 0 : (double)lastWalletRecord["Diff_in_currency"],
                        DiffInUSD = lastWalletRecord == null ? 0 : (double)lastWalletRecord["Diff_in_usd"],
                        Type = type
                    };


                    while (timeFrom <= now)
                    {
                        sum.DiffInCurrency = 0;
                        sum.DiffInUSD = 0;

                        var rows = select.Where(h =>
                            (DateTime)h["Transaction_date"] >= timeFrom &&
                            (DateTime)h["Transaction_date"] < timeTo &&
                            (string)h["Transaction_type"] == type);

                        if (rows.Count() > 0)
                        {
                            foreach (DBItem row in rows)
                            {
                                sum.TotalInCurrency += (double)row["Amount_in_currency"];
                                sum.TotalInUSD += (double)row["Amount_in_usd"];
                                sum.DiffInCurrency += (double)row["Amount_in_currency"];
                                sum.DiffInUSD += (double)row["Amount_in_usd"];
                            }
                        }

                        DBItem item = new DBItem(db, walletSummaryTable);
                        item["Date"] = timeTo;
                        item["Currency_code"] = (string)wallet["Currency_code"];
                        item["Total_in_currency"] = sum.TotalInCurrency;
                        item["Total_in_usd"] = sum.TotalInUSD;
                        item["Diff_in_currency"] = sum.DiffInCurrency;
                        item["Diff_in_usd"] = sum.DiffInUSD;
                        item["Wallet_address"] = address;
                        item["Transaction_type"] = sum.Type;

                        walletSummaryTable.Add(item);

                        timeFrom = timeTo;
                        timeTo = timeFrom.AddHours(1);
                    }
                }
            }

            db.SaveChanges();
        }
        private static DateTime FloorDateTime(DateTime dateTime, TimeSpan interval)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
        }
        class WalletSummaryItem
        {
            public double TotalInCurrency;
            public double TotalInUSD;
            public double DiffInCurrency;
            public double DiffInUSD;
            public string Type;
        }
    }
}
