using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Robot.Utils
{
    public class Util
    {
        public string createToken(string tvmKodu)
        {
            var data = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Areas\\Robot\\Utils\\appsettings.json");
            dynamic json = JsonConvert.DeserializeObject(data);

            string secret = Convert.ToString(json.AppSettings.Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.SerialNumber, tvmKodu)

                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}