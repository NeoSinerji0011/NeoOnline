using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IMembershipService
    {
        bool ValidateUser(string tckn, string sifre);
        bool ValidateUserAEGON(string personelKodu, string sifre);
        bool ValidateUserAEGON_SSO(string USER_NAME, string SESSION_TOKEN, string CHECKSUM);
        bool ValidateUserMAPFRE(string personelKodu, string sifre);
        void LockUser(int kullaniciKodu);
        void UnlockUser(int kullaniciKodu);
    }
}
