using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AllianzPoliceSilincekMap : EntityTypeConfiguration<AllianzPoliceSilincek>
    {
        public AllianzPoliceSilincekMap()
        {
            // Primary Key
            this.HasKey(t => new { t.policeno, t.ekno });

            // Properties
            this.Property(t => t.policeno)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ekno)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.yenilemeno)
                .HasMaxLength(50);

            this.Property(t => t.sirketno)
                .HasMaxLength(50);

            this.Property(t => t.adi)
                .HasMaxLength(500);

            this.Property(t => t.soyadi)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("AllianzPoliceSilincek");
            this.Property(t => t.policeid).HasColumnName("policeid");
            this.Property(t => t.policeno).HasColumnName("policeno");
            this.Property(t => t.ekno).HasColumnName("ekno");
            this.Property(t => t.yenilemeno).HasColumnName("yenilemeno");
            this.Property(t => t.sirketno).HasColumnName("sirketno");
            this.Property(t => t.adi).HasColumnName("adi");
            this.Property(t => t.soyadi).HasColumnName("soyadi");
        }
    }
}
