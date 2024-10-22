using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceUretimHedefPlanlananMap : EntityTypeConfiguration<PoliceUretimHedefPlanlanan>
    {
        public PoliceUretimHedefPlanlananMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("PoliceUretimHedefPlanlanan");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMKoduTali).HasColumnName("TVMKoduTali");
            this.Property(t => t.Donem).HasColumnName("Donem");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.PoliceAdedi1).HasColumnName("PoliceAdedi1");
            this.Property(t => t.Prim1).HasColumnName("Prim1");
            this.Property(t => t.PoliceAdedi2).HasColumnName("PoliceAdedi2");
            this.Property(t => t.Prim2).HasColumnName("Prim2");
            this.Property(t => t.PoliceAdedi3).HasColumnName("PoliceAdedi3");
            this.Property(t => t.Prim3).HasColumnName("Prim3");
            this.Property(t => t.PoliceAdedi4).HasColumnName("PoliceAdedi4");
            this.Property(t => t.Prim4).HasColumnName("Prim4");
            this.Property(t => t.PoliceAdedi5).HasColumnName("PoliceAdedi5");
            this.Property(t => t.Prim5).HasColumnName("Prim5");
            this.Property(t => t.PoliceAdedi6).HasColumnName("PoliceAdedi6");
            this.Property(t => t.Prim6).HasColumnName("Prim6");
            this.Property(t => t.PoliceAdedi7).HasColumnName("PoliceAdedi7");
            this.Property(t => t.Prim7).HasColumnName("Prim7");
            this.Property(t => t.PoliceAdedi8).HasColumnName("PoliceAdedi8");
            this.Property(t => t.Prim8).HasColumnName("Prim8");
            this.Property(t => t.PoliceAdedi9).HasColumnName("PoliceAdedi9");
            this.Property(t => t.Prim9).HasColumnName("Prim9");
            this.Property(t => t.PoliceAdedi10).HasColumnName("PoliceAdedi10");
            this.Property(t => t.Prim10).HasColumnName("Prim10");
            this.Property(t => t.PoliceAdedi11).HasColumnName("PoliceAdedi11");
            this.Property(t => t.Prim11).HasColumnName("Prim11");
            this.Property(t => t.PoliceAdedi12).HasColumnName("PoliceAdedi12");
            this.Property(t => t.Prim12).HasColumnName("Prim12");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
        }
    }
}
