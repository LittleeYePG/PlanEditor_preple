using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.Data
{
    public class mstLineDB
    {
        public List<Data.cResource> GetResource_Demo(bool _dtail = false)
        {
            List<Data.cResource> resources = new List<Data.cResource>();
            using (Data.DB.PlanEditorEntities db = new DB.PlanEditorEntities())
            {
                var result = db.mstline_Marunix.Where(w => w.DelFlag == 0).ToList();
                int c = 0;
                foreach (var r in result)
                {
                    Data.cResource re = new Data.cResource();
                    c++;
                    if (!_dtail)
                    {
                        re.Id = c;
                        re.Code = r.LineID + "MRP";
                        re.Caption = r.LineName;
                        re.Color = 1;
                        re.LineName = r.LineName;
                        re.LineCode = r.LineID.ToString();
                        re.MRP = true;
                        resources.Add(re);

                        c++;
                        Data.cResource re1 = new Data.cResource();
                        re1.Id = c;
                        re1.Code = r.LineID + "CRP";
                        re1.Caption = "";
                        re1.Color = 4;
                        re1.LineName = r.LineName;
                        re1.LineCode = r.LineID.ToString();
                        re1.MRP = false;
                        resources.Add(re1);
                    }

                    if (_dtail)
                    {
                        c++;
                        Data.cResource re1 = new Data.cResource();
                        re1.Id = c;
                        re1.Code = r.LineID + "CRP";
                        re1.Caption = r.LineName;
                        re1.Color = 4;
                        re1.LineName = r.LineName;
                        re1.LineCode = r.LineID.ToString();
                        re1.MRP = false;
                        resources.Add(re1);
                    }
                }
            }
            return resources;
        }
        public List<Data.DB.mstitemcapacity> GetMstitemcapacities(string ItemCode,string TypeCode,string LineCode)
        {
            using (Data.DB.PlanEditorEntities db = new DB.PlanEditorEntities())
            {
                var result = db.mstitemcapacities
                    .Where(w => w.ItemCode == ItemCode && w.TypeCode == TypeCode && w.LineCode != LineCode)
                    .OrderBy(o=>o.LinePriority)
                    .ToList();
                return result;
            }
        }
        public string getLineName(int LineCode)
        {
            using (Data.DB.PlanEditorEntities db = new DB.PlanEditorEntities())
            {
                var result = db.mstline_Marunix
                    .Where(w => w.LineID == LineCode)
                    .FirstOrDefault();
                if (result != null)
                    return result.LineName;
                else
                    return "";
            }
        }
        public bool CheckLine(string ItemCode, int ResourceId, List<Data.cResource> resources)
        {
            var LineCode = resources.Where(w => w.Id == Convert.ToInt32(ResourceId)).Select(s => s.LineCode).FirstOrDefault();
            using (Data.DB.PlanEditorEntities db = new DB.PlanEditorEntities())
            {
                var result = (from t1 in db.mstitemcapacities
                              where t1.ItemCode == ItemCode
                              where t1.LineCode == LineCode
                              select new
                              {
                                  t1.LineCode,
                              }).ToList();
                if (result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public double getWorkTime(string ItemCode, int ResourceId, List<Data.cResource> resources)
        {
            var LineCode = resources.Where(w => w.Id == Convert.ToInt32(ResourceId)).Select(s => s.LineCode).FirstOrDefault();
            using (Data.DB.PlanEditorEntities db = new DB.PlanEditorEntities())
            {
                var result = (from t1 in db.mstitemcapacities
                              where t1.ItemCode == ItemCode
                              where t1.LineCode == LineCode
                              select new
                              {
                                  t1.LineCode,
                                  t1.WorkingTime,
                              }).FirstOrDefault();
                if (result != null)
                {
                    return (double)result.WorkingTime;
                }
                else
                {
                    return 0;
                }
            }
        }
        public double getWorkTime(string ItemCode, string LineCode)
        {
            using (Data.DB.PlanEditorEntities db = new DB.PlanEditorEntities())
            {
                var result = (from t1 in db.mstitemcapacities
                              where t1.ItemCode == ItemCode
                              where t1.LineCode == LineCode
                              select new
                              {
                                  t1.LineCode,
                                  t1.WorkingTime,
                              }).FirstOrDefault();
                if (result != null)
                {
                    return (double)result.WorkingTime;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
