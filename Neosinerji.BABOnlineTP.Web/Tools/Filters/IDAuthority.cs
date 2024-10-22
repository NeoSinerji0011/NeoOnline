using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http.Controllers;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;


namespace Neosinerji.BABOnlineTP.Web.Tools
{
    public class IDAuthority : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ILogService _Log = DependencyResolver.Current.GetService<ILogService>();
            try
            {
                this.TeklifId = null;
                this.Id = null;

                this.TeklifId = HttpContext.Current.Request.Params["teklifId"];

                foreach (var item in filterContext.ActionParameters)
                {
                    switch (item.Key)
                    {
                        case "id": this.Id = item.Value != null ? item.Value.ToString() : ""; break;
                        case "Id": this.Id = item.Value != null ? item.Value.ToString() : ""; break;
                        case "teklifId": this.TeklifId = item.Value != null ? item.Value.ToString() : ""; break;
                    }
                }


                string actionName = filterContext.ActionDescriptor.ActionName;


                IAktifKullaniciService _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
                IMusteriService _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();


                switch (this.Type)
                {
                    case "Teklif":
                        if (actionName == "Ekle")
                        {
                            if (!String.IsNullOrEmpty(this.Id))
                            {
                                int musteriKodu = Convert.ToInt32(this.Id);

                                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);

                                if (musteri == null)
                                {
                                    filterContext.Result = new RedirectResult("/Error/Errorpage/403");
                                    return;
                                }

                                if (!String.IsNullOrEmpty(this.TeklifId))
                                    TeklifYetkiKontolu(filterContext, this.TeklifId);
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(this.Id))
                                TeklifYetkiKontolu(filterContext, this.TeklifId);

                            //Teklif detayı sorgulama sayfaları için teklif id kontrol ediliyor.
                            if (!String.IsNullOrEmpty(this.TeklifId))
                                TeklifYetkiKontolu(filterContext, this.TeklifId);
                        }
                        break;
                    case "Musteri":
                        if (!String.IsNullOrEmpty(this.Id))
                        {
                            if (actionName != "Ekle")
                            {
                                int musteriKodu = Convert.ToInt32(this.Id);

                                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);

                                if (musteri == null)
                                {
                                    filterContext.Result = new RedirectResult("/Error/Errorpage/403");
                                    return;
                                }
                            }
                        }
                        break;
                    case "PotansiyelMusteri":
                        if (!String.IsNullOrEmpty(this.Id))
                        {
                            if (actionName != "Ekle")
                            {
                                int musteriKodu = Convert.ToInt32(this.Id);

                                PotansiyelMusteriGenelBilgiler musteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);

                                if (musteri == null)
                                {
                                    filterContext.Result = new RedirectResult("/Error/Errorpage/403");
                                    return;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                filterContext.Result = new RedirectResult("/Error/ErrorPage/500");
            }
        }

        public string Id { get; set; }
        public string TeklifId { get; set; }
        public string Type { get; set; }

        private void TeklifYetkiKontolu(ActionExecutingContext filterContext, string teklifID)
        {
            if (!String.IsNullOrEmpty(teklifID))
            {
                ITeklifService _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
                int id;
                if (int.TryParse(teklifID, out id))
                {
                    TeklifGenel teklif = _TeklifService.GetTeklifGenel(id);
                    if (teklif == null)
                    {
                        filterContext.Result = new RedirectResult("/Error/Errorpage/403");
                        return;
                    }
                }
                else filterContext.Result = new RedirectResult("/Error/Errorpage/404");
            }
        }
    }
}