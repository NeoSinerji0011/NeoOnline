using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class KimlikNoUretMap : EntityTypeConfiguration<KimlikNoUret>
    {
        public KimlikNoUretMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TcknSayac)
                .IsRequired()
                .HasMaxLength(11);

            this.ToTable("KimlikNoUret");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TcknSayac).HasColumnName("TcknSayac");
        }
    }
}
