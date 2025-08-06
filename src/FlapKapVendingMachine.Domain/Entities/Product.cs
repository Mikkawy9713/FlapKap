using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlapKapVendingMachine.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int AmountAvailable { get; set; }
        public int Cost { get; set; } // in cents
        public int SellerId { get; set; }
    }
}
