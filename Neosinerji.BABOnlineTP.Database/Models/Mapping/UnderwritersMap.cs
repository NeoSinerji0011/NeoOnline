using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class UnderwritersMap : EntityTypeConfiguration<Underwriters>
    {
        public UnderwritersMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.UnderwriterAdi)
                .HasMaxLength(150);

            this.ToTable("Underwriters");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.UnderwriterAdi).HasColumnName("UnderwriterAdi");
            this.Property(t => t.UnderwriterPayOrani).HasColumnName("UnderwriterPayOrani");
        }
          
    }
}
