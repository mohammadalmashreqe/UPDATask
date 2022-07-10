using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class EmployeeManagementViewModel
    {

        public IEnumerable<EmployeeViewModel> Employees { set; get;  }
    }
}
