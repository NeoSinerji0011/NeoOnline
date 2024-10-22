using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class X_XMLAraTableMap : EntityTypeConfiguration<X_XMLAraTable>
    {
        public X_XMLAraTableMap()
        {
            // Primary Key
            this.HasKey(t => t.L);

            // Properties
            this.Property(t => t.L)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AK)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.MK)
                .HasMaxLength(15);

            this.Property(t => t.MA)
                .HasMaxLength(50);

            this.Property(t => t.AM)
                .HasMaxLength(100);

            this.Property(t => t.SM)
                .HasMaxLength(15);

            this.Property(t => t.MG)
                .HasMaxLength(15);

            this.Property(t => t.NA)
                .HasMaxLength(15);

            this.Property(t => t.KS)
                .HasMaxLength(15);

            this.Property(t => t.T1)
                .HasMaxLength(15);

            this.Property(t => t.T2)
                .HasMaxLength(15);

            this.Property(t => t.T3)
                .HasMaxLength(15);

            this.Property(t => t.T4)
                .HasMaxLength(15);

            this.Property(t => t.T5)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("X_XMLAraTable");
            this.Property(t => t.L).HasColumnName("L");
            this.Property(t => t.AK).HasColumnName("AK");
            this.Property(t => t.MK).HasColumnName("MK");
            this.Property(t => t.MA).HasColumnName("MA");
            this.Property(t => t.AM).HasColumnName("AM");
            this.Property(t => t.SM).HasColumnName("SM");
            this.Property(t => t.MG).HasColumnName("MG");
            this.Property(t => t.NA).HasColumnName("NA");
            this.Property(t => t.KS).HasColumnName("KS");
            this.Property(t => t.T1).HasColumnName("T1");
            this.Property(t => t.T2).HasColumnName("T2");
            this.Property(t => t.T3).HasColumnName("T3");
            this.Property(t => t.T4).HasColumnName("T4");
            this.Property(t => t.T5).HasColumnName("T5");
        }
    }
}
