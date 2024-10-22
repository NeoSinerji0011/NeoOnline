using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMYetkiGrupSablonuMap : EntityTypeConfiguration<TVMYetkiGrupSablonu>
    {
        public TVMYetkiGrupSablonuMap()
        {
            // Primary Key
            this.HasKey(t => t.YetkiSablonId);

            this.Property(t => t.YetkiGrupAdi)
             .IsRequired()
             .HasMaxLength(50);
            this.Property(t => t.AnaMenuAciklama)
             .IsRequired()
             .HasMaxLength(150);
            this.Property(t => t.AltMenuAciklama)
              .HasMaxLength(150);
            this.Property(t => t.SekmeAciklama)
              .HasMaxLength(150);
            // Properties
            // Table & Column Mappings
            this.ToTable("TVMYetkiGrupSablonu");
            this.Property(t => t.YetkiSablonId).HasColumnName("YetkiSablonId");
            this.Property(t => t.YetkiGrupKodu).HasColumnName("YetkiGrupKodu");
            this.Property(t => t.YetkiGrupAdi).HasColumnName("YetkiGrupAdi");
           // this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.AnaMenuKodu).HasColumnName("AnaMenuKodu");
            this.Property(t => t.AltMenuKodu).HasColumnName("AltMenuKodu");
            this.Property(t => t.SekmeKodu).HasColumnName("SekmeKodu");
            this.Property(t => t.AnaMenuAciklama).HasColumnName("AnaMenuAciklama");
            this.Property(t => t.AltMenuAciklama).HasColumnName("AltMenuAciklama");
            this.Property(t => t.SekmeAciklama).HasColumnName("SekmeAciklama");
            this.Property(t => t.Gorme).HasColumnName("Gorme");
            this.Property(t => t.YeniKayit).HasColumnName("YeniKayit");
            this.Property(t => t.Degistirme).HasColumnName("Degistirme");
            this.Property(t => t.Silme).HasColumnName("Silme");
        }

    }
}

