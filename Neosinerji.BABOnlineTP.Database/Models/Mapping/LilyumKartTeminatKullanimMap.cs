using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class LilyumKartTeminatKullanimMap : EntityTypeConfiguration<LilyumKartTeminatKullanim>
    {
        public LilyumKartTeminatKullanimMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id });

            // Properties
            this.Property(t => t.TvmKodu)
                .IsRequired();

            this.Property(t => t.KullaniciKodu)
                  .IsRequired();
            this.Property(t => t.ReferansNo)
                .HasMaxLength(20)
               .IsRequired();

            this.Property(t => t.TeminatId)
                  .IsRequired();
            this.Property(t => t.ToplamKullanimHakkiAdet)
                .IsRequired();

            this.Property(t => t.KayitTarihi)
                .IsRequired();

           
            // Table & Column Mappings
            this.ToTable("LilyumKartTeminatKullanim");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu"); 
            this.Property(t => t.ReferansNo).HasColumnName("ReferansNo"); 
            this.Property(t => t.LilyumKartNo).HasColumnName("LilyumKartNo"); 
            this.Property(t => t.TeminatId).HasColumnName("TeminatId");
            this.Property(t => t.ToplamKullanimHakkiAdet).HasColumnName("ToplamKullanimHakkiAdet");
            this.Property(t => t.ToplamKullanilanAdet).HasColumnName("ToplamKullanilanAdet");
            this.Property(t => t.TeminatSonKullanilanTarihi).HasColumnName("TeminatSonKullanilanTarihi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            this.Property(t => t.EkleyenKullanici).HasColumnName("EkleyenKullanici");
            this.Property(t => t.GuncelleyenKullanici).HasColumnName("GuncelleyenKullanici");

            //// Relationships
            //this.HasRequired(t => t.AracMarka)
            //    .WithMany(t => t.AracTips)
            //    .HasForeignKey(d => d.MarkaKodu);

        }
    }
}
