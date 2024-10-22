using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HDIKaskoHukuksalKorumaBedelleriMap : EntityTypeConfiguration<HDIKaskoHukuksalKorumaBedelleri>
    {
        public HDIKaskoHukuksalKorumaBedelleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.KullanimTarziKodu)
                .HasMaxLength(5);

            this.Property(t => t.Kod2)
                .HasMaxLength(5);

            this.Property(t => t.Kademe)
                .HasMaxLength(5);

            this.Property(t => t.MotorluAracBedeli)
                .HasPrecision(10,2);

            this.Property(t => t.SurucuyeBagliBedeli)
                .HasPrecision(10, 2);
            
                 this.Property(t => t.Aciklama)
                .HasMaxLength(100);

            
            // Table & Column Mappings
            this.ToTable("HDIKaskoHukuksalKorumaBedelleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.Kod2).HasColumnName("Kod2");
            this.Property(t => t.MotorluAracBedeli).HasColumnName("MotorluAracBedeli");
            this.Property(t => t.SurucuyeBagliBedeli).HasColumnName("SurucuyeBagliBedeli");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}
