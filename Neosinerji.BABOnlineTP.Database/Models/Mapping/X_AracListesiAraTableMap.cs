using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class X_AracListesiAraTableMap : EntityTypeConfiguration<X_AracListesiAraTable>
    {
        public X_AracListesiAraTableMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MarkaKodu)
                .HasMaxLength(10);

            this.Property(t => t.TipKodu)
                .HasMaxLength(10);

            this.Property(t => t.MarkaAdi)
                .HasMaxLength(50);

            this.Property(t => t.TipAdi)
                .HasMaxLength(100);

            this.Property(t => t.Yil2017)
                .HasMaxLength(50);

            this.Property(t => t.Yil2016)
                .HasMaxLength(50);

            this.Property(t => t.Yil2015)
                .HasMaxLength(50);

            this.Property(t => t.Yil2014)
                .HasMaxLength(50);

            this.Property(t => t.Yil2013)
                .HasMaxLength(50);

            this.Property(t => t.Yil2012)
                .HasMaxLength(50);

            this.Property(t => t.Yil2011)
                .HasMaxLength(50);

            this.Property(t => t.Yil2010)
                .HasMaxLength(50);

            this.Property(t => t.Yil2009)
                .HasMaxLength(50);

            this.Property(t => t.Yil2008)
                .HasMaxLength(50);

            this.Property(t => t.Yil2007)
                .HasMaxLength(50);

            this.Property(t => t.Yil2006)
                .HasMaxLength(50);

            this.Property(t => t.Yil2005)
                .HasMaxLength(50);

            this.Property(t => t.Yil2004)
                .HasMaxLength(50);

            this.Property(t => t.Yil2003)
                .HasMaxLength(50);

            this.Property(t => t.Yil2002)
                .HasMaxLength(50);

            this.Property(t => t.Yil2001)
                .HasMaxLength(50);

            this.Property(t => t.Yil2000)
                .HasMaxLength(50);

            this.Property(t => t.Yil1999)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("X_AracListesiAraTable");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.MarkaAdi).HasColumnName("MarkaAdi");
            this.Property(t => t.TipAdi).HasColumnName("TipAdi");
            this.Property(t => t.Yil2017).HasColumnName("Yil2017");
            this.Property(t => t.Yil2016).HasColumnName("Yil2016");
            this.Property(t => t.Yil2015).HasColumnName("Yil2015");
            this.Property(t => t.Yil2014).HasColumnName("Yil2014");
            this.Property(t => t.Yil2013).HasColumnName("Yil2013");
            this.Property(t => t.Yil2012).HasColumnName("Yil2012");
            this.Property(t => t.Yil2011).HasColumnName("Yil2011");
            this.Property(t => t.Yil2010).HasColumnName("Yil2010");
            this.Property(t => t.Yil2009).HasColumnName("Yil2009");
            this.Property(t => t.Yil2008).HasColumnName("Yil2008");
            this.Property(t => t.Yil2007).HasColumnName("Yil2007");
            this.Property(t => t.Yil2006).HasColumnName("Yil2006");
            this.Property(t => t.Yil2005).HasColumnName("Yil2005");
            this.Property(t => t.Yil2004).HasColumnName("Yil2004");
            this.Property(t => t.Yil2003).HasColumnName("Yil2003");
            this.Property(t => t.Yil2002).HasColumnName("Yil2002");
            this.Property(t => t.Yil2001).HasColumnName("Yil2001");
            this.Property(t => t.Yil2000).HasColumnName("Yil2000");
            this.Property(t => t.Yil1999).HasColumnName("Yil1999");
        }
    }
}
