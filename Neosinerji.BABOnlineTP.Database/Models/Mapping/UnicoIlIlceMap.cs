﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{

    public class UnicoIlIlceMap : EntityTypeConfiguration<UnicoIlIlce>
    {
        public UnicoIlIlceMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.IlKodu)
                .HasMaxLength(10)
                .IsRequired();

            this.Property(t => t.IlAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IlceKodu)
                .HasMaxLength(10)
            .IsRequired();

            this.Property(t => t.IlceAdi)
               .IsRequired()
               .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("UnicoIlIlce");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
        }
    }
}