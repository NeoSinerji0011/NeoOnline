using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_IlceYuzolcumMap : EntityTypeConfiguration<YZ_IlceYuzolcum>
    {
        public YZ_IlceYuzolcumMap()
        {
            this.ToTable("YZ_IlceYuzolcum");
            this.HasKey(t => t.IlAdi);
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.Yuzolcum).HasColumnName("Yuzolcum");
        }
    }
}
