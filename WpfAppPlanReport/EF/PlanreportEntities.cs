namespace WpfAppPlanReport.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PlanReportEntities : DbContext
    {
        public PlanReportEntities()
            : base("name=PlanReportEntities")
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasMany(e => e.Plans)
                .WithOptional(e => e.Department)
                .HasForeignKey(e => e.DepId);
        }
    }
}
