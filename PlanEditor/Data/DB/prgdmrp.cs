//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlanEditor.Data.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class prgdmrp
    {
        public string ItemCode { get; set; }
        public string TypeCode { get; set; }
        public System.DateTime MDate { get; set; }
        public Nullable<decimal> COrder { get; set; }
        public Nullable<decimal> Unofficial { get; set; }
        public Nullable<decimal> CsmpParent { get; set; }
        public Nullable<decimal> DeliOrder { get; set; }
        public Nullable<decimal> Purchase { get; set; }
        public Nullable<decimal> RecievePlan { get; set; }
        public Nullable<decimal> ProdPlan { get; set; }
        public Nullable<decimal> Stock { get; set; }
        public Nullable<decimal> Net { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
