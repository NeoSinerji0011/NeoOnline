using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifNoMapfreMap : EntityTypeConfiguration<TeklifNoMapfre>
    {
        public TeklifNoMapfreMap()
        {
            // Primary Key
            this.HasKey(t => t.TeklifNo);

            // Properties
            // Table & Column Mappings
            this.ToTable("TeklifNoMapfre");
            this.Property(t => t.TeklifNo).HasColumnName("TeklifNo");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
        }
    }
}
