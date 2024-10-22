﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class YZ_IlceKoordinatMap : EntityTypeConfiguration<YZ_IlceKoordinat>
    {
        public YZ_IlceKoordinatMap()
        {
            this.ToTable("YZ_IlceKoordinat");
            this.HasKey(t => t.IlAdi);
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.Enlem).HasColumnName("Enlem");
            this.Property(t => t.Boylam).HasColumnName("Boylam");
        }
    }
}