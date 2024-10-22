using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
   public class ParaBirimleriMap : EntityTypeConfiguration<ParaBirimleri>
    {
        public ParaBirimleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Birimi)
               .IsRequired()
               .HasMaxLength(20);

            this.Property(t => t.Adi)
               .IsRequired()
               .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ParaBirimleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Birimi).HasColumnName("Birimi");
            this.Property(t => t.Adi).HasColumnName("Adi");


        }
    }
}
