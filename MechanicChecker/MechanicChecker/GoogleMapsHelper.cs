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

namespace MechanicChecker
{
    public class GoogleMapsHelper
    {        
        public static async Task<string> CalculateDistance(string source, string destination)
        {
            string distance = "";
            string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + source + "&destinations=" + destination + "&key=AIzaSyAWd6Xlw3397XCmkMm3IjrfLBD2eXaXhCE";
            WebRequest request = WebRequest.Create(url);
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
