using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
   public class Cr_KaskoHukuksalKorumaMap : EntityTypeConfiguration<Cr_KaskoHukuksalKoruma>
    {
        public Cr_KaskoHukuksalKorumaMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.DegerKodu)
                .IsRequired()
                .HasMaxLength(200);


            // Table & Column Mappings
            this.ToTable("Cr_KaskoHukuksalKoruma");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.DegerKodu).HasColumnName("DegerKodu");
            this.Property(t => t.Bedel).HasColumnName("Bedel").HasPrecision(12, 2);
        }
    }
}
