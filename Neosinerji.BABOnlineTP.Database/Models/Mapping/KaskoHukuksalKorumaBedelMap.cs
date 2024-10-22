using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
   public class KaskoHukuksalKorumaBedelMap : EntityTypeConfiguration<KaskoHukuksalKorumaBedel>
    {
        public KaskoHukuksalKorumaBedelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Text)
                .IsRequired()
                .HasMaxLength(200);
            

            // Table & Column Mappings
            this.ToTable("KaskoHukuksalKorumaBedel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Bedel).HasColumnName("Bedel").HasPrecision(12,2);
            this.Property(t => t.Text).HasColumnName("Text");
        }
    }
}
