namespace WpfAppPlanReport.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Plan")]
    public partial class Plan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Plan()
        {
            Reports = new HashSet<Report>();
        }

        public int Id { get; set; }

        public int? DepId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? Datetime { get; set; }

        [Required]
        [StringLength(500)]
        public string PlanText { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report> Reports { get; set; }
    }
}
