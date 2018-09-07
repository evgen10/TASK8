using DAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class EmployeeTerritories: BaseEntity
    {
        [Key]
        public int EmployeeID { get; set; }
        [Key]
        public string TerritoryID { get; set; }

    }
}
