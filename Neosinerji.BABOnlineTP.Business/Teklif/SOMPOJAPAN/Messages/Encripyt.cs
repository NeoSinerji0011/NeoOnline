using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.SOMPOJAPAN.Messages
{
    public class Crypto
    {
        public string Encript(string metin)
        {
            Encoding iso = Encoding.GetEncoding("ISO-8859-9");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(metin);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
           // string msg = iso.GetString(isoBytes);
            
            var publicKey ="<RSAKeyValue><Modulus>0MGEnUxDpWP8PXNP42HkPJ1nnaSUZaf4gQGo0rwvoqJfpybtkeTsdsFi2OFu3AELiJMuzf0a7jxB8/u149AFgh0zHoMg9AMaRoCVlZGhkjvDjYJOonbwtStFswoOj4HOgh7K8ul3nJWonSuqmzCvNjMG7JMEY7/8bHaVPJ20ZpauSWeo"+
            "FJav5RtR/AihxUWg0nlSbAZV6bnCQpsxOl2zjiMHW2EEvgA0K87sXmrtq34mzeNVixCBRQh/NxBpxU8NMcCcDV73K54jpkZd7FMlKRxBgG6qwNHYm2iQVM/+RtSGZBtExuOoHlj8jVltqQmN1e3N9F4XNhBIl8WU3BalcOYJMTgLktjbWlBPAuAiY4Qt4x/onCEH/oADg+xTyXoqCCdTeUs"+
            "YeMVGMqfcJRzQ/w/iZ+Er5yCIs2xPfqtX7rcKrW3Dn0SzDimeBxB0hagspMn8qnvuI+BbIkHMLYrHLLXf9v448zGPh0dSNcq4snQa8K2Ds/sXaTUx/gi/hudELNztd80zFp8InGYlfW1yptUpDLIis4oLpvb82zUn5g3tSqFZhFkb254aFY6IvsfDP8De7mp8G0LbGL8zJemW96l7atNEHMNV"+
            "oU6/kv+iUuJl7QXrGYnk6z48aecssRXNtcBAxjs9bo7r2DKis9vN8voJlaMvzO3G9b1Omn3XQCk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";


            string base64Encrypted;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // client encrypting data with public key issued by server          
                    rsa.FromXmlString(publicKey);
                    var encryptedData = rsa.Encrypt(isoBytes, true);
                    base64Encrypted = Convert.ToBase64String(encryptedData);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return base64Encrypted;
        }
    }
}
