using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace NeXTAssessment
{
    class Program
    {        
        static void Main(string[] args)
        {
            Console.WriteLine("ENTER THE DATE AND TIME (eg: 23/07/2021 6:51pm) : ");
            string date = Console.ReadLine();
            Console.WriteLine();

            DateTime inputDate = DateTime.Parse(date);

            DateTime univDate = inputDate.ToUniversalTime();
            univDate = univDate.AddMinutes(-60);
            //Console.WriteLine("AFTER PARSING : " + parsedDate);

            long[] unixTimestamps = new long[20];
            for (int loop = 0; loop < 13; loop++)
            {                
                long unixTime = ((DateTimeOffset)univDate).ToUnixTimeSeconds();
                unixTimestamps[loop] = unixTime;
                Console.WriteLine(loop+1 + ". " + univDate + " ---- " +unixTime);
                Console.WriteLine();

                univDate = univDate.AddMinutes(+10);

                #region - calling API
                HttpClient client = new HttpClient();
                var responseTask = client.GetAsync("https://api.wheretheiss.at/v1/satellites/25544/positions?timestamps=" + unixTimestamps[loop] + "&units=miles");

                responseTask.Wait();
                if (responseTask.IsCompleted)
                {
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var messageTask = result.Content.ReadAsStringAsync();
                        messageTask.Wait();

                        List<Location> location = JsonConvert.DeserializeObject<List<Location>>(messageTask.Result);
                        
                        foreach ( var item in location)
                        {                           
                            var response = client.GetStringAsync("https://api.wheretheiss.at/v1/coordinates/" +item.latitude +"," +item.longitude);

                            response.Wait();

                            Value value = JsonConvert.DeserializeObject<Value>(response.Result);

                                Console.WriteLine("Latitude: " + value.latitude);
                                Console.WriteLine("Longitude: " + value.longitude);
                                Console.WriteLine("Time Zome: " + value.timezone_id);
                                Console.WriteLine("Country Code: " + value.country_code);
                                Console.WriteLine("Map URL: " + value.map_url);
                            
                        }
                        
                        Console.WriteLine();
                    }
                }
                #endregion

            }

            Console.WriteLine("INPUT DATE " + inputDate);

        }

        class Location
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
        }

        class Value
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
            public string timezone_id { get; set; }
            public string country_code { get; set; }
            public string  map_url { get; set; }
        }
    }
}
