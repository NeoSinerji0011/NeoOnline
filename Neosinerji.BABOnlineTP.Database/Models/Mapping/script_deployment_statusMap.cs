using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class script_deployment_statusMap : EntityTypeConfiguration<script_deployment_status>
    {
        public script_deployment_statusMap()
        {
            // Primary Key
            this.HasKey(t => new { t.deployment_id, t.logical_server, t.database_name, t.status, t.num_retries });

            // Properties
            this.Property(t => t.logical_server)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.database_name)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.status)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.num_retries)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("script_deployment_status", "sys");
            this.Property(t => t.deployment_id).HasColumnName("deployment_id");
            this.Property(t => t.worker_id).HasColumnName("worker_id");
            this.Property(t => t.logical_server).HasColumnName("logical_server");
            this.Property(t => t.database_name).HasColumnName("database_name");
            this.Property(t => t.deployment_start).HasColumnName("deployment_start");
            this.Property(t => t.deployment_end).HasColumnName("deployment_end");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.num_retries).HasColumnName("num_retries");
            this.Property(t => t.messages).HasColumnName("messages");
        }
    }
}
