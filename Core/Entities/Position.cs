using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Position :BaseEntity
    {
        [Column(TypeName = "nvarchar (150)")]
        public string Name { get; set; }
        public virtual ICollection<EmployeePosition> UserPositions { get; set; }

    }
}
