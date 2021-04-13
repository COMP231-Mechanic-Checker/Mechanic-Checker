using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MechanicChecker.Models;

namespace MechanicChecker
{
    public class GoogleMapsHelper
    {
        public static string CalculateDistance(ExternalAPIsContext contextAPIs, string source, string destination)
        {
            string apiKeyOwner = Startup.Configuration.GetSection("APIKeyOwners")["GoogleMaps"];
            ExternalAPIs googleMapsAPI = contextAPIs.GetApiByService("DeveloperAPI GoogleMaps", apiKeyOwner);

            // we assume that the key is active and quota is sufficient since we have unlimited quota for this for the next 2 months
            string googleMapsAPIKey = googleMapsAPI.APIKey;

            string distance = "";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + source + "&destinations=" + destination + "&key=" + googleMapsAPIKey;
            WebRequest request = WebRequest.Create(url);
            contextAPIs.activateAPI("DeveloperAPI GoogleMaps", apiKeyOwner); // reduce quota by 1

            using (WebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var result = reader.ReadToEnd();
                    //xml parsing and reading
                    XDocument doc = XDocument.Parse(result);
                    XElement row = doc.Root.Element("row");
                    XElement el = row.Element("element");
                    XElement dist = el.Element("distance");

                    distance = dist.Element("text").Value;
                }
            }

            return distance;
        }
    }
}
