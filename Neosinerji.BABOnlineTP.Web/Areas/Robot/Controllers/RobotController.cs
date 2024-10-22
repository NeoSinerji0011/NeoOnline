using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Areas.Robot.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Robot.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.RobotTeklif, AltMenuKodu = AltMenuler.RobotTeklifAl)]
    public class RobotController : Controller
    {
        IAktifKullaniciService _AktifKullanici;
        public RobotController(IAktifKullaniciService aktifkullanici)
        {
            _AktifKullanici = aktifkullanici;
        }
        public ActionResult Index()
        {
            RobotModel robotModel = new RobotModel();
            var tvmkodu = _AktifKullanici.TVMKodu.ToString();
            tvmkodu = tvmkodu.Length > 3 ? tvmkodu.Substring(0, 3) : tvmkodu;
            robotModel.Token = new Utils.Util().createToken(tvmkodu);
            robotModel.TvmKodu = int.Parse(tvmkodu);
            robotModel.TvmKK = _AktifKullanici.KullaniciKodu;
            robotModel.Port = "1" + tvmkodu;
            string url = "", port = "";
            var pathfile = AppDomain.CurrentDomain.BaseDirectory + @"\Files\robot_config.json";
            var data = System.IO.File.ReadAllText(pathfile);
            dynamic dyn_data = JsonConvert.DeserializeObject(data);

            foreach (var item in dyn_data)
            {
                url = item.Url.Value;
                if (!string.IsNullOrEmpty(item.Port.Value))
                {
                    url += ":" + item.Port.Value;
                } 
            }
            return Redirect(url + "/?token=" + robotModel.Token + "&tvm=" + robotModel.TvmKodu + "&tvmkk=" + robotModel.TvmKK);

        }
    }
}