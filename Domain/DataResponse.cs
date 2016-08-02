using CoursUpdate.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoursUpdate
{
    [DataContract]
    public class DataResponse
    {

        [DataMember(Name = "posts")]
        public PostCollection posts { get; set; }
        [DataMember(Name = "id")]
        public string id { get; set; }

    }
}
