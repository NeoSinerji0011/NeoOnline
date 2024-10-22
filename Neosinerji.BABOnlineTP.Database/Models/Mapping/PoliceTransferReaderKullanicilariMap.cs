using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceTransferReaderKullanicilariMap : EntityTypeConfiguration<PoliceTransferReaderKullanicilari>
    {
          public PoliceTransferReaderKullanicilariMap()
    {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SigortaSirketKodu)
             .HasMaxLength(5);

            this.Property(t => t.ReaderKodu)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PoliceTransferReaderKullanicilari");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SigortaSirketKodu).HasColumnName("SigortaSirketKodu");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.AltTvmKodu).HasColumnName("AltTvmKodu");
            this.Property(t => t.ReaderKodu).HasColumnName("ReaderKodu");
            this.Property(t => t.Durum).HasColumnName("Durum");
            
        }
}
}
