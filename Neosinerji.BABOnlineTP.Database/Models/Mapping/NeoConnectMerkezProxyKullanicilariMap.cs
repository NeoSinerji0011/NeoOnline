﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    class NeoConnectMerkezProxyKullanicilariMap : EntityTypeConfiguration<NeoConnectMerkezProxyKullanicilari>
    {
        public NeoConnectMerkezProxyKullanicilariMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SigortaSirketKodu)
             .HasMaxLength(5);
      
            // Table & Column Mappings
            this.ToTable("NeoConnectMerkezProxyKullanicilari");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SigortaSirketKodu).HasColumnName("SigortaSirketKodu");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu");

        }
    }
}
