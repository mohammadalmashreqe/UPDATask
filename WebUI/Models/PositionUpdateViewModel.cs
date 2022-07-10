using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class PositionUpdateViewModel
    {
        public PositionViewModel Position { get; set; }
        public IEnumerable<EmployeeViewModel> Members { get; set; }
        public IEnumerable<EmployeeViewModel> NonMembers { get; set; }
    }
}
