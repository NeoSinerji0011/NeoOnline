using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_MeslekIndirimiKaskoMap : EntityTypeConfiguration<CR_MeslekIndirimiKasko>
    {
        public CR_MeslekIndirimiKaskoMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.MeslekKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.MeslekKodu)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.CR_MeslekKodu)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.CR_Aciklama)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("CR_MeslekIndirimiKasko");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.MeslekKodu).HasColumnName("MeslekKodu");
            this.Property(t => t.CR_MeslekKodu).HasColumnName("CR_MeslekKodu");
            this.Property(t => t.CR_Aciklama).HasColumnName("CR_Aciklama");
        }
    }
}
