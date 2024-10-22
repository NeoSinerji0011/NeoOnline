using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class NeoConnetVersiyonNoMap : EntityTypeConfiguration<NeoConnetVersiyonNo>
    {
        public NeoConnetVersiyonNoMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("NeoConnetVersiyonNo");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.VersiyonNo).HasColumnName("VersiyonNo");
        }
    }
}
