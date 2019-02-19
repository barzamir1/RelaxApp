using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App1.DataObjects
{
    public class Users
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        public String id { set; get; }
        public String FirstName { set; get; }
        public String LastName { set; get; }
        public DateTime DateOfBirth { set; get; }
        public String Gender { set; get; }
        public String Occupation { set; get; }
        public String EmergencyContactName { set; get; }
        public String EmergencyContactPhone { set; get; }
        public String EmergencyContactEmail { set; get; }
        public bool isTherapist { set; get; }
        public String shortID { set; get; } //unique ID
        
        //not a part of Users table:
        [Newtonsoft.Json.JsonIgnore]
        public String WatchingUserID{set;get;}
    }
}
