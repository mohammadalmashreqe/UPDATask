using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Employee : BaseEntity
    {
        [Column(TypeName = "nvarchar (150)")]
        public string Name { get; set; }
        public Guid DepartmentId { set; get; }
        public DateTime JoiningDate{set; get;}
        public DateTime DateofBirth { set; get; }
        
        [Column(TypeName = "Ntext")]

        public string Address { get; set; }
        [Column(TypeName = "Nvarchar(100)")]

        public string ImageUrl { get; set; }
        public Guid UserId { get; set; }

        public virtual Departments Department{ get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<EmployeePosition> UserPositions { get; set; }










    }
}
