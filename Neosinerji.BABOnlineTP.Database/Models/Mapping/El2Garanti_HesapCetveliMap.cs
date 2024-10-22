using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class El2Garanti_HesapCetveliMap : EntityTypeConfiguration<El2Garanti_HesapCetveli>
    {
        public El2Garanti_HesapCetveliMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Periyod, t.TeminatTuru, t.MotorHacmi });

            // Properties
            this.Property(t => t.TeminatTuru)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("El2Garanti_HesapCetveli");
            this.Property(t => t.Periyod).HasColumnName("Periyod");
            this.Property(t => t.TeminatTuru).HasColumnName("TeminatTuru");
            this.Property(t => t.TeminatTutari).HasColumnName("TeminatTutari");
            this.Property(t => t.MotorHacmi).HasColumnName("MotorHacmi");
            this.Property(t => t.Optional_1).HasColumnName("Optional_1");
            this.Property(t => t.Optional_2).HasColumnName("Optional_2");
            this.Property(t => t.Mandatory_1).HasColumnName("Mandatory_1");
            this.Property(t => t.Mandatory_2).HasColumnName("Mandatory_2");
        }
    }
}
