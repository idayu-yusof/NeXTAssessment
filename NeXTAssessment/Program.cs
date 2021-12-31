﻿using Newtonsoft.Json;
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
            DateTime inputDate = DateTime.Parse(date);

            inputDate = inputDate.AddMinutes(-60);
            //Console.WriteLine("AFTER PARSING : " + parsedDate);

            long[] unixTimestamps = new long[20];
            for (int loop = 0; loop < 13; loop++)
            {                
                long unixTime = ((DateTimeOffset)inputDate).ToUnixTimeSeconds();
                unixTimestamps[loop] = unixTime;
                Console.WriteLine(loop+1 + " ---- " + inputDate + " ---- " +unixTime);
                Console.WriteLine();

                inputDate = inputDate.AddMinutes(+10);

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

                        List<Attributes> attributes = JsonConvert.DeserializeObject<List<Attributes>>(messageTask.Result);
                        
                        foreach ( var item in attributes)
                        {
                            Console.WriteLine("Latitude: " + item.latitude);
                        }
                        

                        //dynamic Result = JsonConvert.DeserializeObject<RootObject>(messageTask.Result);

                        //var value = Result.Attributes.MajorIssue.attributeValue;

                        //var jo = JObject.Parse(messageTask.Result);
                        //var lat = jo["latitude"].ToString();
                        //Console.WriteLine("latitude: " +lat);
                        //Console.Read();

                        Console.WriteLine("Result : " + messageTask.Result);
                        Console.WriteLine();
                    }
                }
                #endregion

            }

            Console.WriteLine("CURRENT TIME " + inputDate);

            //Program program = new Program();
            //await program.GetISSLocation(parsedDate);

        }

        class Attributes
        {
            public float latitude { get; set; }
        }
        //private async Task GetISSLocation(DateTime date)
        //{
        //    var result = date.AddMinutes(-10);

        //    long unixTime = ((DateTimeOffset)date).ToUnixTimeSeconds();
        //    Console.WriteLine("UNIX TIMPSTAMP : " + unixTime);

        //    Console.WriteLine("DEDUCTED 10 MINS : " + result);
        //}
    }
}