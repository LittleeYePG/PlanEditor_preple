using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor
{
    public class MyCustomScale : TimeScaleHour
    {
        public MyCustomScale() { }

        public override string DisplayName { get => "Custom Work Hours"; set => base.DisplayName = value; }
        public override string MenuCaption { get => "Custom Work Hours"; set => base.MenuCaption = value; }

        public override string FormatCaption(DateTime start, DateTime end)
        {
            if (start.Hour <= 12) return start.Hour.ToString() + " AM";
            else return (start.Hour - 12).ToString() + " PM";
        }
        public override bool IsDateVisible(DateTime date)
        {
            int ot = 0;
            TimeSpan Start = Funcion.clsCFunction.StartTime;
            TimeSpan End = Funcion.clsCFunction.EndTime;

            if (date.Date.Equals(DateTime.Today))
                End += TimeSpan.FromMinutes((double)Funcion.clsCFunction.GetOT);


            if (date.Hour >= Start.Hours && date.Hour <= End.Hours)
                return true;// !(date.Hour == 14);
            else return false;
        }
    }
}
