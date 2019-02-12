using System;
using System.Collections.Generic;
using System.Text;

namespace App1.DataObjects
{
    class UserAuthorizations
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        public String id { set; get; } //key
        public String TherapistID { set; get; }
        public String AuthorizedToViewUserID { set; get; }
    }
}
