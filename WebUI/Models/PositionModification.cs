using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Models
{
    public class PositionModification
    {
        [Required]
        public string PositionName { get; set; }

        public Guid PositionId { get; set; }

        public Guid[] AddIds { get; set; }

        public Guid[] DeleteIds { get; set; }
    }
}
