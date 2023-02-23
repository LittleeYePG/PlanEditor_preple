using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlanEditor.Funcion
{
    public static class clsCFunction
    {
        public static bool getDataBaseConnect
        {
            get
            {
                try
                {
                    using (Data.DB.PlanEditorEntities db = new Data.DB.PlanEditorEntities())
                    {                        
                        db.Database.Connection.Open();
                        db.Database.Connection.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // OBS3.sysFunction.sysLoggin.WriterLog(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    MessageBox.Show(ex.Message); 
                    return false;
                }
            }
        }
        public static string getDataSource
        {
            get
            {
                try
                {
                    using (Data.DB.PlanEditorEntities db = new Data.DB.PlanEditorEntities())
                    {
                        return db.Database.Connection.DataSource;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return "";
                }
            }
        }
        public static string getDataBaseName
        {
            get
            {
                try
                {
                    using (Data.DB.PlanEditorEntities db = new Data.DB.PlanEditorEntities())
                    {
                        return db.Database.Connection.Database;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return "";
                }
            }
        }

        private static TimeSpan startTime = Properties.Settings.Default.StartTime;
        private static TimeSpan endTime = Properties.Settings.Default.EndTime;
        public static string version 
        { 
            get 
            { 
              return "Version Demo [" +  Assembly.GetExecutingAssembly().GetName().Version.ToString()+"]   "; 
            } 
        }
        public static decimal GetOT { get; set; } = 0;        
        public static decimal WorkHours
        {
            get
            {
                return (decimal)(EndTime - StartTime).TotalMinutes;
            }
        }
        public static decimal GetWorkHours(DateTime date)
        {
            try
            {
                using (Data.DB.PlanEditorEntities db = new Data.DB.PlanEditorEntities())
                {
                    if (date == DateTime.Today)
                    {
                        return (decimal)(EndTime - StartTime).TotalMinutes + GetOT;
                    }
                    else
                    {
                        return (decimal)(EndTime - StartTime).TotalMinutes;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }

            
        }
        public static string InfoWorkingTime(int WorkTime)
        {
            string sTime = "";
            if (WorkTime > 60)
            {
                int iH = (int)Math.Floor((decimal)(WorkTime / 60));
                WorkTime -= iH * 60;
                sTime += iH + " hrs.";
            }
            if (WorkTime > 0)
            {
                if (sTime.Length > 0)
                    sTime += " ";

                sTime += WorkTime + " min.";
            }
            if (sTime.Length == 0)
                sTime = "0 hrs.";
            return sTime;
        }
        public static double GetMinutes(TimeSpan time)
        {
            double t = 0;
            t = time.Hours * 60;
            t += time.Minutes;
            return t;
        }

        public static Data.mmstOTDB OTDB = new Data.mmstOTDB();


        private static List<Data.DB.mstLine_WorkTime> _WorkTimes = null;
        public static List<Data.DB.mstLine_WorkTime> WorkTimes
        {
            get
            {
                return _WorkTimes;
            }
            set
            {
                _WorkTimes = value;
            }
        }

        /// <summary>
        /// Check ว่า Load DB มาแล้วหรือยัง
        /// </summary>
        /// <returns></returns>
        private static bool checkWorkTimeDB()
        {
            try
            {
                if (WorkTimes == null)
                {
                    using (Data.DB.PlanEditorEntities db = new Data.DB.PlanEditorEntities())
                    {
                        WorkTimes = db.mstLine_WorkTime.ToList();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// เช็คว่า date และ linecode ว่าใช่เวลาทำงานหรือไม่
        /// </summary>
        /// <param name="date"></param>
        /// <param name="LineCode"></param>
        /// <returns></returns>
        public static bool checkWorkTime(DateTime date, string LineCode)
        {
            try
            {
                if (!checkWorkTimeDB())
                {
                    return false;
                }
                var result = WorkTimes
                    .Where(w => w.LineCode.Equals(LineCode) && 
                            w.StartTime <= date.TimeOfDay &&
                            w.EndTime > date.TimeOfDay)
                    .ToList();
                if (result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return OTDB.checkTimeOT(date, LineCode);
                }
                    return false;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// หาค่า worktime ของ linecode คืนค่าเป็น Minutes
        /// </summary>
        /// <param name="LineCode"></param>
        /// <returns></returns>
        public static double GetmstWorkTime_Minutes(DateTime date,string LineCode)
        {
            try
            {
                if (!checkWorkTimeDB())
                {
                    return 0;
                }
                var result = GetWorkTimeofDay(date, LineCode);

                //aptnew.SetId((GetOTDB.GetMstOTs.Max(m => (int?)m.ID) ?? 0) + 1);
                //var result = (WorkTimes
                //    .Where(w => w.LineCode.Equals(LineCode))                    
                //    ).ToList();
                var m = 0.0;
                foreach (var r in result)
                {
                    m += ((TimeSpan)r.EndTime - (TimeSpan)r.StartTime).TotalMinutes;
                }
                //m += OTDB.GetOTTime(date, LineCode);
                return m;
            }
            catch
            {
                return 0;
            }
        }


        /// <summary>
        /// Return Start Time 
        /// </summary>
        public static TimeSpan StartTime
        {
            get
            {
                try
                {
                    if (checkWorkTimeDB())
                    {
                       // var ot = OTDB.GetMinTime();

                        var result = WorkTimes.OrderBy(o => o.StartTime).FirstOrDefault();
                        if (result != null)
                        { 
                            //if ((TimeSpan)result.StartTime < ot)
                            //{
                                return (TimeSpan)result.StartTime;
                            //}
                            //else
                            //{
                            //    return ot;
                            //}
                        }                            
                        else
                            startTime = new TimeSpan();
                    }
                    else
                    {
                        startTime = new TimeSpan();
                    }
                }
                catch
                {
                    startTime = new TimeSpan();
                }
                return startTime;
            }
        }
        /// <summary>
        /// Return End Time
        /// </summary>
        public static TimeSpan EndTime
        {
            get
            {
                try
                {
                    if (checkWorkTimeDB())
                    {
                       // var ot = OTDB.GetMaxTime();
                        var result = WorkTimes.OrderByDescending(o => o.EndTime).FirstOrDefault();
                        if (result != null)
                        {
                            //if ((TimeSpan)result.EndTime > ot)
                            //{
                                return (TimeSpan)result.EndTime;
                            //}
                            //else
                            //{
                            //    return ot;
                            //}
                        }
                        else
                            startTime = new TimeSpan();
                    }
                    else
                    {
                        startTime = new TimeSpan();
                    }
                }
                catch
                {
                    startTime = new TimeSpan();
                }
                return startTime;
            }
        }


        /// <summary>
        /// return StartTime ที่รวม OT
        /// </summary>
        public static TimeSpan GetStartTime
        {
            get
            {
                return OTDB.GetMinTime();
            }
        }
        /// <summary>
        /// return EndTime ที่รวม OT
        /// </summary>
        public static TimeSpan GetEndTime
        {
            get
            {
                return OTDB.GetMaxTime();
            }
        }

        /// <summary>
        /// หาช่วงเวลาทำงานของแต่ละวัน 
        /// </summary>
        /// <param name="_date"></param>
        /// <param name="_lineCode"></param>
        /// <returns></returns>
        public static List<Data.DB.mstLine_WorkTime> GetWorkTimeofDay(DateTime _date,string _lineCode)
        {
            try
            {
                //List<Data.mmstOTDB> result = new List<Data.mmstOTDB>();
                #region หาเวลาทำงานปัจจุบัน
                var result = (WorkTimes
                    .Where(w => w.LineCode.Equals(_lineCode))
                    ).ToList();
                #endregion

                #region หาเวลาทำงานโอที
                foreach (var r in OTDB.GetMstOTs.Where(w=>w.Date == _date && w.LineCode == _lineCode && w.Type == 2))
                {
                    Data.DB.mstLine_WorkTime ot = new Data.DB.mstLine_WorkTime();
                    ot.StartTime = r.stTime;
                    ot.EndTime = r.etTime;
                    ot.LineCode = r.LineCode;
                    result.Add(ot);
                }
                #endregion
                #region StopTime
                Data.DB.mstLine_WorkTime st = new Data.DB.mstLine_WorkTime();
                foreach (var r in OTDB.GetMstOTs.Where(w => w.Date == _date && w.LineCode == _lineCode && w.Type == 1))
                {
                    //หาช่วงเวลาทำงาน ที่ Stop time เริ่มทำงาน
                    var _t = result.Where(w => w.LineCode == r.LineCode && w.StartTime <= r.stTime && w.EndTime >= r.stTime).FirstOrDefault();
                    if (_t != null)
                    {
                        #region StartTime
                        //ทำส่วนหัว
                        st = new Data.DB.mstLine_WorkTime();

                        if (_t.StartTime != r.stTime)
                        {
                            st.StartTime = _t.StartTime;
                            st.EndTime = r.stTime;
                            st.LineCode = r.LineCode;
                            result.Add(st);
                        }
                        

                        if (_t.EndTime > r.etTime)
                        {
                            // ส่วนที่เหลือ
                            st = new Data.DB.mstLine_WorkTime();
                            st.StartTime = r.etTime;
                            st.EndTime = _t.EndTime;
                            st.LineCode = r.LineCode;
                            result.Add(st);

                            result.Remove(_t);
                        }
                        else
                        {
                            // หาว่า Stop End Time ไปคาบเกียวกับ เวลาทำงานไหนอีก หรือไม่
                            var _o = result.Where(w => w.LineCode == r.LineCode && w.StartTime <= r.etTime && w.EndTime >= r.etTime).FirstOrDefault();
                            if (_o != null)
                            {
                                st = new Data.DB.mstLine_WorkTime();
                                st.StartTime = r.etTime;
                                st.EndTime = _o.EndTime;
                                st.LineCode = r.LineCode;
                                result.Add(st);

                                result.Remove(_o);
                                
                            }
                        }
                        result.Remove(_t);

                        #endregion


                    }



                }
                #endregion
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
    public static class FColor
    {
        public static Color OTTime
        {
            get
            {
                return ColorTranslator.FromHtml("#A9DFBF");
            }
        }
        public static Color StopTime
        {
            get
            {
                return ColorTranslator.FromHtml("#F1948A");
            }
        }
        public static Color StopTimeOT
        {
            get
            {
                return ColorTranslator.FromHtml("#B0BEC5");
            }
        }
        public static Color HoliDay
        {
            get
            {
                return ColorTranslator.FromHtml("#E6B0AA");
            }
        }
        public static Color LineMRP
        {
            get
            {
                return ColorTranslator.FromHtml("#FFFFCC");
            }
        }
        public static Color LineSimulator
        {
            get
            {
                return ColorTranslator.FromHtml("#E1FFE1");
            }
        }
    }
}
