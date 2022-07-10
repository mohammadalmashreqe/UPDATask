using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class CreateNewEmployeeViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public  string Email { get; set; }
        [Required]

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]

        public string Name { get; set; }

        [DisplayName("Department")]
        [Required]
        public Guid DepartmentId { set; get; }
        [DisplayName("Employee Role")]
        [Required]
        public Guid RoleId { set; get; }
        [Required]

        [DataType(DataType.Date)]
        [DisplayName("BirthDate")]

        public DateTime DateofBirth { set; get; }
        [Required]

        public string Address { get; set; }
        [Required]

        public string ImageUrl { get; set; }
        [Required]

        public IFormFile Image { get; set; }
        //public Guid UserId { get; set; }
    }
}
