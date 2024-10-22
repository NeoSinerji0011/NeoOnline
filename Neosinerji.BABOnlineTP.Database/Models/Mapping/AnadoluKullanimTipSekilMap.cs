using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{

    public class AnadoluKullanimTipSekilMap : EntityTypeConfiguration<AnadoluKullanimTipSekil>
    {
        public AnadoluKullanimTipSekilMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.NeoOnlineKullanimTarzi)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.KullanimTipi)
                  .IsRequired()
                  .HasMaxLength(20);

            this.Property(t => t.TipAdi)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.KullanimSekli)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.SekilAdi)
                 .IsRequired()
                 .HasMaxLength(100);

            this.Property(t => t.Durum)
                  .IsRequired();

            // Table & Column Mappings
            this.ToTable("AnadoluKullanimTipSekil");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.NeoOnlineKullanimTarzi).HasColumnName("NeoOnlineKullanimTarzi");
            this.Property(t => t.KullanimTipi).HasColumnName("KullanimTipi");
            this.Property(t => t.TipAdi).HasColumnName("TipAdi");
            this.Property(t => t.KullanimSekli).HasColumnName("KullanimSekli");
            this.Property(t => t.SekilAdi).HasColumnName("SekilAdi");
        }
    }
}
