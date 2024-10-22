using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class NeoConnectTvmSirketYetkileriMap : EntityTypeConfiguration<NeoConnectTvmSirketYetkileri>
    {
        public NeoConnectTvmSirketYetkileriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TumKodu)
                .IsRequired()
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("NeoConnectTvmSirketYetkileri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.TumKodu).HasColumnName("TumKodu");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
