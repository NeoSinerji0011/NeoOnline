using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_IlNufusMap : EntityTypeConfiguration<YZ_IlNufus>
    {
        public YZ_IlNufusMap()
        {
            this.ToTable("YZ_IlNufus");
            this.HasKey(t => t.Plaka);
            this.Property(t => t.Plaka).HasColumnName("Plaka");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.ToplamNufus).HasColumnName("ToplamNufus");
            this.Property(t => t.ErkekNufus).HasColumnName("ErkekNufus");
            this.Property(t => t.KadinNufus).HasColumnName("KadinNufus");
            this.Property(t => t.Enlem).HasColumnName("Enlem");
            this.Property(t => t.Boylam).HasColumnName("Boylam");
            this.Property(t => t.Yuzolcum).HasColumnName("Yuzolcum");
            this.Property(t => t.NufusYogunluk).HasColumnName("NufusYogunluk");
        }
    }
}
