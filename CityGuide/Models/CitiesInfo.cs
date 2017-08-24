using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityGuide.Models
{
    public class CitiesInfo
    {
        public string CityName { get; set; }

        public string CityURL { get; set; }

        public string CityDescription { get; set; }

        public double CityLatitude { get; set; }

        public double CityLongitude { get; set; }
    }
}