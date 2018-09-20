using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class FixTransactionHistoryAction : Action
    {
        private DBConnection _db;

        public override int Id
        {
            get
            {
                return 300121;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {

                };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid:FixTransactionHistoryAction";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;
            _db = core.Entitron;

            try
            {
                DBTable transactionHistoryTable = _db.Table("TransactionHistory", false); //trans. history table
                DBTable transactionWalletSummaryTable = _db.Table("TransactionWalletSummary", false); //trans.w summary table

                Tabloid fme = _db.Tabloid("FixMissingExchangeRate", false);
                List<DBItem> fixedMissingExr = fme.Select().ToList();

                Dictionary<string, DateTime> firstErrorByWallet = new Dictionary<string, DateTime>();
                foreach (var f in fixedMissingExr)
                {
                    if (f["newVal"] is System.DBNull) //if Conversion table found nothing
                        continue;
                    if (!firstErrorByWallet.ContainsKey(f["Wallet_Address"].ToString()))
                    {
                        firstErrorByWallet.Add(f["Wallet_Address"].ToString(), (DateTime)f["Transaction_date"]);
                    }
                    else {
                        if ((DateTime)f["Transaction_date"] < firstErrorByWallet[f["Wallet_Address"].ToString()])
                        {
                            firstErrorByWallet[f["Wallet_Address"].ToString()] = (DateTime)f["Transaction_date"];
                        }
                    }
                    //var rowToUpdate = thList.SingleOrDefault(r => r["id"].ToString() == f["id"].ToString());
                    DBItem rowToUpdate = transactionHistoryTable.Select().Where(c => c.Column("id").Equal(f["id"])).First();

                    rowToUpdate["Exchange_rate"] = (double)f["newVal"];
                    rowToUpdate["Amount_in_usd"] = (double)f["newVal"] * (double)rowToUpdate["Amount_in_currency"]; //now we have new fixed data
                    rowToUpdate["ToBeFixed"] = 0; //don't fix again if it has been fixed now
                    transactionHistoryTable.Update(rowToUpdate, (int)f["id"]);
                }

               

                foreach (var d in firstErrorByWallet) {
                    var result = transactionWalletSummaryTable.Select().Where(x => x.Column("Wallet_address").Equal(d.Key).And.Column("Date").GreaterOrEqual(d.Value)).ToList();
                    foreach (var r in result) {
                        transactionWalletSummaryTable.Delete((int)r["id"]);
                    }
                }

                _db.SaveChanges();
            }
            catch (Exception)
            {
            }

        }
        private double GetExchangeRate(DateTime txDate, string walletCurrency, COREobject core)
        {
            var totalSeconds = convertDateTimeToTimeStamp(txDate);
            DBTable miningHistoryTable = _db.Table("MiningHistory", false); //mining. history table
            var rowWithSimilarTimeStamp = miningHistoryTable.Select().Where(x => x.Column("Time_stamp").GreaterOrEqual(txDate.AddMinutes(-1000)).And.Column("Time_stamp").LessOrEqual(txDate.AddMinutes(10000))).Order(columnNames: "Time_stamp").FirstOrDefault();
            double exR = 0;
            if (rowWithSimilarTimeStamp == null)
            {
                return exR;
            }
            else {

                switch (walletCurrency)
                {
                    case "ETH":
                        exR = (double)rowWithSimilarTimeStamp["Conversion_ETH"];
                        break;
                    case "BTC":
                        exR = (double)rowWithSimilarTimeStamp["Conversion_BTC"];
                        break;
                    case "ZEC":
                        exR = (double)rowWithSimilarTimeStamp["Conversion_ZEC"];
                        break;
                    case "DCR":
                        exR = (double)rowWithSimilarTimeStamp["Conversion_DCR"];
                        break;
                    case "LTC":
                        exR = (double)rowWithSimilarTimeStamp["Conversion_LTC"];
                        break;

                }
                return exR;
            }
        }
        private double convertDateTimeToTimeStamp(DateTime dt)
        {
            DateTime startDateTime = new DateTime(1970, 1, 9, 0, 0, 00);
            TimeSpan result;
            result = dt - startDateTime;
            double totalSeconds = result.TotalSeconds;
            return totalSeconds;
        }
    }
}
