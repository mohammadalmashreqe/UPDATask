using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
  public  class EmployeePosition 
    {

        public Guid EmployeeID { get; set; }
        public Guid PositionID { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Position Position { get; set; } 
    }
}
