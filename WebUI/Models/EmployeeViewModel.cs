using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class EmployeeViewModel
    {
       public Guid EmpId { set; get;  }
        public string Name { get; set; }
        public string DepartmentName { set; get; }
        public string JoiningDate { set; get; }
        public string DateofBirth { set; get; }

        public string Email { get; set; }

        public string Address { get; set; }
       
        //public IEnumerable <string> Roles { set; get;  }
        public string ImageUrl { get; set; }
        //public Guid UserId { get; set; }
    }
}
