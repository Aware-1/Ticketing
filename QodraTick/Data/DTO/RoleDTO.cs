using System.ComponentModel.DataAnnotations;

namespace Data.DTO
{
    public class RoleDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }
    }
}
