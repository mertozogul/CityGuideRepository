using CityGuide.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using System.Web.Services;

namespace CityGuide.Controllers
{
    public class CityGuideController : Controller
    {
        #region Action For CityGuide Main Page
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Action For Auto Complete Textbox
        /*
            Autocomplete Textbox için yazdığım action. Autocomplete Textbox'dan gelen string ile başlayan 
            şehir isimlerini json ile Autocomplete Textbox'a gönderdim. Autocomplete Textbox'dan gelen string 
            terimi boş ise (gelen boş string'in içerisindeki boşluk karakter sayısı önemli değildir) bütün şehir
            isimlerini Json ile Autocomplete Textbox'a gönderdim. Yani Autocomplete Textbox'a girilen bir veya 
            daha fazla boşluk bütün şehirlerin listelenmesini sağlar. Şehir isimlerini linq ile GetCityInfos() 
            methodundan çektim. 
        */
        public ActionResult GetSearchedCityNames(string term)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            string searchTerm = term.ToLower(culture);
            var result = (dynamic)null;
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                result = from r in GetCityInfos()
                         select r.CityName;
            }
            else
            {
                result = from r in GetCityInfos()
                         where r.CityName.ToLower().StartsWith(searchTerm)
                         select r.CityName;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Action For Returning City Details To CityDetails Page
        /*
            Search butonuna tıklandığında bu action çalışır.GetCityInfos() metodunu kullanarak 
            CitiesInfo model classına şehir detay bilgilerini çektim ve model ile view'e gönderdim. 
        */
        public ActionResult GetCityDetails(string CityName)
        {
            /*
                cities.json'da bulunan double enlem ve boylam sayıları virgülle ayrılmış olarak deil de 
                noktayla ayrılmış olarak gelmesi için sistemin sayı ayracını ',' den '.' ya dönüştürdüm.
                Bu işlemi yaparak haritayı gösteren javascript fonksiyonuna enlem ve boylam bilgilerini 
                doğru formatta göndermiş oldum.
            */
            CultureInfo culture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;

            var cities = GetCityInfos();

            if (IsCityExistInDatabase(cities, CityName))
            {

                CitiesInfo model = new CitiesInfo();
                foreach (var city in cities)
                {
                    if (city.CityName.ToLower(culture) == CityName.ToLower(culture))
                        model = city;
                }
                return View("CityDetails", model);
            }
            else
            {
                return View("CityNotFound");
            }
        }
        #endregion

        #region Method For Checking Whether The City Is In DataBase
        /*
            Bu method kendisine parametre olarak gelen string değerin kendisine parametre
            olarak gelen listenin içerisinde olup olmadığını kontrol eder. Eğer string 
            listenin içerisinde mevcut ise true değilse false değerini döner. Bu method 
            GetCityDetails(string CityName) tarafından çağrılır.Ve Autocomplete Textboxdan 
            gelen şehrin database de olup olmadığı test edilir.Yapılan teste göre kullanıcıya 
            bilgi verilir.
        */
        public bool IsCityExistInDatabase(IList<CitiesInfo> cities, string CityName)
        {
            bool isExist = false;
            CultureInfo culture = CultureInfo.CurrentCulture;
            foreach (var city in cities)
            {
                if (city.CityName.ToLower(culture) == CityName.ToLower(culture))
                    isExist = true;
            }
            return isExist;
        }
        #endregion

        #region Method For Getting City Details From Json File
        /*
            Bu method'u cities.json'da (Json dosyası) bulunan şehir detay datasını çekmek için yazdım. Json datasını
            c#'a json framework'ü olan json.net (newtonsoft) kullanarak (deserialize işlemi ile) çektim. Çekilen datayı 
            Ilist'e atayıp return ettim. Böylece json datasını Ilist biçiminde elde etmek isteyen diğer actionlarda veya
            methodlarda kullanılabilecek ortak bir method yazmış oldum. 
        */
        public IList<CitiesInfo> GetCityInfos()
        {
            IList<CitiesInfo> cities = new List<CitiesInfo>();
            string path = ControllerContext.HttpContext.Server.MapPath("~/JsonData/Cities.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                cities = JsonConvert.DeserializeObject<List<CitiesInfo>>(json);
            }
            return cities;
        }
        #endregion
    }
}