using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
   public  class Departments :BaseEntity
    {
        [Column(TypeName = "nvarchar (100)")]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar (5)")]
        public string Code { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
