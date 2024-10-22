using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifDokumanMap : EntityTypeConfiguration<TeklifDokuman>
    {
        public TeklifDokumanMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.DokumanURL)
                .HasMaxLength(4000);
            this.Property(t => t.DokumanAdi)
                .HasMaxLength(200);


            // Table & Column Mappings
            this.ToTable("TeklifDokuman");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.DokumanAdi).HasColumnName("DokumanAdi");
            this.Property(t => t.DokumanTipi).HasColumnName("DokumanTipi");
            this.Property(t => t.DokumanURL).HasColumnName("DokumanURL");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");

            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifDokumans)
                .HasForeignKey(d => d.TeklifId);
        }
    }
}
