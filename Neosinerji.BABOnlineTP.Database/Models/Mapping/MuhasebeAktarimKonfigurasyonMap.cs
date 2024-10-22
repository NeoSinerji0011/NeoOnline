using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MuhasebeAktarimKonfigurasyonMap : EntityTypeConfiguration<MuhasebeAktarimKonfigurasyon>
    {
        public MuhasebeAktarimKonfigurasyonMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id });          

            this.Property(t => t.TvmUnvani)
                .HasMaxLength(500);

            this.Property(t => t.SirketKodu)
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("MuhasebeAktarimKonfigurasyon");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.TvmUnvani).HasColumnName("TvmUnvani");
            this.Property(t => t.BaslangicTarihi).HasColumnName("BaslangicTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.SirketKodu).HasColumnName("SirketKodu");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.AktarimTamamlandi).HasColumnName("AktarimTamamlandi");
            this.Property(t => t.AktarimYuzdesi).HasColumnName("AktarimYuzdesi");
        }
    }
}
