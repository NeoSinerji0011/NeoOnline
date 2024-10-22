using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AtananIsNotlarMap : EntityTypeConfiguration<AtananIsNotlar>
    {
        public AtananIsNotlarMap()
        {
            // Primary Key
            this.HasKey(t => new { t.NotId });

            // Properties
            this.Property(t => t.IsId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);            

            this.Property(t => t.Konu)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.NotAciklamasi)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("AtananIsNotlar");
            this.Property(t => t.NotId).HasColumnName("NotId");
            this.Property(t => t.IsId).HasColumnName("IsId");
            this.Property(t => t.Konu).HasColumnName("Konu");
            this.Property(t => t.NotAciklamasi).HasColumnName("NotAciklamasi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMPersonelKodu).HasColumnName("TVMPersonelKodu");

            // Relationships
            this.HasRequired(t => t.AtananIsler)
                .WithMany(t => t.AtananIsNots)
                .HasForeignKey(d => d.IsId);

        }
    }
}
