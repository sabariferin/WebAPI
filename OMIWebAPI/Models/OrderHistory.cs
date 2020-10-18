using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMIWebAPI.Models
{
    public class OrderHistory
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
    }
}
