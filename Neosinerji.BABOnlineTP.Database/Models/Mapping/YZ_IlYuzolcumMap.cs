using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_IlYuzolcumMap : EntityTypeConfiguration<YZ_IlYuzolcum>
    {
        public YZ_IlYuzolcumMap()
        {
            this.ToTable("YZ_IlYuzolcum");
            this.HasKey(t => t.IlAdi);
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.Yuzolcum).HasColumnName("Yuzolcum");
        }
    }
}
