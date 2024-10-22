using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class ErgoIlIlceMap : EntityTypeConfiguration<ErgoIlIlce>
    {
        public ErgoIlIlceMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.IlKodu)
                .IsRequired();

            this.Property(t => t.IlAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IlceKodu)
            .IsRequired();

            this.Property(t => t.IlceAdi)
               .IsRequired()
               .HasMaxLength(50);

            this.Property(t => t.TramerDistrictId)
            .IsRequired();

            // Table & Column Mappings
            this.ToTable("ErgoIlIlce");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.TramerDistrictId).HasColumnName("TramerDistrictId");
        }
    }
}
