using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GenerateAuctionEvaluationsAction : Action
    {
        public override int Id => 192;

        public override string[] InputVar => new string[] { "AuctionId", "CurrentDemands", "?DiminishingMode", "?PreviousDemands" };

        public override string Name => "Specific: Generate auction evaluations";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;
            DBTable evaluationTable = db.Table("auctions_evaluations");
            //var demandsView = core.Entitron.GetDynamicView("DemandsVsCapacity");

            bool diminishingMode = vars.ContainsKey("DiminishingMode") && (bool)vars["DiminishingMode"] == true;
            int auctionId = (int)vars["AuctionId"];
            var currentDemands = (List<DBItem>)vars["CurrentDemands"];

            if (currentDemands.Count == 0)
                return;

            DateTime timestamp = DateTime.Now;
            int userId = core.User.Id;

            int totalCapacity = Convert.ToInt32(currentDemands[0]["Capacity"]);
            int currentRoundNumber = (int)currentDemands[0]["RoundNumber"];
            double remainingCapacity = totalCapacity;
            double sumOfCurrentDemands = currentDemands.Sum(d => Convert.ToDouble(d["DemandAbsolute"]));

            if (!diminishingMode)
            {
                // RWE subjekty
                if (currentRoundNumber == 1 && Convert.ToBoolean(currentDemands[0]["is_annual"]) && currentDemands.Any(d => Convert.ToBoolean(d["is_RWE_group"])))
                {
                    // split RWE & non RWE
                    List<DBItem> rweDemands = new List<DBItem>();
                    List<DBItem> nonRweDemands = new List<DBItem>();
                    foreach (DBItem demand in currentDemands)
                    {
                        if (Convert.ToBoolean(demand["is_RWE_group"]))
                            rweDemands.Add(demand);
                        else
                            nonRweDemands.Add(demand);
                    }

                    // non RWE demands
                    foreach (DBItem demand in nonRweDemands)
                    {
                        double demandAbsolute = Convert.ToDouble(demand["DemandAbsolute"]);
                        double demandFraction = Convert.ToDouble(demand["DemandFraction"]);
                        int orgId = (int)demand["OrgId"];
                        int period = (int)demand["Period"];
                        double roundPrice = Convert.ToDouble(demand["RoundPrice"]);
                        remainingCapacity -= demandAbsolute;

                        var newEvaluation = new DBItem(db, evaluationTable);
                        newEvaluation["id_auction"] = auctionId;
                        newEvaluation["id_organization"] = orgId;

                        newEvaluation["assigned_capacity_absolute"] = demandAbsolute;
                        newEvaluation["assigned_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                        newEvaluation["demanded_capacity_absolute"] = demandAbsolute;
                        newEvaluation["demanded_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                        newEvaluation["unsatisfied_capacity_absolute"] = 0;
                        newEvaluation["unsatisfied_capacity_percentage"] = 0;
                        newEvaluation["supplemented_capacity_absolute"] = 0;
                        newEvaluation["supplemented_capacity_percentage"] = 0;

                        newEvaluation["period"] = period;
                        newEvaluation["unit_price"] = roundPrice;
                        newEvaluation["total_price"] = roundPrice * demandAbsolute;

                        newEvaluation["id_user_insert"] = userId;
                        newEvaluation["id_user_change"] = userId;
                        newEvaluation["datetime_insert"] = timestamp;
                        newEvaluation["datetime_change"] = timestamp;

                        evaluationTable.Add(newEvaluation);
                    }

                    // RWE demands - split remaining
                    double rweTotalDemandCapacity = sumOfCurrentDemands - totalCapacity + remainingCapacity;
                    double ratio = remainingCapacity / rweTotalDemandCapacity;
                    foreach (DBItem demand in rweDemands)
                    {
                        double demandAbsolute = Convert.ToDouble(demand["DemandAbsolute"]);
                        double demandFraction = Convert.ToDouble(demand["DemandFraction"]);
                        double assignedAbsolute = System.Math.Round(ratio * demandAbsolute, 3);
                        double assignedPercentage = System.Math.Round((assignedAbsolute / totalCapacity) * 100, 2);
                        int orgId = (int)demand["OrgId"];
                        int period = (int)demand["Period"];
                        double roundPrice = Convert.ToDouble(demand["RoundPrice"]);

                        var newEvaluation = new DBItem(db, evaluationTable);
                        newEvaluation["id_auction"] = auctionId;
                        newEvaluation["id_organization"] = orgId;

                        newEvaluation["assigned_capacity_absolute"] = assignedAbsolute;
                        newEvaluation["assigned_capacity_percentage"] = assignedPercentage;
                        newEvaluation["demanded_capacity_absolute"] = demandAbsolute;
                        newEvaluation["demanded_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                        newEvaluation["unsatisfied_capacity_absolute"] = 0;
                        newEvaluation["unsatisfied_capacity_percentage"] = 0;
                        newEvaluation["supplemented_capacity_absolute"] = 0;
                        newEvaluation["supplemented_capacity_percentage"] = 0;

                        newEvaluation["period"] = period;
                        newEvaluation["unit_price"] = roundPrice;
                        newEvaluation["total_price"] = roundPrice * assignedAbsolute;

                        newEvaluation["id_user_insert"] = userId;
                        newEvaluation["id_user_change"] = userId;
                        newEvaluation["datetime_insert"] = timestamp;
                        newEvaluation["datetime_change"] = timestamp;

                        evaluationTable.Add(newEvaluation);
                    }
                }
                else if (sumOfCurrentDemands == totalCapacity || currentRoundNumber == 1)
                    foreach (var demand in currentDemands)
                    {
                        double demandAbsolute = Convert.ToDouble(demand["DemandAbsolute"]);
                        double demandFraction = Convert.ToDouble(demand["DemandFraction"]);
                        int orgId = (int)demand["OrgId"];
                        int period = (int)demand["Period"];
                        double roundPrice = Convert.ToDouble(demand["RoundPrice"]);

                        var newEvaluation = new DBItem(db, evaluationTable);
                        newEvaluation["id_auction"] = auctionId;
                        newEvaluation["id_organization"] = orgId;

                        newEvaluation["assigned_capacity_absolute"] = demandAbsolute;
                        newEvaluation["assigned_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                        newEvaluation["demanded_capacity_absolute"] = demandAbsolute;
                        newEvaluation["demanded_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                        newEvaluation["unsatisfied_capacity_absolute"] = 0;
                        newEvaluation["unsatisfied_capacity_percentage"] = 0;
                        newEvaluation["supplemented_capacity_absolute"] = 0;
                        newEvaluation["supplemented_capacity_percentage"] = 0;

                        newEvaluation["period"] = period;
                        newEvaluation["unit_price"] = roundPrice;
                        newEvaluation["total_price"] = roundPrice * demandAbsolute;

                        newEvaluation["id_user_insert"] = userId;
                        newEvaluation["id_user_change"] = userId;
                        newEvaluation["datetime_insert"] = timestamp;
                        newEvaluation["datetime_change"] = timestamp;

                        evaluationTable.Add(newEvaluation);
                    }
                else
                {
                    // Zkombinování poptávek s poptávkami z přechozího kola
                    // poptávka je menší než celkem -> sumOfCurrentDemands < totalCapacity
                    var previousDemands = (List<DBItem>)vars["PreviousDemands"];
                    double sumOfPreviousDemands = previousDemands.Sum(d => Convert.ToDouble(d["DemandAbsolute"]));
                    Dictionary<int, DBItem> evaluations = new Dictionary<int, DBItem>();

                    foreach (DBItem demand in currentDemands)
                    {
                        double demandAbsolute = Convert.ToDouble(demand["DemandAbsolute"]);
                        double demandFraction = Convert.ToDouble(demand["DemandFraction"]);
                        int orgId = (int)demand["OrgId"];
                        int period = (int)demand["Period"];
                        double roundPrice = Convert.ToDouble(demand["RoundPrice"]);

                        var newEvaluation = new DBItem(db, evaluationTable);
                        newEvaluation["id_auction"] = auctionId;
                        newEvaluation["id_organization"] = orgId;

                        newEvaluation["assigned_capacity_absolute"] = demandAbsolute;
                        newEvaluation["assigned_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                        newEvaluation["demanded_capacity_absolute"] = demandAbsolute;
                        newEvaluation["demanded_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                        newEvaluation["unsatisfied_capacity_absolute"] = 0;
                        newEvaluation["unsatisfied_capacity_percentage"] = 0;
                        newEvaluation["supplemented_capacity_absolute"] = 0;
                        newEvaluation["supplemented_capacity_percentage"] = 0;

                        newEvaluation["period"] = period;
                        newEvaluation["unit_price"] = roundPrice;
                        newEvaluation["total_price"] = roundPrice * demandAbsolute;

                        newEvaluation["id_user_insert"] = userId;
                        newEvaluation["id_user_change"] = userId;
                        newEvaluation["datetime_insert"] = timestamp;
                        newEvaluation["datetime_change"] = timestamp;

                        evaluations.Add(orgId, newEvaluation);

                        foreach (DBItem previousDemand in previousDemands)
                        {
                            if (orgId == (int)previousDemand["OrgId"])
                                previousDemand["DemandAbsolute"] = Convert.ToDouble(previousDemand["DemandAbsolute"]) - Convert.ToDouble(demand["DemandAbsolute"]);
                        }
                    }

                    sumOfPreviousDemands -= sumOfCurrentDemands;

                    foreach (DBItem demand in previousDemands)
                    {
                        double demandAbsolute = Convert.ToDouble(demand["DemandAbsolute"]);
                        double previousFinalAbsolute = System.Math.Round((demandAbsolute / sumOfPreviousDemands) * (totalCapacity - sumOfCurrentDemands), 3);
                        double demandFraction = Convert.ToDouble(demand["DemandFraction"]);
                        int orgId = (int)demand["OrgId"];
                        int period = (int)demand["Period"];
                        double roundPrice = Convert.ToDouble(demand["RoundPrice"]);

                        if (evaluations.ContainsKey(orgId))
                        {
                            // already exists -> merge
                            DBItem newEvaluation = evaluations[orgId];

                            double totalAssignedAbsolute = (double)newEvaluation["assigned_capacity_absolute"] + previousFinalAbsolute;
                            double totalAssignedPercentage = System.Math.Round((totalAssignedAbsolute / totalCapacity) * 100, 2);
                            double totalPrice = (double)newEvaluation["total_price"] + (roundPrice * previousFinalAbsolute);
                            double unitPrice = System.Math.Round(totalPrice / totalAssignedAbsolute, 2);

                            newEvaluation["assigned_capacity_absolute"] = totalAssignedAbsolute;
                            newEvaluation["assigned_capacity_percentage"] = totalAssignedPercentage;
                            newEvaluation["supplemented_capacity_absolute"] = previousFinalAbsolute;
                            newEvaluation["supplemented_capacity_percentage"] = System.Math.Round((previousFinalAbsolute / totalCapacity) * 100, 2);
                            newEvaluation["unit_price"] = unitPrice;
                            newEvaluation["total_price"] = totalPrice;
                        }
                        else
                        {
                            // add
                            // count capacity
                            var newEvaluation = new DBItem(db, evaluationTable);
                            newEvaluation["id_auction"] = auctionId;
                            newEvaluation["id_organization"] = orgId;

                            newEvaluation["assigned_capacity_absolute"] = previousFinalAbsolute;
                            newEvaluation["assigned_capacity_percentage"] = System.Math.Round((previousFinalAbsolute / totalCapacity) * 100, 2);
                            newEvaluation["demanded_capacity_absolute"] = demandAbsolute;
                            newEvaluation["demanded_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                            newEvaluation["unsatisfied_capacity_absolute"] = 0;
                            newEvaluation["unsatisfied_capacity_percentage"] = 0;
                            newEvaluation["supplemented_capacity_absolute"] = 0;
                            newEvaluation["supplemented_capacity_percentage"] = 0;

                            newEvaluation["period"] = period;
                            newEvaluation["unit_price"] = roundPrice;
                            newEvaluation["total_price"] = roundPrice * previousFinalAbsolute;

                            newEvaluation["id_user_insert"] = userId;
                            newEvaluation["id_user_change"] = userId;
                            newEvaluation["datetime_insert"] = timestamp;
                            newEvaluation["datetime_change"] = timestamp;

                            evaluations.Add(orgId, newEvaluation);
                        }
                    }

                    // save created evaluations
                    foreach (var pair in evaluations)
                    {
                        evaluationTable.Add(pair.Value);
                    }
                }
            }
            else
            {
                // Vyhodnocení klesající aukce, kde se poptávky řadí podle časové priority

                // předcházející kola
                double previousRoundsSum = db.Tabloid("DemandsForRound")
                                        .Select("Demand").Where(c =>
                                            c.Column("AuctionId").Equal(auctionId)
                                            .And.Column("RoundNumber").NotEqual(currentRoundNumber))
                                        .ToList().Sum(d => Convert.ToDouble(d["Demand"]));
                remainingCapacity -= previousRoundsSum;
                // toto kolo
                foreach (var demand in currentDemands)
                {
                    if (remainingCapacity <= 0)
                        break;

                    double demandAbsolute = Convert.ToDouble(demand["DemandAbsolute"]);
                    double demandFraction = Convert.ToDouble(demand["DemandFraction"]);
                    double assignedAbsolute =
                        remainingCapacity < demandAbsolute
                        ? remainingCapacity
                        : demandAbsolute;
                    int orgId = (int)demand["OrgId"];
                    int period = (int)demand["Period"];
                    double roundPrice = Convert.ToDouble(demand["RoundPrice"]);
                    remainingCapacity -= assignedAbsolute;

                    var newEvaluation = new DBItem(db, evaluationTable);
                    newEvaluation["id_auction"] = auctionId;
                    newEvaluation["id_organization"] = orgId;

                    newEvaluation["assigned_capacity_absolute"] = assignedAbsolute;
                    newEvaluation["assigned_capacity_percentage"] = System.Math.Round((assignedAbsolute / totalCapacity) * 100, 2);
                    newEvaluation["demanded_capacity_absolute"] = demandAbsolute;
                    newEvaluation["demanded_capacity_percentage"] = System.Math.Round(demandFraction * 100, 2);
                    newEvaluation["unsatisfied_capacity_absolute"] = demandAbsolute - assignedAbsolute;
                    newEvaluation["unsatisfied_capacity_percentage"] = System.Math.Round(((demandAbsolute - assignedAbsolute) / totalCapacity) * 100, 2);
                    newEvaluation["supplemented_capacity_absolute"] = 0;
                    newEvaluation["supplemented_capacity_percentage"] = 0;

                    newEvaluation["period"] = period;
                    newEvaluation["unit_price"] = roundPrice;
                    newEvaluation["total_price"] = roundPrice * assignedAbsolute;

                    newEvaluation["id_user_insert"] = userId;
                    newEvaluation["id_user_change"] = userId;
                    newEvaluation["datetime_insert"] = timestamp;
                    newEvaluation["datetime_change"] = timestamp;

                    evaluationTable.Add(newEvaluation);
                }
            }
            db.SaveChanges();
        }
    }
}
