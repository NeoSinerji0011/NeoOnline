using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class script_deploymentsMap : EntityTypeConfiguration<script_deployments>
    {
        public script_deploymentsMap()
        {
            // Primary Key
            this.HasKey(t => new { t.deployment_id, t.coordinator_id, t.deployment_name, t.deployment_submitted, t.status, t.retry_policy, t.script });

            // Properties
            this.Property(t => t.deployment_name)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.status)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.results_table)
                .HasMaxLength(261);

            this.Property(t => t.retry_policy)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.script)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("script_deployments", "sys");
            this.Property(t => t.deployment_id).HasColumnName("deployment_id");
            this.Property(t => t.coordinator_id).HasColumnName("coordinator_id");
            this.Property(t => t.deployment_name).HasColumnName("deployment_name");
            this.Property(t => t.deployment_submitted).HasColumnName("deployment_submitted");
            this.Property(t => t.deployment_start).HasColumnName("deployment_start");
            this.Property(t => t.deployment_end).HasColumnName("deployment_end");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.results_table).HasColumnName("results_table");
            this.Property(t => t.retry_policy).HasColumnName("retry_policy");
            this.Property(t => t.script).HasColumnName("script");
            this.Property(t => t.messages).HasColumnName("messages");
        }
    }
}
