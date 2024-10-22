using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AtananIslerMap : EntityTypeConfiguration<AtananIsler>
    {
        public AtananIslerMap()
        {
            // Primary Key
            this.HasKey(t => new { t.IsId });

            // Properties
            this.Property(t => t.PoliceNumarasi)
                .HasMaxLength(50);

            this.Property(t => t.SigortaSirketKodu)
                .HasMaxLength(20);

            this.Property(t => t.Aciklama)
                .HasMaxLength(500);
            this.Property(t => t.TalepYapanAcente)
              .HasMaxLength(50);
            this.Property(t => t.GonderenAdiSoyadi)
               .HasMaxLength(100);

            this.Property(t => t.GonderenEmail)
               .HasMaxLength(200);

            this.Property(t => t.GonderenTel)
               .HasMaxLength(20);

            this.Property(t => t.GonderenFax)
            .HasMaxLength(20);

            this.Property(t => t.Baslik)
           .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("AtananIsler");
            this.Property(t => t.IsId).HasColumnName("IsId");
            this.Property(t => t.IsTipi).HasColumnName("IsTipi");
            this.Property(t => t.TalepKanaliKodu).HasColumnName("TalepKanaliKodu");
            this.Property(t => t.IsAlanTVMKodu).HasColumnName("IsAlanTVMKodu");
            this.Property(t => t.IsAlanKullaniciKodu).HasColumnName("IsAlanKullaniciKodu");
            this.Property(t => t.Baslik).HasColumnName("Baslik");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.PoliceNumarasi).HasColumnName("PoliceNumarasi");
            this.Property(t => t.YenilemeNo).HasColumnName("YenilemeNo");
            this.Property(t => t.EkNo).HasColumnName("EkNo");
            this.Property(t => t.SigortaSirketKodu).HasColumnName("SigortaSirketKodu");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.AtamaTarihi).HasColumnName("AtamaTarihi");
            this.Property(t => t.BaslamaTarihi).HasColumnName("BaslamaTarihi");
            this.Property(t => t.TahminiBitisTarihi).HasColumnName("TahminiBitisTarihi");
            this.Property(t => t.TamamlanmaTarihi).HasColumnName("TamamlanmaTarihi");
            this.Property(t => t.TalepYapanAcente).HasColumnName("TalepYapanAcente");
            this.Property(t => t.GonderenTCVKN).HasColumnName("GonderenTCVKN");
            this.Property(t => t.GonderenAdiSoyadi).HasColumnName("GonderenAdiSoyadi");
            this.Property(t => t.GonderenEmail).HasColumnName("GonderenEmail");
            this.Property(t => t.GonderenTel).HasColumnName("GonderenTel");
            this.Property(t => t.GonderenFax).HasColumnName("GonderenFax");
            this.Property(t => t.OncelikSeviyesi).HasColumnName("OncelikSeviyesi");
            this.Property(t => t.IsAtayanTVMKodu).HasColumnName("IsAtayanTVMKodu");
            this.Property(t => t.IsAtayanKullaniciKodu).HasColumnName("IsAtayanKullaniciKodu");
            this.Property(t => t.EvrakNo).HasColumnName("EvrakNo");
        }
    }
}
