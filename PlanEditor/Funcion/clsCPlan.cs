﻿using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlanEditor.Funcion
{
    public class clsCPlan
    {
        public List<Data.cMRPCapacity> GetCMRPCapacities(DateTime dtpDateFr, DateTime dtpDateTo, List<Data.cResource> resources)
        {
            //List<Data.cMRPCapacity> cs = new List<Data.cMRPCapacity>();
            using (Data.DB.PlanEditorEntities db = new Data.DB.PlanEditorEntities())
            {
                var lType = new string[] {"0","1" };
                var result = (from t1 in db.prgdmrps
                              join t2 in db.mstitems on t1.ItemCode equals t2.ItemCode
                              where t1.MDate >= dtpDateFr.Date
                              where t1.MDate <= dtpDateTo.Date
                              where t1.Net > 0
                              where lType.Contains(t2.ItemType)
                              select new Data.cMRPCapacity
                              {
                                  LineCode = t2.DefaultLineCode.ToString(),
                                  TimeWork = (double)(db.mstitemcapacities
                                            .Where(w => w.ItemCode == t1.ItemCode &&
                                            w.TypeCode == t1.TypeCode &&
                                            w.LineCode == t2.DefaultLineCode.ToString()).Select(s => (decimal?)s.WorkingTime).FirstOrDefault() ?? (decimal)0.00),
                                  MDate = (DateTime)t1.MDate,
                                  ItemCode = t1.ItemCode,
                                  ItemName = t2.ItemName,
                                  TypeCode = t1.TypeCode,
                                  OrderProdNo = t2.ItemName,
                                  PlanQty = (decimal?)t1.Net ?? (decimal)0.00, 
                                  LabelID = 1,
                                  MDate_Ref = (DateTime)t1.MDate,
                                  RemainCalQty = (decimal?)t1.Net ?? (decimal)0.00,
                              }
                          ).ToList();

                int appid = 1;
                foreach (var r in result)
                {
                    r.ID = appid;
                    r.TotalWorkTime = r.PlanQty * (decimal)r.TimeWork;
                    var id = resources.Where(w => w.Code == r.LineCode+"MRP").Select(s => s.Id).FirstOrDefault();
                    if (id != null)
                    {
                        r.ResourceID = id;
                    }
                    else
                    {
                        r.ResourceID = 0;
                    }
                    appid++;
                }
                return result;
            }
            
        }
        public List<Data.cMRPCapacity> calCMRPPercen(List<Data.cMRPCapacity> mRPCapacities)
        {
            //List<Data.cMRPCapacity> cs = new List<Data.cMRPCapacity>();
            //foreach (var r in mRPCapacities)
            //{

            //}
            //return cs;
            var aa = (from t1 in mRPCapacities
                      group new { t1 } by new
                      {
                          t1.LineCode,
                          t1.MDate,
                          //t1.MDate_Ref,
                          t1.ResourceID,
                          t1.LabelID,
                      } into g
                      select new Data.cMRPCapacity
                      {
                          LineCode = g.Key.LineCode,
                          LabelID = g.Key.LabelID,
                          MDate = g.Key.MDate,
                          ResourceID = g.Key.ResourceID,
                          MRPWorkTime = g.Sum(x => x.t1.TotalWorkTime),
                          MRPCapacity = (g.Sum(x => x.t1.TotalWorkTime) * 100) / (decimal)clsCFunction.GetWorkHours(g.Key.MDate),
                      }
                         ).ToList();
            return aa;
        }
        
        private List<Data.cMRPCapacity> cs = new List<Data.cMRPCapacity>();
        public List<Data.cMRPCapacity> calCMRP(List<Data.cMRPCapacity> mRPCapacities, List<Data.cResource> resources)
        {
            cs = new List<Data.cMRPCapacity>();
            using (Data.DB.PlanEditorEntities db = new Data.DB.PlanEditorEntities())
            {
                string txt = "";
                foreach (var r in mRPCapacities.Where(w => w.RemainCalQty > 0 && w.TimeWork == 0).ToList())
                {
                    if (txt == "") txt = "ไม่สามารถประมวลผลได้ เนื่องจาก มี ItemCode ที่ยังไม่ได้ Set TimeWork ";
                    txt += Environment.NewLine + r.ItemCode;
                }
                if (txt != "")
                {
                    XtraMessageBox.Show(txt, "Warning", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    return cs;
                }
                
                #region Round 3 หาวันก่อนหน้า ที่อยู่ใน line ของมันเอง - 7 วัน
                //for (int i = 0;i< 7; i++)
                //{
                //    mRPCapacities = calLoop(mRPCapacities, resources, i);
                //}

                int i = 0;
                while (mRPCapacities.Where(w=>w.RemainCalQty > 0).ToList().Count() > 0)
                {
                    mRPCapacities = calLoop(mRPCapacities, resources, i);
                    i++;
                }

                #endregion
            }
            return cs;
        }
        private List<Data.cMRPCapacity> calLoop(List<Data.cMRPCapacity> mRPCapacities, List<Data.cResource> resources,int dayDiff)
        {
            Data.mstLineDB lineDB = new Data.mstLineDB();
            using (Data.DB.PlanEditorEntities db = new Data.DB.PlanEditorEntities())
            {
                dayDiff = dayDiff * -1;
                decimal FreeTime = clsCFunction.WorkHours;
                decimal Qty = 0;
                #region Round 1 หาในวันปัจจุบันและใน line ของตัวมันเอง
                foreach (var c in mRPCapacities.Where(w=>w.RemainCalQty > 0).OrderBy(o => o.LineCode).OrderBy(o1 => o1.MDate))
                {
                    DateTime MDate = c.MDate.AddDays(dayDiff);
                    Qty = c.RemainCalQty;
                    if (c.TimeWork == 0) continue;
                    //while (Qty > 0)
                    //{
                    FreeTime = clsCFunction.GetWorkHours(MDate);
                    Data.cMRPCapacity cMRP = new Data.cMRPCapacity();
                    cMRP.ItemCode = c.ItemCode;
                    cMRP.TypeCode = c.TypeCode;
                    cMRP.OrderProdNo = c.OrderProdNo;
                    cMRP.TimeWork = c.TimeWork;
                    cMRP.LabelID = 2;
                    cMRP.MDate_Ref = c.MDate;
                    cMRP.LineCode = c.LineCode;
                    cMRP.ItemName = c.ItemName;
                    //if (c.ItemCode == "00TSBB0086802" || c.ItemCode == "00TSBB0088802_M1")
                    //{
                    //    cMRP.LabelID = 2;
                    //}

                    //หาว่ามีรายดารนี้ หรือยัง
                    decimal TotalTime = 0;
                    var en = cs.Where(w => w.LineCode == c.LineCode && w.MDate == MDate).Sum(s => s.TotalWorkTime);
                    if (en != null)
                    {
                        FreeTime -= en;
                        TotalTime = en;
                    }

                    if (FreeTime > 0)
                    {
                        var cQty = (FreeTime / (decimal)c.TimeWork);
                        if (cQty >= Qty)
                        {
                            cMRP.PlanQty = Qty;
                        }
                        else
                        {
                            cMRP.PlanQty = cQty;
                        }


                        //cMRP.PlanQty = Qty - (FreeTime / (decimal)c.TimeWork);
                        cMRP.MDate = MDate;
                        cMRP.StartPlanDate = MDate.AddMinutes(clsCFunction.GetMinutes(clsCFunction.StartTime));
                        cMRP.StartPlanDate = cMRP.StartPlanDate.AddMinutes((double)TotalTime);
                        cMRP.EndPlanDate = cMRP.StartPlanDate.AddMinutes((double)cMRP.PlanQty * (double)cMRP.TimeWork);
                        cMRP.MRPCapacity = cMRP.PlanQty * (decimal)cMRP.TimeWork;
                        cMRP.TotalWorkTime = cMRP.PlanQty * (decimal)cMRP.TimeWork;
                        var id = resources.Where(w => w.Code == c.LineCode + "CRP").FirstOrDefault();
                        if (id != null)
                        {
                            cMRP.ResourceID = id.Id;
                        }
                        else
                        {
                            cMRP.ResourceID = 0;
                        }

                        if (dayDiff*-1 > 0)
                        {
                            cMRP.Remark = string.Format(@"Ref. {0} Line {1} Round {2}", c.MDate.ToString("dd/MM/yyyy"), lineDB.getLineName(Convert.ToInt32(c.LineCode)), dayDiff * -1);
                        }

                        Qty -= cMRP.PlanQty;
                        cs.Add(cMRP);
                        c.RemainCalQty = Qty;
                    }

                    //    Qty = 0;
                    //}
                }
                #endregion
                #region Round 2 หาในวันปัจจุบันและใน line ที่สามารถลงได้
                foreach (var c in mRPCapacities.Where(w => w.RemainCalQty > 0).OrderBy(o => o.LineCode).OrderBy(o1 => o1.MDate))
                {
                    DateTime MDate = c.MDate.AddDays(dayDiff);
                    Qty = c.RemainCalQty;
                    if (c.TimeWork == 0) continue;
                    //while (Qty > 0)
                    //{
                    FreeTime = clsCFunction.GetWorkHours(MDate);

                    foreach (var l in lineDB.GetMstitemcapacities(c.ItemCode, c.TypeCode, c.LineCode))
                    {
                        if (Qty == 0) continue;
                        //หาว่ามีรายดารนี้ หรือยัง
                        decimal TotalTime = 0;
                        var en = cs.Where(w => w.LineCode == l.LineCode && w.MDate == MDate).Sum(s => s.TotalWorkTime);
                        if (en != null)
                        {
                            FreeTime -= en;
                            TotalTime = en;
                        }

                        if (FreeTime > 0)
                        {
                            Data.cMRPCapacity cMRP = new Data.cMRPCapacity();
                            cMRP.ItemCode = c.ItemCode;
                            cMRP.TypeCode = c.TypeCode;
                            cMRP.OrderProdNo = c.OrderProdNo;
                            cMRP.TimeWork = (double)l.WorkingTime;
                            cMRP.LabelID = 2;
                            cMRP.MDate_Ref = c.MDate;
                            cMRP.LineCode = l.LineCode;
                            cMRP.ItemName = c.ItemName;
                            var cQty = (FreeTime / (decimal)l.WorkingTime);
                            if (cQty >= Qty)
                            {
                                cMRP.PlanQty = Qty;
                            }
                            else
                            {
                                cMRP.PlanQty = cQty;
                            }

                            cMRP.MDate = MDate;
                            cMRP.StartPlanDate = MDate.AddMinutes(clsCFunction.GetMinutes(clsCFunction.StartTime));
                            cMRP.StartPlanDate = cMRP.StartPlanDate.AddMinutes((double)TotalTime);
                            cMRP.EndPlanDate = cMRP.StartPlanDate.AddMinutes((double)cMRP.PlanQty * (double)cMRP.TimeWork);
                            cMRP.MRPCapacity = cMRP.PlanQty * (decimal)cMRP.TimeWork;
                            cMRP.TotalWorkTime = cMRP.PlanQty * (decimal)cMRP.TimeWork;
                            var id = resources.Where(w => w.Code == l.LineCode + "CRP").FirstOrDefault();
                            if (id != null)
                            {
                                cMRP.ResourceID = id.Id;
                            }
                            else
                            {
                                cMRP.ResourceID = 0;
                            }

                            cMRP.Remark = string.Format(@"Ref. {0} Line {1} Round {2}", c.MDate.ToString("dd/MM/yyyy"), lineDB.getLineName(Convert.ToInt32(c.LineCode)),dayDiff*-1);

                            Qty -= cMRP.PlanQty;
                            cs.Add(cMRP);
                            c.RemainCalQty = Qty;
                        }
                    }

                }
                #endregion
            }
            return mRPCapacities;
        }
        
    }
}
