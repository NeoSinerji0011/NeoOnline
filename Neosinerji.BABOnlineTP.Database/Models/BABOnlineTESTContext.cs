using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Neosinerji.BABOnlineTP.Database.Models.Mapping;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class BABOnlineTESTContext : DbContext
    {
        static BABOnlineTESTContext()
        {
            System.Data.Entity.Database.SetInitializer<BABOnlineTESTContext>(null);
        }

        public BABOnlineTESTContext()
            : base("Name=BABOnlineTESTContext")
        {
        }

        public DbSet<AltMenu> AltMenus { get; set; }
        public DbSet<AltMenuSekme> AltMenuSekmes { get; set; }
        public DbSet<AnaMenu> AnaMenus { get; set; }
        public DbSet<AracKullanimSekli> AracKullanimSeklis { get; set; }
        public DbSet<AracKullanimTarzi> AracKullanimTarzis { get; set; }
        public DbSet<AracMarka> AracMarkas { get; set; }
        public DbSet<AracMarkaYedek> AracMarkaYedeks { get; set; }
        public DbSet<AracModel> AracModels { get; set; }
        public DbSet<AracModelYedek> AracModelYedeks { get; set; }
        public DbSet<AracTip> AracTips { get; set; }
        public DbSet<AracTipYedek> AracTipYedeks { get; set; }
        public DbSet<AracTrafikTeminat> AracTrafikTeminats { get; set; }
        public DbSet<AutoPoliceTransfer> AutoPoliceTransfers { get; set; }
        public DbSet<BankaSubeleri> BankaSubeleris { get; set; }
        public DbSet<Belediye> Belediyes { get; set; }
        public DbSet<BelediyeIl> BelediyeIls { get; set; }
        public DbSet<Bran> Brans { get; set; }
        public DbSet<BransUrun> BransUruns { get; set; }
        public DbSet<CR_AracEkSoru> CR_AracEkSoru { get; set; }
        public DbSet<CR_AracGrup> CR_AracGrup { get; set; }
        public DbSet<CR_IlIlce> CR_IlIlce { get; set; }
        public DbSet<CR_KaskoAMS> CR_KaskoAMS { get; set; }
        public DbSet<CR_KaskoDM> CR_KaskoDM { get; set; }
        public DbSet<CR_KaskoFK> CR_KaskoFK { get; set; }
        public DbSet<CR_KaskoIkameTuru> CR_KaskoIkameTuru { get; set; }
        public DbSet<CR_KaskoIMM> CR_KaskoIMM { get; set; }
        public DbSet<CR_KrediHayatCarpan> CR_KrediHayatCarpan { get; set; }
        public DbSet<CR_KullanimTarzi> CR_KullanimTarzi { get; set; }
        public DbSet<CR_MeslekIndirimiKasko> CR_MeslekIndirimiKasko { get; set; }
        public DbSet<CR_TescilIlIlce> CR_TescilIlIlce { get; set; }
        public DbSet<CR_TrafikFK> CR_TrafikFK { get; set; }
        public DbSet<CR_TrafikIMM> CR_TrafikIMM { get; set; }
        public DbSet<CR_TUMMusteri> CR_TUMMusteri { get; set; }
        public DbSet<CR_Ulke> CR_Ulke { get; set; }
        public DbSet<DaskBelde> DaskBeldes { get; set; }
        public DbSet<DaskIl> DaskIls { get; set; }
        public DbSet<DaskIlce> DaskIlces { get; set; }
        public DbSet<DaskKurumlar> DaskKurumlars { get; set; }
        public DbSet<DaskSubeler> DaskSubelers { get; set; }
        public DbSet<DepremMuafiyet> DepremMuafiyets { get; set; }
        public DbSet<DigerUlkeOranlari> DigerUlkeOranlaris { get; set; }
        public DbSet<Dil> Dils { get; set; }
        public DbSet<DilAciklama> DilAciklamas { get; set; }
        public DbSet<DuyuruDokuman> DuyuruDokumen { get; set; }
        public DbSet<Duyurular> Duyurulars { get; set; }
        public DbSet<DuyuruTVM> DuyuruTVMs { get; set; }
        public DbSet<El2Garanti_HesapCetveli> El2Garanti_HesapCetveli { get; set; }
        public DbSet<EPostaFormatlari> EPostaFormatlaris { get; set; }
        public DbSet<GenelTanimlar> GenelTanimlars { get; set; }
        public DbSet<HasarAnlasmaliServisler> HasarAnlasmaliServislers { get; set; }
        public DbSet<HasarAsistansFirmalari> HasarAsistansFirmalaris { get; set; }
        public DbSet<HasarBankaHesaplari> HasarBankaHesaplaris { get; set; }
        public DbSet<HasarEksperIslemleri> HasarEksperIslemleris { get; set; }
        public DbSet<HasarEksperListesi> HasarEksperListesis { get; set; }
        public DbSet<HasarGenelBilgiler> HasarGenelBilgilers { get; set; }
        public DbSet<HasarIletisimYetkilileri> HasarIletisimYetkilileris { get; set; }
        public DbSet<HasarNotlari> HasarNotlaris { get; set; }
        public DbSet<HasarZorunluEvraklari> HasarZorunluEvraklaris { get; set; }
        public DbSet<HasarZorunluEvrakListesi> HasarZorunluEvrakListesis { get; set; }
        public DbSet<Il> Ils { get; set; }
        public DbSet<Ilce> Ilces { get; set; }
        public DbSet<IsDurum> IsDurums { get; set; }
        public DbSet<IsDurumDetay> IsDurumDetays { get; set; }
        public DbSet<IsTakip> IsTakips { get; set; }
        public DbSet<IsTakipDetay> IsTakipDetays { get; set; }
        public DbSet<IsTakipDokuman> IsTakipDokumen { get; set; }
        public DbSet<IsTakipIsTipleri> IsTakipIsTipleris { get; set; }
        public DbSet<IsTakipIsTipleriDetay> IsTakipIsTipleriDetays { get; set; }
        public DbSet<IsTakipKullaniciGrupKullanicilari> IsTakipKullaniciGrupKullanicilaris { get; set; }
        public DbSet<IsTakipKullaniciGruplari> IsTakipKullaniciGruplaris { get; set; }
        public DbSet<IsTakipSoru> IsTakipSorus { get; set; }
        public DbSet<Istigal> Istigals { get; set; }
        public DbSet<KaskoDM> KaskoDMs { get; set; }
        public DbSet<KaskoFK> KaskoFKs { get; set; }
        public DbSet<KaskoIMM> KaskoIMMs { get; set; }
        public DbSet<KaskoTasinanYukKademeleri> KaskoTasinanYukKademeleris { get; set; }
        public DbSet<Kesintiler> Kesintilers { get; set; }
        public DbSet<KesintiTurleri> KesintiTurleris { get; set; }
        public DbSet<Konfigurasyon> Konfigurasyons { get; set; }
        public DbSet<MapfreKullanici> MapfreKullanicis { get; set; }
        public DbSet<MenuIslem> MenuIslems { get; set; }
        public DbSet<Meslek> Mesleks { get; set; }
        public DbSet<MeslekIndirimiKasko> MeslekIndirimiKaskoes { get; set; }
        public DbSet<MusteriAdre> MusteriAdres { get; set; }
        public DbSet<MusteriDokuman> MusteriDokumen { get; set; }
        public DbSet<MusteriGenelBilgiler> MusteriGenelBilgilers { get; set; }
        public DbSet<MusteriNot> MusteriNots { get; set; }
        public DbSet<MusteriTelefon> MusteriTelefons { get; set; }
        public DbSet<NeoConnectLog> NeoConnectLogs { get; set; }
        public DbSet<NeoConnectPoliceDetay> NeoConnectPoliceDetays { get; set; }
        public DbSet<NeoConnectSirketDetay> NeoConnectSirketDetays { get; set; }
        public DbSet<NeoConnectSirketGrupKullaniciDetay> NeoConnectSirketGrupKullaniciDetays { get; set; }
        public DbSet<NeoConnectTvmSirketYetkileri> NeoConnectTvmSirketYetkileris { get; set; }
        public DbSet<NeoConnectYasakliUrller> NeoConnectYasakliUrllers { get; set; }
        public DbSet<OfflinePolouse> OfflinePolice { get; set; }
        public DbSet<OfflinePoliceNumara> OfflinePoliceNumaras { get; set; }
        public DbSet<OtoLoginSigortaSirketKullanicilar> OtoLoginSigortaSirketKullanicilars { get; set; }
        public DbSet<ParaBirimleri> ParaBirimleris { get; set; }
        public DbSet<PaylasimliPoliceUretim> PaylasimliPoliceUretims { get; set; }
        public DbSet<PoliceArac> PoliceAracs { get; set; }
        public DbSet<PoliceGenel> PoliceGenels { get; set; }
        public DbSet<PoliceOdemePlani> PoliceOdemePlanis { get; set; }
        public DbSet<PoliceRizikoAdresi> PoliceRizikoAdresis { get; set; }
        public DbSet<PoliceSigortaEttiren> PoliceSigortaEttirens { get; set; }
        public DbSet<PoliceSigortali> PoliceSigortalis { get; set; }
        public DbSet<PoliceTahsilat> PoliceTahsilats { get; set; }
        public DbSet<PoliceTaliAcenteler> PoliceTaliAcentelers { get; set; }
        public DbSet<PoliceTaliAcenteRapor> PoliceTaliAcenteRapors { get; set; }
        public DbSet<PoliceUretimHedefGerceklesen> PoliceUretimHedefGerceklesens { get; set; }
        public DbSet<PoliceUretimHedefPlanlanan> PoliceUretimHedefPlanlanans { get; set; }
        public DbSet<PoliceVergi> PoliceVergis { get; set; }
        public DbSet<PotansiyelMusteriAdre> PotansiyelMusteriAdres { get; set; }
        public DbSet<PotansiyelMusteriDokuman> PotansiyelMusteriDokumen { get; set; }
        public DbSet<PotansiyelMusteriGenelBilgiler> PotansiyelMusteriGenelBilgilers { get; set; }
        public DbSet<PotansiyelMusteriNot> PotansiyelMusteriNots { get; set; }
        public DbSet<PotansiyelMusteriTelefon> PotansiyelMusteriTelefons { get; set; }
        public DbSet<SaglikBolgeleri> SaglikBolgeleris { get; set; }
        public DbSet<SaglikTeminatPrimleri> SaglikTeminatPrimleris { get; set; }
        public DbSet<SaglikTeminatTipleri> SaglikTeminatTipleris { get; set; }
        public DbSet<SchengenUlkeOranlari> SchengenUlkeOranlaris { get; set; }
        public DbSet<SigortaSirketleri> SigortaSirketleris { get; set; }
        public DbSet<Soru> Sorus { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<TaliAcenteKomisyonOrani> TaliAcenteKomisyonOranis { get; set; }
        public DbSet<TCKN> TCKNs { get; set; }
        public DbSet<TeklifArac> TeklifAracs { get; set; }
        public DbSet<TeklifAracEkSoru> TeklifAracEkSorus { get; set; }
        public DbSet<TeklifDigerSirketler> TeklifDigerSirketlers { get; set; }
        public DbSet<TeklifEMailLog> TeklifEMailLogs { get; set; }
        public DbSet<TeklifGenel> TeklifGenels { get; set; }
        public DbSet<TeklifNoSayac> TeklifNoSayacs { get; set; }
        public DbSet<TeklifNot> TeklifNots { get; set; }
        public DbSet<TeklifOdemePlani> TeklifOdemePlanis { get; set; }
        public DbSet<TeklifProvizyon> TeklifProvizyons { get; set; }
        public DbSet<TeklifRizikoAdresi> TeklifRizikoAdresis { get; set; }
        public DbSet<TeklifSigortaEttiren> TeklifSigortaEttirens { get; set; }
        public DbSet<TeklifSigortali> TeklifSigortalis { get; set; }
        public DbSet<TeklifSoru> TeklifSorus { get; set; }
        public DbSet<TeklifTeminat> TeklifTeminats { get; set; }
        public DbSet<TeklifVergi> TeklifVergis { get; set; }
        public DbSet<TeklifWebServisCevap> TeklifWebServisCevaps { get; set; }
        public DbSet<Teminat> Teminats { get; set; }
        public DbSet<TrafikFK> TrafikFKs { get; set; }
        public DbSet<TrafikIMM> TrafikIMMs { get; set; }
        public DbSet<TUMBankaHesaplari> TUMBankaHesaplaris { get; set; }
        public DbSet<TUMDetay> TUMDetays { get; set; }
        public DbSet<TUMDokumanlar> TUMDokumanlars { get; set; }
        public DbSet<TUMDurumTarihcesi> TUMDurumTarihcesis { get; set; }
        public DbSet<TUMIletisimYetkilileri> TUMIletisimYetkilileris { get; set; }
        public DbSet<TUMIPBaglanti> TUMIPBaglantis { get; set; }
        public DbSet<TUMNotlar> TUMNotlars { get; set; }
        public DbSet<TUMUrunleri> TUMUrunleris { get; set; }
        public DbSet<TVMAcentelikleri> TVMAcentelikleris { get; set; }
        public DbSet<TVMBankaHesaplari> TVMBankaHesaplaris { get; set; }
        public DbSet<TVMBolgeleri> TVMBolgeleris { get; set; }
        public DbSet<TVMDepartmanlar> TVMDepartmanlars { get; set; }
        public DbSet<TVMDetay> TVMDetays { get; set; }
        public DbSet<TVMDokumanlar> TVMDokumanlars { get; set; }
        public DbSet<TVMDurumTarihcesi> TVMDurumTarihcesis { get; set; }
        public DbSet<TVMIletisimYetkilileri> TVMIletisimYetkilileris { get; set; }
        public DbSet<TVMIPBaglanti> TVMIPBaglantis { get; set; }
        public DbSet<TVMKullaniciAtama> TVMKullaniciAtamas { get; set; }
        public DbSet<TVMKullaniciDurumTarihcesi> TVMKullaniciDurumTarihcesis { get; set; }
        public DbSet<TVMKullanicilar> TVMKullanicilars { get; set; }
        public DbSet<TVMKullaniciNotlar> TVMKullaniciNotlars { get; set; }
        public DbSet<TVMKullaniciSifremiUnuttum> TVMKullaniciSifremiUnuttums { get; set; }
        public DbSet<TVMKullaniciSifreTarihcesi> TVMKullaniciSifreTarihcesis { get; set; }
        public DbSet<TVMNotlar> TVMNotlars { get; set; }
        public DbSet<TVMSMSKullaniciBilgi> TVMSMSKullaniciBilgis { get; set; }
        public DbSet<TVMUrunYetkileri> TVMUrunYetkileris { get; set; }
        public DbSet<TVMWebServisKullanicilari> TVMWebServisKullanicilaris { get; set; }
        public DbSet<TVMYetkiGruplari> TVMYetkiGruplaris { get; set; }
        public DbSet<TVMYetkiGrupYetkileri> TVMYetkiGrupYetkileris { get; set; }
        public DbSet<Ulke> Ulkes { get; set; }
        public DbSet<UlkeKodlari> UlkeKodlaris { get; set; }
        public DbSet<Urun> Uruns { get; set; }
        public DbSet<UrunParametreleri> UrunParametreleris { get; set; }
        public DbSet<UrunSoru> UrunSorus { get; set; }
        public DbSet<UrunTeminat> UrunTeminats { get; set; }
        public DbSet<UrunVergi> UrunVergis { get; set; }
        public DbSet<Vergi> Vergis { get; set; }
        public DbSet<VKN> VKNs { get; set; }
        public DbSet<WebServisCevap> WebServisCevaps { get; set; }
        public DbSet<WEBServisLog> WEBServisLogs { get; set; }
        public DbSet<X_AracDegerAraTable> X_AracDegerAraTable { get; set; }
        public DbSet<X_AracListesiAraTable> X_AracListesiAraTable { get; set; }
        public DbSet<X_XMLAraTable> X_XMLAraTable { get; set; }
        public DbSet<YKN> YKNs { get; set; }
        public DbSet<z_mapfre_marka> z_mapfre_marka { get; set; }
       
        public DbSet<ZMarka> ZMarkas { get; set; }
        public DbSet<ZMarkaTip> ZMarkaTips { get; set; }
        public DbSet<zTVMUrunYetkileri> ZTVMUrunYetkileris { get; set; }
        public DbSet<TVMYetkiGrupSablonu> TVMYetkiGrupSablonus { get; set; }
        public DbSet<YZ_BuyuksehirNufus> YZ_BuyuksehirNufuss { get; set; }
        public DbSet<ReasurorGenel> ReasurorGenels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AltMenuMap());
            modelBuilder.Configurations.Add(new AltMenuSekmeMap());
            modelBuilder.Configurations.Add(new AnaMenuMap());
            modelBuilder.Configurations.Add(new AracKullanimSekliMap());
            modelBuilder.Configurations.Add(new AracKullanimTarziMap());
            modelBuilder.Configurations.Add(new AracMarkaMap());
            modelBuilder.Configurations.Add(new AracMarkaYedekMap());
            modelBuilder.Configurations.Add(new AracModelMap());
            modelBuilder.Configurations.Add(new AracModelYedekMap());
            modelBuilder.Configurations.Add(new AracTipMap());
            modelBuilder.Configurations.Add(new AracTipYedekMap());
            modelBuilder.Configurations.Add(new AracTrafikTeminatMap());
            modelBuilder.Configurations.Add(new AutoPoliceTransferMap());
            modelBuilder.Configurations.Add(new BankaSubeleriMap());
            modelBuilder.Configurations.Add(new BelediyeMap());
            modelBuilder.Configurations.Add(new BelediyeIlMap());
            modelBuilder.Configurations.Add(new BranMap());
            modelBuilder.Configurations.Add(new BransUrunMap());
            modelBuilder.Configurations.Add(new CR_AracEkSoruMap());
            modelBuilder.Configurations.Add(new CR_AracGrupMap());
            modelBuilder.Configurations.Add(new CR_IlIlceMap());
            modelBuilder.Configurations.Add(new CR_KaskoAMSMap());
            modelBuilder.Configurations.Add(new CR_KaskoDMMap());
            modelBuilder.Configurations.Add(new CR_KaskoFKMap());
            modelBuilder.Configurations.Add(new CR_KaskoIkameTuruMap());
            modelBuilder.Configurations.Add(new CR_KaskoIMMMap());
            modelBuilder.Configurations.Add(new CR_KrediHayatCarpanMap());
            modelBuilder.Configurations.Add(new CR_KullanimTarziMap());
            modelBuilder.Configurations.Add(new CR_MeslekIndirimiKaskoMap());
            modelBuilder.Configurations.Add(new CR_TescilIlIlceMap());
            modelBuilder.Configurations.Add(new CR_TrafikFKMap());
            modelBuilder.Configurations.Add(new CR_TrafikIMMMap());
            modelBuilder.Configurations.Add(new CR_TUMMusteriMap());
            modelBuilder.Configurations.Add(new CR_UlkeMap());
            modelBuilder.Configurations.Add(new DaskBeldeMap());
            modelBuilder.Configurations.Add(new DaskIlMap());
            modelBuilder.Configurations.Add(new DaskIlceMap());
            modelBuilder.Configurations.Add(new DaskKurumlarMap());
            modelBuilder.Configurations.Add(new DaskSubelerMap());
            modelBuilder.Configurations.Add(new DepremMuafiyetMap());
            modelBuilder.Configurations.Add(new DigerUlkeOranlariMap());
            modelBuilder.Configurations.Add(new DilMap());
            modelBuilder.Configurations.Add(new DilAciklamaMap());
            modelBuilder.Configurations.Add(new DuyuruDokumanMap());
            modelBuilder.Configurations.Add(new DuyurularMap());
            modelBuilder.Configurations.Add(new DuyuruTVMMap());
            modelBuilder.Configurations.Add(new El2Garanti_HesapCetveliMap());
            modelBuilder.Configurations.Add(new EPostaFormatlariMap());
            modelBuilder.Configurations.Add(new GenelTanimlarMap());
            modelBuilder.Configurations.Add(new HasarAnlasmaliServislerMap());
            modelBuilder.Configurations.Add(new HasarAsistansFirmalariMap());
            modelBuilder.Configurations.Add(new HasarBankaHesaplariMap());
            modelBuilder.Configurations.Add(new HasarEksperIslemleriMap());
            modelBuilder.Configurations.Add(new HasarEksperListesiMap());
            modelBuilder.Configurations.Add(new HasarGenelBilgilerMap());
            modelBuilder.Configurations.Add(new HasarIletisimYetkilileriMap());
            modelBuilder.Configurations.Add(new HasarNotlariMap());
            modelBuilder.Configurations.Add(new HasarZorunluEvraklariMap());
            modelBuilder.Configurations.Add(new HasarZorunluEvrakListesiMap());
            modelBuilder.Configurations.Add(new IlMap());
            modelBuilder.Configurations.Add(new IlceMap());
            modelBuilder.Configurations.Add(new IsDurumMap());
            modelBuilder.Configurations.Add(new IsDurumDetayMap());
            modelBuilder.Configurations.Add(new IsTakipMap());
            modelBuilder.Configurations.Add(new IsTakipDetayMap());
            modelBuilder.Configurations.Add(new IsTakipDokumanMap());
            modelBuilder.Configurations.Add(new IsTakipIsTipleriMap());
            modelBuilder.Configurations.Add(new IsTakipIsTipleriDetayMap());
            modelBuilder.Configurations.Add(new IsTakipKullaniciGrupKullanicilariMap());
            modelBuilder.Configurations.Add(new IsTakipKullaniciGruplariMap());
            modelBuilder.Configurations.Add(new IsTakipSoruMap());
            modelBuilder.Configurations.Add(new IstigalMap());
            modelBuilder.Configurations.Add(new KaskoDMMap());
            modelBuilder.Configurations.Add(new KaskoFKMap());
            modelBuilder.Configurations.Add(new KaskoIMMMap());
            modelBuilder.Configurations.Add(new KaskoTasinanYukKademeleriMap());
            modelBuilder.Configurations.Add(new KesintilerMap());
            modelBuilder.Configurations.Add(new KesintiTurleriMap());
            modelBuilder.Configurations.Add(new KonfigurasyonMap());
            modelBuilder.Configurations.Add(new MapfreKullaniciMap());
            modelBuilder.Configurations.Add(new MenuIslemMap());
            modelBuilder.Configurations.Add(new MeslekMap());
            modelBuilder.Configurations.Add(new MeslekIndirimiKaskoMap());
            modelBuilder.Configurations.Add(new MusteriAdreMap());
            modelBuilder.Configurations.Add(new MusteriDokumanMap());
            modelBuilder.Configurations.Add(new MusteriGenelBilgilerMap());
            modelBuilder.Configurations.Add(new MusteriNotMap());
            modelBuilder.Configurations.Add(new MusteriTelefonMap());
            modelBuilder.Configurations.Add(new NeoConnectLogMap());
            modelBuilder.Configurations.Add(new NeoConnectPoliceDetayMap());
            modelBuilder.Configurations.Add(new NeoConnectSirketDetayMap());
            modelBuilder.Configurations.Add(new NeoConnectSirketGrupKullaniciDetayMap());
            modelBuilder.Configurations.Add(new NeoConnectTvmSirketYetkileriMap());
            modelBuilder.Configurations.Add(new NeoConnectYasakliUrllerMap());
            modelBuilder.Configurations.Add(new OfflinePolouseMap());
            modelBuilder.Configurations.Add(new OfflinePoliceNumaraMap());
            modelBuilder.Configurations.Add(new OtoLoginSigortaSirketKullanicilarMap());
            modelBuilder.Configurations.Add(new ParaBirimleriMap());
            modelBuilder.Configurations.Add(new PaylasimliPoliceUretimMap());
            modelBuilder.Configurations.Add(new PoliceAracMap());
            modelBuilder.Configurations.Add(new PoliceGenelMap());
            modelBuilder.Configurations.Add(new PoliceOdemePlaniMap());
            modelBuilder.Configurations.Add(new PoliceRizikoAdresiMap());
            modelBuilder.Configurations.Add(new PoliceSigortaEttirenMap());
            modelBuilder.Configurations.Add(new PoliceSigortaliMap());
            modelBuilder.Configurations.Add(new PoliceTahsilatMap());
            modelBuilder.Configurations.Add(new PoliceTaliAcentelerMap());
            modelBuilder.Configurations.Add(new PoliceTaliAcenteRaporMap());
            modelBuilder.Configurations.Add(new PoliceUretimHedefGerceklesenMap());
            modelBuilder.Configurations.Add(new PoliceUretimHedefPlanlananMap());
            modelBuilder.Configurations.Add(new PoliceVergiMap());
            modelBuilder.Configurations.Add(new PotansiyelMusteriAdreMap());
            modelBuilder.Configurations.Add(new PotansiyelMusteriDokumanMap());
            modelBuilder.Configurations.Add(new PotansiyelMusteriGenelBilgilerMap());
            modelBuilder.Configurations.Add(new PotansiyelMusteriNotMap());
            modelBuilder.Configurations.Add(new PotansiyelMusteriTelefonMap());
            modelBuilder.Configurations.Add(new SaglikBolgeleriMap());
            modelBuilder.Configurations.Add(new SaglikTeminatPrimleriMap());
            modelBuilder.Configurations.Add(new SaglikTeminatTipleriMap());
            modelBuilder.Configurations.Add(new SchengenUlkeOranlariMap());
            modelBuilder.Configurations.Add(new SigortaSirketleriMap());
            modelBuilder.Configurations.Add(new SoruMap());
            modelBuilder.Configurations.Add(new TableMap());
            modelBuilder.Configurations.Add(new TaliAcenteKomisyonOraniMap());
            modelBuilder.Configurations.Add(new TCKNMap());
            modelBuilder.Configurations.Add(new TeklifAracMap());
            modelBuilder.Configurations.Add(new TeklifAracEkSoruMap());
            modelBuilder.Configurations.Add(new TeklifDigerSirketlerMap());
            modelBuilder.Configurations.Add(new TeklifEMailLogMap());
            modelBuilder.Configurations.Add(new TeklifGenelMap());
            modelBuilder.Configurations.Add(new TeklifNoSayacMap());
            modelBuilder.Configurations.Add(new TeklifNotMap());
            modelBuilder.Configurations.Add(new TeklifOdemePlaniMap());
            modelBuilder.Configurations.Add(new TeklifProvizyonMap());
            modelBuilder.Configurations.Add(new TeklifRizikoAdresiMap());
            modelBuilder.Configurations.Add(new TeklifSigortaEttirenMap());
            modelBuilder.Configurations.Add(new TeklifSigortaliMap());
            modelBuilder.Configurations.Add(new TeklifSoruMap());
            modelBuilder.Configurations.Add(new TeklifTeminatMap());
            modelBuilder.Configurations.Add(new TeklifVergiMap());
            modelBuilder.Configurations.Add(new TeklifWebServisCevapMap());
            modelBuilder.Configurations.Add(new TeminatMap());
            modelBuilder.Configurations.Add(new TrafikFKMap());
            modelBuilder.Configurations.Add(new TrafikIMMMap());
            modelBuilder.Configurations.Add(new TUMBankaHesaplariMap());
            modelBuilder.Configurations.Add(new TUMDetayMap());
            modelBuilder.Configurations.Add(new TUMDokumanlarMap());
            modelBuilder.Configurations.Add(new TUMDurumTarihcesiMap());
            modelBuilder.Configurations.Add(new TUMIletisimYetkilileriMap());
            modelBuilder.Configurations.Add(new TUMIPBaglantiMap());
            modelBuilder.Configurations.Add(new TUMNotlarMap());
            modelBuilder.Configurations.Add(new TUMUrunleriMap());
            modelBuilder.Configurations.Add(new TVMAcentelikleriMap());
            modelBuilder.Configurations.Add(new TVMBankaHesaplariMap());
            modelBuilder.Configurations.Add(new TVMBolgeleriMap());
            modelBuilder.Configurations.Add(new TVMDepartmanlarMap());
            modelBuilder.Configurations.Add(new TVMDetayMap());
            modelBuilder.Configurations.Add(new TVMDokumanlarMap());
            modelBuilder.Configurations.Add(new TVMDurumTarihcesiMap());
            modelBuilder.Configurations.Add(new TVMIletisimYetkilileriMap());
            modelBuilder.Configurations.Add(new TVMIPBaglantiMap());
            modelBuilder.Configurations.Add(new TVMKullaniciAtamaMap());
            modelBuilder.Configurations.Add(new TVMKullaniciDurumTarihcesiMap());
            modelBuilder.Configurations.Add(new TVMKullanicilarMap());
            modelBuilder.Configurations.Add(new TVMKullaniciNotlarMap());
            modelBuilder.Configurations.Add(new TVMKullaniciSifremiUnuttumMap());
            modelBuilder.Configurations.Add(new TVMKullaniciSifreTarihcesiMap());
            modelBuilder.Configurations.Add(new TVMNotlarMap());
            modelBuilder.Configurations.Add(new TVMSMSKullaniciBilgiMap());
            modelBuilder.Configurations.Add(new TVMUrunYetkileriMap());
            modelBuilder.Configurations.Add(new TVMWebServisKullanicilariMap());
            modelBuilder.Configurations.Add(new TVMYetkiGruplariMap());
            modelBuilder.Configurations.Add(new TVMYetkiGrupYetkileriMap());
            modelBuilder.Configurations.Add(new UlkeMap());
            modelBuilder.Configurations.Add(new UlkeKodlariMap());
            modelBuilder.Configurations.Add(new UrunMap());
            modelBuilder.Configurations.Add(new UrunParametreleriMap());
            modelBuilder.Configurations.Add(new UrunSoruMap());
            modelBuilder.Configurations.Add(new UrunTeminatMap());
            modelBuilder.Configurations.Add(new UrunVergiMap());
            modelBuilder.Configurations.Add(new VergiMap());
            modelBuilder.Configurations.Add(new VKNMap());
            modelBuilder.Configurations.Add(new WebServisCevapMap());
            modelBuilder.Configurations.Add(new WEBServisLogMap());
            modelBuilder.Configurations.Add(new X_AracDegerAraTableMap());
            modelBuilder.Configurations.Add(new X_AracListesiAraTableMap());
            modelBuilder.Configurations.Add(new X_XMLAraTableMap());
            modelBuilder.Configurations.Add(new YKNMap());

           
            modelBuilder.Configurations.Add(new zTVMUrunYetkileriMap());
            modelBuilder.Configurations.Add(new TVMYetkiGrupSablonuMap());
            
            modelBuilder.Configurations.Add(new YZ_BuyuksehirNufusMap());
            modelBuilder.Configurations.Add(new ReasurorGenelMap());

        }
    }
}
