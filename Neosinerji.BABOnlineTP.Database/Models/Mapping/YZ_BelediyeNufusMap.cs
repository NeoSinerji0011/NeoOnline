using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_BelediyeNufusMap : EntityTypeConfiguration<YZ_BelediyeNufus>
    {
        public YZ_BelediyeNufusMap()
        {
            this.ToTable("YZ_BelediyeNufus");
            this.HasKey(t => t.Plaka);
            this.Property(t => t.Plaka).HasColumnName("Plaka");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.BeldeKoyKodu).HasColumnName("BeldeKoyKodu");
            this.Property(t => t.BelediyeKodu).HasColumnName("BelediyeKodu");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.BelediyeAdi).HasColumnName("BelediyeAdi");
            this.Property(t => t.Nitelik).HasColumnName("Nitelik");
            this.Property(t => t.ToplamNufus).HasColumnName("ToplamNufus");
            this.Property(t => t.ErkekNufus).HasColumnName("ErkekNufus");
            this.Property(t => t.KadinNufus).HasColumnName("KadinNufus");
        }
    }
}
