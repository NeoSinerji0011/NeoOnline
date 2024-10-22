using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IFormsAuthenticationService
    {
        void SignIn(string tckn, bool createPersistentCookie);
        void SignOut();
    }
}
