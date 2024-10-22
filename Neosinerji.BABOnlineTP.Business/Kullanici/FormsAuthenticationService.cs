using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Neosinerji.BABOnlineTP.Database;

namespace Neosinerji.BABOnlineTP.Business
{
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        IAktifKullaniciService CurrentUser;

        public FormsAuthenticationService(IAktifKullaniciService currentUser)
        {
            CurrentUser = currentUser;
        }

        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthenticationTicket ticket = new
                        FormsAuthenticationTicket(1, userName,
                                                    TurkeyDateTime.Now, TurkeyDateTime.Now.AddDays(20),
                                                    createPersistentCookie,
                                                    CurrentUser.KullaniciKodu.ToString(),
                                                    FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.Expires = TurkeyDateTime.Now.AddMinutes(20);
            cookie.Secure = true;

            //Set cookie
            HttpContext.Current.Response.Cookies.Add(cookie);


            HttpCookie projeKod = new HttpCookie("proc_kod", CurrentUser.ProjeKodu);
            projeKod.Expires = TurkeyDateTime.Now.AddDays(10);
            HttpContext.Current.Response.Cookies.Add(projeKod);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
            //  HttpContext.Current.Session.Abandon();
        }
    }
}
