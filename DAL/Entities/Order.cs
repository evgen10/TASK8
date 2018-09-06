using DAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Entities
{
    public class Order : BaseEntity
    {
    
        [NotColumn]
        public OrderStatuses OrderStatus { get;set; }

        public Order()
        {
            this.OrderStatus = SetStatus();
        }

        [Key]
        public int OrderID { get; set; }

        public string CustomerID { get; set; }
        public int? EmployeeID { get; set; }


        private DateTime? orderDate;
        public DateTime? OrderDate
        {
            get { return orderDate; }
            set
            {
                orderDate = value;
                OrderStatus = SetStatus();
            }
        }


        public DateTime? RequiredDate { get; set; }

        private DateTime? shippedDate;
        public DateTime? ShippedDate
        {
            get { return shippedDate; }
            set
            {
                shippedDate = value;
                OrderStatus = SetStatus();
            }
        }




        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }



        private OrderStatuses SetStatus()
        {
            if (this.OrderDate == null)
            {
                return OrderStatuses.New;
            }

            if (this.ShippedDate == null)
            {
                return OrderStatuses.Underway;
            }
            else
            {
                return OrderStatuses.Completed;
            }
        }

    }
}
