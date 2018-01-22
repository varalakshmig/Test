using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleBotTemplate.Models
{
    [Serializable]
    public class Request
    {
        public string botName { get; set; }
        public string userName { get; set; }
    }
}