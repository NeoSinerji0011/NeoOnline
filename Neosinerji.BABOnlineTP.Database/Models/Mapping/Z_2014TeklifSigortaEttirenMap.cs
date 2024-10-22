using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_2014TeklifSigortaEttirenMap : EntityTypeConfiguration<Z_2014TeklifSigortaEttiren>
    {
        public Z_2014TeklifSigortaEttirenMap()
        {
            // Primary Key
            this.HasKey(t => t.TableSigortaEttirenId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Z_2014TeklifSigortaEttiren");
            this.Property(t => t.TableSigortaEttirenId).HasColumnName("TableSigortaEttirenId");
            this.Property(t => t.SigortaEttirenId).HasColumnName("SigortaEttirenId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
        }
    }
}
