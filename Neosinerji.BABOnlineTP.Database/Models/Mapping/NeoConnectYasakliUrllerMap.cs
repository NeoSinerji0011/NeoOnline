using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class NeoConnectYasakliUrllerMap : EntityTypeConfiguration<NeoConnectYasakliUrller>
    {
        public NeoConnectYasakliUrllerMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.YasaklanacakUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("NeoConnectYasakliUrller");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.AltTvmKodu).HasColumnName("AltTvmKodu");
            this.Property(t => t.SigortaSirketKodu).HasColumnName("SigortaSirketKodu");
            this.Property(t => t.YasaklanacakUrl).HasColumnName("YasaklanacakUrl");
        }
    }
}
