using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_2014TeklifTeminatMap : EntityTypeConfiguration<Z_2014TeklifTeminat>
    {
        public Z_2014TeklifTeminatMap()
        {
            // Primary Key
            this.HasKey(t => t.TableId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Z_2014TeklifTeminat");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.TeminatKodu).HasColumnName("TeminatKodu");
            this.Property(t => t.TeminatBedeli).HasColumnName("TeminatBedeli");
            this.Property(t => t.TeminatVergi).HasColumnName("TeminatVergi");
            this.Property(t => t.TeminatNetPrim).HasColumnName("TeminatNetPrim");
            this.Property(t => t.TeminatBrutPrim).HasColumnName("TeminatBrutPrim");
            this.Property(t => t.Komisyon_).HasColumnName("Komisyon ");
            this.Property(t => t.Adet).HasColumnName("Adet");
        }
    }
}
