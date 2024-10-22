using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMAcentelikleriMap : EntityTypeConfiguration<TVMAcentelikleri>
    {
        public TVMAcentelikleriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BransKodu)
                .IsRequired();
            this.Property(t => t.SigortaSirketKodu)
              .IsRequired()
              .HasMaxLength(5);


            // Table & Column Mappings
            this.ToTable("TVMAcentelikleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.SigortaSirketKodu).HasColumnName("SigortaSirketKodu");
            this.Property(t => t.OdemeTipi).HasColumnName("OdemeTipi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
