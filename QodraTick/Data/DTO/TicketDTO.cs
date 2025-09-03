using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO;

public class TicketDTO
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Subject { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Priority { get; set; }

    [Required]
    public string Category { get; set; }

    public int? AssignedToUserId { get; set; }
    public int? ClosedByUserId { get; set; }
}
