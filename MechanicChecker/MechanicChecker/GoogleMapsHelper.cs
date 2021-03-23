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
        /// <summary>
        /// returns distance from user entered location to the product address location
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns>
        /// sample return data from google maps api
        ///     <?xml version="1.0" encoding="UTF-8"?>
        ///     <DistanceMatrixResponse>
        ///     <status>OK</status>
        ///     <origin_address>Scarborough, ON M1B 1L6, Canada</origin_address>
        ///     <destination_address>2500 Lawrence Ave E #10, Scarborough, ON M1P 4R7, Canada</destination_address>
        ///      <row>
        ///      <element>
        ///       <status>OK</status>
        ///       <duration>
        ///        <value>813</value>
        ///        <text>14 mins</text>
        ///       </duration>
        ///       <distance>
        ///        <value>9190</value>
        ///        <text>9.2 km</text>
        ///       </distance>
        ///      </element>
        ///     </row>
        ///     </DistanceMatrixResponse>
        /// </returns>
        public static string CalculateDistance(string source, string destination)
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
