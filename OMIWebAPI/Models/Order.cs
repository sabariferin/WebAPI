using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMIWebAPI.Models
{
    public class Order
    {
        public int? ID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public DateTime PurchasedDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
