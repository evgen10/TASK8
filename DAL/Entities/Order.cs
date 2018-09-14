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
        protected DateTime? shippedDate;
        protected DateTime? orderDate;

        public Order()
        {
            this.OrderStatus = SetStatus();
        }

        [NotColumn]
        public OrderStatuses OrderStatus { get; private set; }

        [Key]
        public int OrderID { get; set; }

        public string CustomerID { get; set; }

        public int? EmployeeID { get; set; }

        [Unchangeable]
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

        [Unchangeable]
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

        /// <summary>
        /// Устанавливает статус заказа
        /// </summary>
        /// <returns></returns>
        protected OrderStatuses SetStatus()
        {
            OrderStatuses status = OrderStatuses.New;

            if (this.OrderDate == null && this.ShippedDate == null)
            {
                status = OrderStatuses.New;
            }

            if (this.OrderDate != null && this.ShippedDate == null)
            {
                status = OrderStatuses.Underway;
            }

            if (this.ShippedDate != null)
            {
                status = OrderStatuses.Completed;
            }


            return status;
        }

    }
}
