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
            string file = "hello.dem";

            foreach (string player in parseDemo(file))
            {
                parseForName(nameRequest(player));
                parseForBan(banRequest(player));
            }
        }

        static List<String> parseDemo(string fileName)
        {
            List<String> playerIDs = new List<String>();
            using (var fileStream = File.OpenRead(fileName))
            {
                using (var parser = new DemoParser(fileStream))
                {
                    //hasMatchStarted framkallar viðvörun um að mögulegt sé
                    //að það verði aldrei notað, með þessu slökkvum við á
                    //þessari viðvörun.
                    #pragma warning disable 0219 
                    bool hasMatchStarted = false;
                    #pragma warning restore 0219

                    parser.ParseHeader();

                    parser.MatchStarted += (sender, e) =>
                    {
                        hasMatchStarted = true;

                        foreach (var player in parser.PlayingParticipants)
                        {
                            string test = player.SteamID.ToString();
                            playerIDs.Add(test);
                        }
                    };
                    //Þetta fyrir neðan er algjörlega ógeðslegt, ég veit. 
                    //En er samt nauðsynlegt svo að playerIDs verði fyllt. 
                    for (int i = 0; i < 3000; i++)
                    {
                        parser.ParseNextTick();
                    }
                }
            }
            return playerIDs;
        }

        //Tekur við XML strengnum frá sendRequest og les úr honum gildin sem við viljum fá
        static void parseForBan(string xmlInput)
        {
            StringBuilder output = new StringBuilder();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlInput), settings))
            {
                    reader.ReadToFollowing("SteamId");
                    output.AppendLine("SteamId: " + reader.ReadElementContentAsString());

                    reader.ReadToFollowing("VACBanned");
                    output.AppendLine("VAC Banned:  " + reader.ReadElementContentAsString());
            }

            Console.WriteLine(output);
        }

        static void parseForName(string xmlInput)
        {
            StringBuilder output = new StringBuilder();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlInput), settings))
            {
                    reader.ReadToFollowing("personaname");
                    output.Append("Player name: " + reader.ReadElementContentAsString());
            }

            Console.WriteLine(output);
        }


        //Sendir HTTP beiðni á Steam Community og sækir bans á einhverjum ákveðnum leikmanni, skilar svo sem XML streng
        static string banRequest(string steamId)
        {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=" + getAPIKey() + "&steamids=" + steamId + "&format=xml");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader stream = new StreamReader(response.GetResponseStream());

                string xmlResult = stream.ReadToEnd();

                return xmlResult;
        } 
        
        static string nameRequest(string steamId)
        {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + getAPIKey() + "&steamids=" + steamId + "&format=xml");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader stream = new StreamReader(response.GetResponseStream());

                string xmlResult = stream.ReadToEnd();

                return xmlResult;
        }

    }
}
