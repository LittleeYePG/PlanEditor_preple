using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor_Plepor
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
            if (date.Date == DateTime.Today && date.Hour == 18)
            {
                ot = 0;
            }

            TimeSpan Start = Funcion.clsCFunction.OTDB.GetMinTime(date); //Funcion.clsCFunction.StartTime;
            //if (Funcion.clsCFunction.StartTime < Start)
            //    Start = Funcion.clsCFunction.StartTime;

            TimeSpan End = Funcion.clsCFunction.OTDB.GetMaxTime(date);
            //if (End < Funcion.clsCFunction.EndTime)
            //    End = Funcion.clsCFunction.EndTime;

            
            if (date.Hour >= Start.Hours && date.Hour <= End.Hours)
                return true;// !(date.Hour == 14);
            else return false;
        }
    }
}
