using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace StressCalculator
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // parse query parameter
            dynamic body = await req.Content.ReadAsStringAsync();
            var dict = req.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            if (!(dict.ContainsKey("UserID") || dict.ContainsKey("intervalsArr") || dict.ContainsKey("ActivityName")
                || dict.ContainsKey("dateTime") || dict.ContainsKey("GPSLat") || dict.ContainsKey("GPSLng")))
                return req.CreateResponse(HttpStatusCode.BadRequest, "missing arguments\n");

            //start processing intervals:
            double[] arr = JsonConvert.DeserializeObject<double[]>(dict["intervalsArr"]);
            Measurement m = new Measurement(new List<double>(arr));
            
            m.UserID = dict["UserID"];
            String ActivityName = dict["ActivityName"];
            m.Date = DateTime.Parse(dict["dateTime"]);
            m.GPSLat = double.Parse(dict["GPSLat"]);
            m.GPSLng = double.Parse(dict["GPSLng"]);
            
            //insert measurement to DB
            await DBSender.SendMeasurementToDBAsync(m, ActivityName);
            return req.CreateResponse(HttpStatusCode.OK, "Successfully added measurement!\n");
        }
    }//TODO: check encryption, add claclStressLevel function and return the stress level.
}

//var arrJson = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "arrJson", true) == 0).Value;
//double[] arr = JsonConvert.DeserializeObject<double[]>(arrJson);
// var e = JsonConvert.DeserializeObject<EventData>(body as string);
//var name = req.GetQueryNameValuePairs()
//    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
//    .Value;

//if (name == null)
//{
//    // Get request body
//    dynamic data = await req.Content.ReadAsAsync<object>();
//    name = data?.name;
//}

//int s = await DBSender.GetActivityID("undefine5");
//await DBSender.SendTestToDBAsync();
//await DBSender.SendMeasureMentToDBAsync(m);