using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceUretimHedefGerceklesenMap : EntityTypeConfiguration<PoliceUretimHedefGerceklesen>
    {
        public PoliceUretimHedefGerceklesenMap()
        {
            // Primary Key
            this.HasKey(t => t.GerceklesenId);

            // Properties
            // Table & Column Mappings
            this.ToTable("PoliceUretimHedefGerceklesen");
            this.Property(t => t.GerceklesenId).HasColumnName("GerceklesenId");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMKoduTali).HasColumnName("TVMKoduTali");
            this.Property(t => t.Donem).HasColumnName("Donem");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.PoliceAdedi1).HasColumnName("PoliceAdedi1");
            this.Property(t => t.Prim1).HasColumnName("Prim1");
            this.Property(t => t.PoliceKomisyonTutari1).HasColumnName("PoliceKomisyonTutari1");
            this.Property(t => t.VerilenKomisyonTutari1).HasColumnName("VerilenKomisyonTutari1");
            this.Property(t => t.PoliceAdedi2).HasColumnName("PoliceAdedi2");
            this.Property(t => t.Prim2).HasColumnName("Prim2");
            this.Property(t => t.PoliceKomisyonTutari2).HasColumnName("PoliceKomisyonTutari2");
            this.Property(t => t.VerilenKomisyonTutari2).HasColumnName("VerilenKomisyonTutari2");
            this.Property(t => t.PoliceAdedi3).HasColumnName("PoliceAdedi3");
            this.Property(t => t.Prim3).HasColumnName("Prim3");
            this.Property(t => t.PoliceKomisyonTutari3).HasColumnName("PoliceKomisyonTutari3");
            this.Property(t => t.VerilenKomisyonTutari3).HasColumnName("VerilenKomisyonTutari3");
            this.Property(t => t.PoliceAdedi4).HasColumnName("PoliceAdedi4");
            this.Property(t => t.Prim4).HasColumnName("Prim4");
            this.Property(t => t.PoliceKomisyonTutari4).HasColumnName("PoliceKomisyonTutari4");
            this.Property(t => t.VerilenKomisyonTutari4).HasColumnName("VerilenKomisyonTutari4");
            this.Property(t => t.PoliceAdedi5).HasColumnName("PoliceAdedi5");
            this.Property(t => t.Prim5).HasColumnName("Prim5");
            this.Property(t => t.PoliceKomisyonTutari5).HasColumnName("PoliceKomisyonTutari5");
            this.Property(t => t.VerilenKomisyonTutari5).HasColumnName("VerilenKomisyonTutari5");
            this.Property(t => t.PoliceAdedi6).HasColumnName("PoliceAdedi6");
            this.Property(t => t.Prim6).HasColumnName("Prim6");
            this.Property(t => t.PoliceKomisyonTutari6).HasColumnName("PoliceKomisyonTutari6");
            this.Property(t => t.VerilenKomisyonTutari6).HasColumnName("VerilenKomisyonTutari6");
            this.Property(t => t.PoliceAdedi7).HasColumnName("PoliceAdedi7");
            this.Property(t => t.Prim7).HasColumnName("Prim7");
            this.Property(t => t.PoliceKomisyonTutari7).HasColumnName("PoliceKomisyonTutari7");
            this.Property(t => t.VerilenKomisyonTutari7).HasColumnName("VerilenKomisyonTutari7");
            this.Property(t => t.PoliceAdedi8).HasColumnName("PoliceAdedi8");
            this.Property(t => t.Prim8).HasColumnName("Prim8");
            this.Property(t => t.PoliceKomisyonTutari8).HasColumnName("PoliceKomisyonTutari8");
            this.Property(t => t.VerilenKomisyonTutari8).HasColumnName("VerilenKomisyonTutari8");
            this.Property(t => t.PoliceAdedi9).HasColumnName("PoliceAdedi9");
            this.Property(t => t.Prim9).HasColumnName("Prim9");
            this.Property(t => t.PoliceKomisyonTutari9).HasColumnName("PoliceKomisyonTutari9");
            this.Property(t => t.VerilenKomisyonTutari9).HasColumnName("VerilenKomisyonTutari9");
            this.Property(t => t.PoliceAdedi10).HasColumnName("PoliceAdedi10");
            this.Property(t => t.Prim10).HasColumnName("Prim10");
            this.Property(t => t.PoliceKomisyonTutari10).HasColumnName("PoliceKomisyonTutari10");
            this.Property(t => t.VerilenKomisyonTutari10).HasColumnName("VerilenKomisyonTutari10");
            this.Property(t => t.PoliceAdedi11).HasColumnName("PoliceAdedi11");
            this.Property(t => t.Prim11).HasColumnName("Prim11");
            this.Property(t => t.PoliceKomisyonTutari11).HasColumnName("PoliceKomisyonTutari11");
            this.Property(t => t.VerilenKomisyonTutari11).HasColumnName("VerilenKomisyonTutari11");
            this.Property(t => t.PoliceAdedi12).HasColumnName("PoliceAdedi12");
            this.Property(t => t.Prim12).HasColumnName("Prim12");
            this.Property(t => t.PoliceKomisyonTutari12).HasColumnName("PoliceKomisyonTutari12");
            this.Property(t => t.VerilenKomisyonTutari12).HasColumnName("VerilenKomisyonTutari12");
        }
    }
}
