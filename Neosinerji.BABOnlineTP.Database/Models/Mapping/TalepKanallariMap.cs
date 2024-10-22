using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TalepKanallariMap : EntityTypeConfiguration<TalepKanallari>
    {
        public TalepKanallariMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Kodu });

            this.Property(t => t.Adi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TalepKanallari");
            this.Property(t => t.Kodu).HasColumnName("Kodu");
            this.Property(t => t.Adi).HasColumnName("Adi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
           
    }
}
