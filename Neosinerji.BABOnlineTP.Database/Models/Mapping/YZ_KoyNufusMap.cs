using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_KoyNufusMap : EntityTypeConfiguration<YZ_KoyNufus>
    {
        public YZ_KoyNufusMap()
        {
            this.ToTable("YZ_KoyNufus");
            this.HasKey(t => t.Plaka);
            this.Property(t => t.Plaka).HasColumnName("Plaka");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.KoyAdi).HasColumnName("KoyAdi");
            this.Property(t => t.ToplamNufus).HasColumnName("ToplamNufus");
            this.Property(t => t.ErkekNufus).HasColumnName("ErkekNufus");
            this.Property(t => t.KadinNufus).HasColumnName("KadinNufus");
        }
    }
   
}
