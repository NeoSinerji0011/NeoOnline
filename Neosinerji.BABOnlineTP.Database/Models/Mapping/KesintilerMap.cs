using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class KesintilerMap : EntityTypeConfiguration<Kesintiler>
    {
        public KesintilerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Kesintiler");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMKoduTali).HasColumnName("TVMKoduTali");
            this.Property(t => t.Donem).HasColumnName("Donem");
            this.Property(t => t.KesintiKodu).HasColumnName("KesintiKodu");
            this.Property(t => t.Borc1).HasColumnName("Borc1");
            this.Property(t => t.Alacak1).HasColumnName("Alacak1");
            this.Property(t => t.Borc2).HasColumnName("Borc2");
            this.Property(t => t.Alacak2).HasColumnName("Alacak2");
            this.Property(t => t.Borc3).HasColumnName("Borc3");
            this.Property(t => t.Alacak3).HasColumnName("Alacak3");
            this.Property(t => t.Borc4).HasColumnName("Borc4");
            this.Property(t => t.Alacak4).HasColumnName("Alacak4");
            this.Property(t => t.Borc5).HasColumnName("Borc5");
            this.Property(t => t.Alacak5).HasColumnName("Alacak5");
            this.Property(t => t.Borc6).HasColumnName("Borc6");
            this.Property(t => t.Alacak6).HasColumnName("Alacak6");
            this.Property(t => t.Borc7).HasColumnName("Borc7");
            this.Property(t => t.Alacak7).HasColumnName("Alacak7");
            this.Property(t => t.Borc8).HasColumnName("Borc8");
            this.Property(t => t.Alacak8).HasColumnName("Alacak8");
            this.Property(t => t.Borc9).HasColumnName("Borc9");
            this.Property(t => t.Alacak9).HasColumnName("Alacak9");
            this.Property(t => t.Borc10).HasColumnName("Borc10");
            this.Property(t => t.Alacak10).HasColumnName("Alacak10");
            this.Property(t => t.Borc11).HasColumnName("Borc11");
            this.Property(t => t.Alacak11).HasColumnName("Alacak11");
            this.Property(t => t.Borc12).HasColumnName("Borc12");
            this.Property(t => t.Alacak12).HasColumnName("Alacak12");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");

            // Relationships
            this.HasRequired(t => t.KesintiTurleri)
                .WithMany(t => t.Kesintilers)
                .HasForeignKey(d => d.KesintiKodu);

        }
    }
}
