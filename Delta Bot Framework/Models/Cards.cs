using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleBotTemplate.Models
{
    public class Cards
    {
        public int buttonCount { get; set; }
        public string[] buttonTitle { get; set; } = new string[10];
        public string[] buttonType { get; set; } = new string[10];
        public string[] buttonValue { get; set; } = new string[10];
        public string HeroImg { get; set; }
        public string HeroTitle { get; set; }
        public string HeroSubtitle { get; set; }
        public string HeroText { get; set; }
        public List<String> buttons { get; set; } = new List<String>();
        public List<ParserForResponseJson.lcards> CardList { get; set; } = new List<ParserForResponseJson.lcards>();

    }
}