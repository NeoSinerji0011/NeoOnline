using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class LilyumKartTeminatlarMap : EntityTypeConfiguration<LilyumKartTeminatlar>
    {
        public LilyumKartTeminatlarMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TeminatId });

            // Properties
            this.Property(t => t.GrupAdi)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(t => t.TeminatAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Limit)
                .HasPrecision(10,2);

            this.Property(t => t.Aciklama)
                .HasMaxLength(500);          

            // Table & Column Mappings
            this.ToTable("LilyumKartTeminatlar");
            this.Property(t => t.TeminatId).HasColumnName("TeminatId");
            this.Property(t => t.GrupAdi).HasColumnName("GrupAdi");
            this.Property(t => t.TeminatAdi).HasColumnName("TeminatAdi");
            this.Property(t => t.Limit).HasColumnName("Limit");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.KullanimHakki).HasColumnName("KullanimHakki");
                    
        }
    }
}
