using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_KoyKoordinatMap : EntityTypeConfiguration<YZ_KoyKoordinat>
    {
        public YZ_KoyKoordinatMap()
        {
            this.ToTable("YZ_KoyKoordinat");
            this.HasKey(t => t.KoyAdi);
            this.Property(t => t.KoyAdi).HasColumnName("KoyAdi");
            this.Property(t => t.MahalleAdi).HasColumnName("MahalleAdi");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.Tip).HasColumnName("Tip");
            this.Property(t => t.Enlem).HasColumnName("Enlem");
            this.Property(t => t.Boylam).HasColumnName("Boylam");
        }
    }
}
