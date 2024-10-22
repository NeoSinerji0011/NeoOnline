using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMDepartmanlarMap : EntityTypeConfiguration<TVMDepartmanlar>
    {
        public TVMDepartmanlarMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.DepartmanKodu });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DepartmanKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Adi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TVMDepartmanlar");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.DepartmanKodu).HasColumnName("DepartmanKodu");
            this.Property(t => t.Adi).HasColumnName("Adi");
            this.Property(t => t.BolgeKodu).HasColumnName("BolgeKodu");
            this.Property(t => t.MerkezYetkisi).HasColumnName("MerkezYetkisi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
