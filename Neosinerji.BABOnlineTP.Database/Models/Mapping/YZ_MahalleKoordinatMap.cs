using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_MahalleKoordinatMap : EntityTypeConfiguration<YZ_MahalleKoordinat>
    {
        public YZ_MahalleKoordinatMap()
        {
            this.ToTable("YZ_MahalleKoordinat");
            this.HasKey(t => t.MahalleAdi);
            this.Property(t => t.MahalleAdi).HasColumnName("MahalleAdi");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.Tip).HasColumnName("Tip");
            this.Property(t => t.Enlem).HasColumnName("Enlem");
            this.Property(t => t.Boylam).HasColumnName("Boylam");
        }
    }
}
