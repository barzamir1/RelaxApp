using System;
using System.Collections.Generic;
using System.Text;

namespace App1.DataObjects
{
    public class Measurements
    {
        public string Id { get; set; }
        public string UserID { get; set; }
        public DateTime Date { set; get; }
        public int TRI {get; set; }
        public double PNN50 { get; set; }
        public double SDNN { get; set; }
        public double SDSD { get; set; }
        public string ActivityID { get; set; }
        public int StressIndex { get; set; }
        public int IsStressed { get; set; }
        public double GPSLat { get; set; }
        public double GPSLng { get; set; }

        //not a part of Measurements table:
        [Newtonsoft.Json.JsonIgnore]
        public string ActivityName { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string LabelColor { get; set; }
    }
}
