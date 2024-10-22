using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_IlceNufusMap : EntityTypeConfiguration<YZ_IlceNufus>
    {
        public YZ_IlceNufusMap()
        {
            this.ToTable("YZ_IlceNufus");
            this.HasKey(t => t.Plaka);
            this.Property(t => t.Plaka).HasColumnName("Plaka");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.ToplamNufus).HasColumnName("ToplamNufus");
            this.Property(t => t.ErkekNufus).HasColumnName("ErkekNufus");
            this.Property(t => t.KadinNufus).HasColumnName("KadinNufus");
            this.Property(t => t.Yuzolcumu).HasColumnName("Yuzolcumu");
            this.Property(t => t.NufusYogunluk).HasColumnName("NufusYogunluk");
            this.Property(t => t.IlceEnlem).HasColumnName("IlceEnlem");
            this.Property(t => t.IlceBoylam).HasColumnName("IlceBoylam");
            this.Property(t => t.IlEnlem).HasColumnName("IlEnlem");
            this.Property(t => t.IlBoylam).HasColumnName("IlBoylam");
            this.Property(t => t.IleUzaklik).HasColumnName("IleUzaklik");

        }
    }
}
