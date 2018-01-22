using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace SampleBotTemplate
{
    public class ParserForFacebook
    {
        public static string FirstName = string.Empty;
        public static string LastName = string.Empty;
        public static string Gender = string.Empty;

        public static CustomerResponse getvalues(string Json, CustomerResponse custResInfo)
        {

            string parserstatus = string.Empty;

            var obj = JsonConvert.DeserializeObject<Root>(Json);
            custResInfo.firstName = obj.first_name;
            custResInfo.lastName = obj.last_name;
            Gender = obj.gender;
            return custResInfo;
        }
    }
    public class Root
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
    }
}
