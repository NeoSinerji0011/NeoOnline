using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MeslekIndirimiKaskoMap : EntityTypeConfiguration<MeslekIndirimiKasko>
    {
        public MeslekIndirimiKaskoMap()
        {
            // Primary Key
            this.HasKey(t => t.MeslekKodu);

            // Properties
            this.Property(t => t.MeslekKodu)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("MeslekIndirimiKasko");
            this.Property(t => t.MeslekKodu).HasColumnName("MeslekKodu");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}
