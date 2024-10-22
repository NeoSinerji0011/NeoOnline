using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifNoSayacMap : EntityTypeConfiguration<TeklifNoSayac>
    {
        public TeklifNoSayacMap()
        {
            // Primary Key
            this.HasKey(t => t.TVMKodu);

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("TeklifNoSayac");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TeklifNo).HasColumnName("TeklifNo");
        }
    }
}
