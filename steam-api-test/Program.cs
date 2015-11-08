using System;
using System.Reflection;
using System.IO;
using DemoInfo;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Text;
using System.Xml;


namespace steam_api_test
{
    class Program
    {
        //Gets the Steam Web API key from file
        static String getAPIKey()
        {
            StreamReader parse = File.OpenText("apikey");
            return parse.ReadLine();
        }

        static void Main(string[] args)
        {
            string playerID = "76561197963115149";

            GetVacBan(sendRequest(playerID));
            Console.ReadKey();
        }

        //Tekur við XML strengnum frá sendRequest og les úr honum gildin sem við viljum fá
        static void GetVacBan(string xmlInput)
        {
            StringBuilder output = new StringBuilder();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlInput), settings))
            {
                reader.ReadToFollowing("SteamId");
                output.AppendLine("SteamId: " + reader.ReadElementContentAsString());

                reader.ReadToFollowing("VACBanned");
                output.AppendLine("VAC Banned?:  " + reader.ReadElementContentAsString());
            }

            Console.WriteLine(output);

        }

        //Sendir HTTP beiðni á Steam Community og sækir bans á einhverjum ákveðnum leikmanni, skilar svo sem XML streng
        static string sendRequest(string steamId)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=" + getAPIKey() + "&steamids=" + steamId + "&format=xml");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader stream = new StreamReader(response.GetResponseStream());

            string xmlResult = stream.ReadToEnd();

            return xmlResult;
        }
    }
}
