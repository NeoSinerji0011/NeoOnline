using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AtananIsDokumanlarMap : EntityTypeConfiguration<AtananIsDokumanlar>
    {
        public AtananIsDokumanlarMap()
        {
            // Primary Key
            this.HasKey(t => new { t.IsId,t.SiraNo });

            // Properties
            this.Property(t => t.DokumanURL)
                .IsRequired()
                .HasMaxLength(500);
            this.Property(t => t.DokumanAdi)
                .IsRequired()
                .HasMaxLength(25);
            // Table & Column Mappings
            this.ToTable("AtananIsDokumanlar");
            this.Property(t => t.IsId).HasColumnName("IsId");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.DokumanURL).HasColumnName("DokumanURL");
            this.Property(t => t.DokumanAdi).HasColumnName("DokumanAdi");
            this.Property(t => t.EklemeTarihi).HasColumnName("EklemeTarihi");
            this.Property(t => t.EkleyenPersonelKodu).HasColumnName("EkleyenPersonelKodu");
            
        }
    }
}
