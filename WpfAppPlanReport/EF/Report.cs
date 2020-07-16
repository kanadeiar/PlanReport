namespace WpfAppPlanReport.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Report")]
    public partial class Report
    {
        public int Id { get; set; }

        public int? PlanId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? Datetime { get; set; }

        [StringLength(200)]
        public string ReportText { get; set; }

        public bool? Complete { get; set; }

        public virtual Plan Plan { get; set; }
    }
}
