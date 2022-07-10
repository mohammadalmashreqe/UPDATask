using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class UserDetailsViewModel
    {
        public Guid EmpId { set; get;  }
        public string Name { get; set; }
        public string Roles { get; set; }
        public string positions { get; set; }
        public DateTime DateOfJoin { set; get; }
        public string DepartmentName {set; get; }
        public string DepartmentCode { set; get; }
        public string Address { get; set; }
        public string Imageurl { set; get; }
        public string UserName { set; get; }
        public string Email { set; get; }
        public IFormFile Image { get; set; }







   

    }
}
