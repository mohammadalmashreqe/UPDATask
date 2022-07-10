using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class UpdateEmployeeInfoViewModel
    {
        public Guid empid { set; get; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        [Required]

       

        public string Name { get; set; }

        [DisplayName("Department")]
        [Required]

        public Guid DepartmentId { set; get; }
   
        [Required]

        [DataType(DataType.Date)]
        [DisplayName("BirthDate")]

        public DateTime DateofBirth { set; get; }
        [Required]

        public string Address { get; set; }
        [Required]

        public string ImageUrl { get; set; }
       

        public IFormFile Image { get; set; }
        //public Guid UserId { get; set; }
    }
}
