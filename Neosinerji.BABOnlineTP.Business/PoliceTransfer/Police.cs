using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class Police : IPolice
    {
        public Police()
        {
        }

        private PoliceGenel _GenelBilgiler;
        public PoliceGenel GenelBilgiler
        {
            get
            {
                if (_GenelBilgiler == null)
                {
                    _GenelBilgiler = new PoliceGenel();
                    _GenelBilgiler.PoliceSigortaEttiren = new PoliceSigortaEttiren();
                    _GenelBilgiler.PoliceSigortali = new PoliceSigortali();
                    _GenelBilgiler.PoliceRizikoAdresi = new PoliceRizikoAdresi();
                    _GenelBilgiler.PoliceArac = new PoliceArac();
                }


                return _GenelBilgiler;
            }
            set
            {
                _GenelBilgiler = value;
            }
        }
        private List<MusteriGenelBilgiler> _MusteriBilgiler;
        public List<MusteriGenelBilgiler> MusteriBilgiler
        {
            get
            {
                if(_MusteriBilgiler == null)
                {
                    _MusteriBilgiler = new List<MusteriGenelBilgiler>();
                }
                return _MusteriBilgiler;
            }
            set
            {
                _MusteriBilgiler = value;
            }
        }
        private List<PoliceOdemePlani> _policeOdemePlani;
        public List<PoliceOdemePlani> policeOdemePlani
        {
            get
            {
                if (_policeOdemePlani == null)
                {
                    _policeOdemePlani = new List<PoliceOdemePlani>();
                }
                return _policeOdemePlani;
            }
            set
            {
                _policeOdemePlani = value;
            }
        }

        private List<PoliceTahsilat> _policeTahsilat;
        public List<PoliceTahsilat> policeTahsilat
        {
            get
            {
                if (_policeTahsilat == null)
                {
                    _policeTahsilat = new List<PoliceTahsilat>();
                }
                return _policeTahsilat;
            }
            set
            {
                _policeTahsilat = value;
            }
        }

        private PoliceSigortaEttiren _policeSigortaEttiren;
        public PoliceSigortaEttiren policeSigortaEttiren
        {
            get
            {
                if (_policeSigortaEttiren == null)
                {
                    _policeSigortaEttiren = new PoliceSigortaEttiren();
                }
                return _policeSigortaEttiren;
            }
            set
            {
                _policeSigortaEttiren = value;
            }
        }

        private PoliceSigortali _policeSigortali;
        public PoliceSigortali policeSigortali
        {
            get
            {
                if (_policeSigortali == null)
                {
                    _policeSigortali = new PoliceSigortali();
                }
                return _policeSigortali;
            }
            set
            {
                _policeSigortali = value;
            }
        }

        public IEnumerable<AcenteBankaHesaplariModel> BankaHesaplari { get; set; }
        //AcenteBankaHesaplariModel asdaff = new AcenteBankaHesaplariModel();
    }
    public class KimlikGuncelleModel
    {
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int policeId { get; set; }

        public string kimlikNo { get; set; }
    }
    public class OdemeTipiGuncelleModel
    {
        public int policeId { get; set; }
        public string OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
        private PoliceGenel _GenelBilgiler;

        public PoliceGenel GenelBilgiler
        {
            get
            {
                if (_GenelBilgiler == null)
                {
                    _GenelBilgiler = new PoliceGenel();
                    _GenelBilgiler.PoliceSigortaEttiren = new PoliceSigortaEttiren();
                    _GenelBilgiler.PoliceSigortali = new PoliceSigortali();
                    _GenelBilgiler.PoliceRizikoAdresi = new PoliceRizikoAdresi();
                    _GenelBilgiler.PoliceArac = new PoliceArac();

                }


                return _GenelBilgiler;
            }
            set
            {
                _GenelBilgiler = value;
            }
        }
    }
    public class AcenteBankaHesaplariModel
    {
        public List<SelectListItem> AcenteBankaHesapList { get; set; }
        public string AcenteBankaHesapKodu { get; set; }
    }
}
