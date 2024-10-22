using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifNotMap : EntityTypeConfiguration<TeklifNot>
    {
        public TeklifNotMap()
        {
            // Primary Key
            this.HasKey(t => t.TeklifId);

            // Properties
            this.Property(t => t.TeklifId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("TeklifNot");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithOptional(t => t.TeklifNot);

        }
    }
}
