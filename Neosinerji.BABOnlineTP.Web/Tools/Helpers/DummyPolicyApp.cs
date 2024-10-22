using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public class DummyPolicyApp
    {
       
        public static List<DumyPolicy> Test()
        {
            string Baseurl = "http://dummy-getpolicy.azurewebsites.net/";
            List<DumyPolicy> EmpInfo = new List<DumyPolicy>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                var Res = client.GetAsync("api/dumypolicy");
                 EmpInfo = JsonConvert.DeserializeObject<List<DumyPolicy>>(Res.Result.Content.ReadAsStringAsync().Result);
                //Checking the response is successful or not which is sent using HttpClient  

                //Storing the response details recieved from web api   
                // var EmpResponse = Res.Result.;
                //Deserializing the response recieved from web api and storing into the Employee list  
                // EmpInfo = JsonConvert.DeserializeObject<List<DumyPolicy>>(EmpResponse);


                //returning the employee list to view  
                return EmpInfo;
            }
        }
        public class DumyPolicy
        {
            public DumyPolicy() { }
            public DumyPolicy(int policyCode, string policyName)
            {
                this.policyCode = policyCode;
                this.policyName = policyName;
            }
            public int policyCode { get; set; }
            public string policyName { get; set; }
            public int probability { get; set; }

        }
    }
}