using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlanEditor_Plepor.uControl
{
    public partial class ucScalTime : DevExpress.XtraEditors.XtraUserControl
    {
        public double GetScal
        {
            get
            {
                return pStart.Width / 2;
            }
        }
        public double GetScalMinutes
        {
            get
            {
                return (pStart.Width / 2) / 60.0;
            }
        }
        public ucScalTime()
        {
            InitializeComponent();
            pDetail.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, this.pDetail.ClientRectangle, Color.DarkBlue, ButtonBorderStyle.Solid);
            };
        }

        public bool AddObject(string _name,TimeSpan stTime,TimeSpan etTime)
        {
            try
            {
                uControl.ucObject uc = new ucObject();
                uc.Name = _name;
                uc.TextStart = string.Format("{0:D2}:{1:D2}", stTime.Hours, stTime.Minutes);
                uc.TextEnd = string.Format("{0:D2}:{1:D2}", etTime.Hours, etTime.Minutes);
                uc.TextDetail = uc.TextStart + " - " + uc.TextEnd;
                
                uc.Width = Convert.ToInt32(((etTime - stTime).TotalMinutes * GetScalMinutes));
                uc.Height = pDetail.Height - 2;
                uc.Top = uc.Top + 1;
                uc.Left = (int)(stTime.TotalMinutes * GetScalMinutes);
                uc.Visible = true;

                uc.BringToFront();
                pDetail.Controls.Add(uc);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ClsObject()
        {
            try
            {
                pDetail.Controls.Clear();
                //foreach (Control c in pDetail.Controls)
                //{
                //    pDetail.Controls.Remove(c);
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
