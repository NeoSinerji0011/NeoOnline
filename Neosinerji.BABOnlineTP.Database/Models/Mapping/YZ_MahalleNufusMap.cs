using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_MahalleNufusMap : EntityTypeConfiguration<YZ_MahalleNufus>
    {
        public YZ_MahalleNufusMap()
        {
            this.ToTable("YZ_MahalleNufus");
            this.HasKey(t => t.Plaka);
            this.Property(t => t.Plaka).HasColumnName("Plaka");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.BeldeKoyKodu).HasColumnName("BeldeKoyKodu");
            this.Property(t => t.MahalleKodu).HasColumnName("MahalleKodu");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.BeldeKoyAdi).HasColumnName("BeldeKoyAdi");
            this.Property(t => t.BelediyeAdi).HasColumnName("BelediyeAdi");
            this.Property(t => t.MahalleAdi).HasColumnName("MahalleAdi");
            this.Property(t => t.Nitelik).HasColumnName("Nitelik");
            this.Property(t => t.ToplamNufus).HasColumnName("ToplamNufus");
            this.Property(t => t.ErkekNufus).HasColumnName("ErkekNufus");
            this.Property(t => t.KadinNufus).HasColumnName("KadinNufus");
            this.Property(t => t.IlAdi2).HasColumnName("IlAdi2");
            this.Property(t => t.IlceAdi2).HasColumnName("IlceAdi2");
            this.Property(t => t.MahalleAdi2).HasColumnName("MahalleAdi2");
            this.Property(t => t.MahEnlem).HasColumnName("MahEnlem");
            this.Property(t => t.MahBoylam).HasColumnName("MahBoylam");
            this.Property(t => t.IlceEnlem).HasColumnName("IlceEnlem");
            this.Property(t => t.IlEnlem).HasColumnName("IlEnlem");
            this.Property(t => t.IlBoylam).HasColumnName("IlBoylam");
            this.Property(t => t.IleUzaklik).HasColumnName("IleUzaklik");
            this.Property(t => t.IlceyeUzaklik).HasColumnName("IlceyeUzaklik");

        }
    }
}
