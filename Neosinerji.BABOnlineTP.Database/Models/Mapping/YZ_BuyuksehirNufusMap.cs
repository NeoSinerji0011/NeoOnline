using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{

    public class YZ_BuyuksehirNufusMap : EntityTypeConfiguration<YZ_BuyuksehirNufus>
    {
        public YZ_BuyuksehirNufusMap()
        {
            this.ToTable("YZ_BuyuksehirNufus");
            this.HasKey(t => t.Plaka);
            this.Property(t => t.Plaka).HasColumnName("Plaka");
            this.Property(t => t.Buyuksehir).HasColumnName("Buyuksehir");
            this.Property(t => t.ToplamNufus).HasColumnName("ToplamNufus");
            this.Property(t => t.ErkekNufus).HasColumnName("ErkekNufus");
            this.Property(t => t.KadinNufus).HasColumnName("KadinNufus");
        }
    }
}
