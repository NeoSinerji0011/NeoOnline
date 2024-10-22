using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    
    public class CariOdemeTipleriMap : EntityTypeConfiguration<CariOdemeTipleri>
    {
        public CariOdemeTipleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Kodu);

            // Properties
            this.Property(t => t.Aciklama)
                .HasMaxLength(100);
                     

            // Table & Column Mappings
            this.ToTable("CariOdemeTipleri");
            this.Property(t => t.Kodu).HasColumnName("Kodu");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
