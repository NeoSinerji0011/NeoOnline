using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.Paratika3DCode
{
    public class ParatikaService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ParatikaService));

        public String getConnection(String url, String reqMsg,out bool hata)
        {
            hata = false;
            String outputData = System.String.Empty;
            try
            {
                url = "https://vpos.paratika.com.tr/paratika/api/v2";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://vpos.paratika.com.tr/paratika/api/v2");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] data = Encoding.UTF8.GetBytes(reqMsg);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                request.KeepAlive = false;
                HttpWebResponse response =
                (HttpWebResponse)request.GetResponse();
                //Console.Read();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                String read = streamRead.ReadToEnd();
                outputData = read;
                streamResponse.Close();
                streamRead.Close();
                response.Close();
                //hata = true;
            }
            catch (WebException e)
            {
                log.Info(" ParatikaService --> getConnection(url,reqMsg) --> WebException " + e.ToString());
                outputData = e.ToString();
                hata = true;
            }
            catch (Exception e)
            {
                log.Info(" ParatikaService --> getConnection(url,reqMsg) --> Exception " + e.ToString());
                outputData = e.ToString();
                hata = true;
            }

         
            return outputData;
        }

        public String getConnectionGuid(String url, String reqMsg)
        {
            String outputData = System.String.Empty;
            try
            {
                url = "https://vpos.paratika.com.tr/paratika/api/v2";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] data = Encoding.UTF8.GetBytes(reqMsg);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                request.KeepAlive = false;
                HttpWebResponse response =
                (HttpWebResponse)request.GetResponse();
                //Console.Read();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                String read = streamRead.ReadToEnd();
                outputData = read;
                streamResponse.Close();
                streamRead.Close();
                response.Close();

            }
            catch (WebException e)
            {
                log.Info(" ParatikaService --> getConnection(url,reqMsg) --> WebException " + e.ToString());
            }
            catch (Exception e)
            {
                log.Info(" ParatikaService --> getConnection(url,reqMsg) --> Exception " + e.ToString());
            }

            return outputData;
        }

    }
}
