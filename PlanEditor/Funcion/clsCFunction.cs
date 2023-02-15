using System;
using System.Collections.Generic;
using System.Configuration;
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

        public static TimeSpan StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }
        public static TimeSpan EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                endTime = value;
            }
        }
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
                sTime = "-";
            return sTime;
        }
        public static double GetMinutes(TimeSpan time)
        {
            double t = 0;
            t = time.Hours * 60;
            t += time.Minutes;
            return t;
        }
    }
}
