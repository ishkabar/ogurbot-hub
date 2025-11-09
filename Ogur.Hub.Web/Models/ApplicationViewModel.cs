using System.ComponentModel.DataAnnotations;

namespace Ogur.Hub.Web.Models
{
    public class ApplicationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Application name is required")]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "Application Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Version is required")]
        [Display(Name = "Version")]
        [RegularExpression(@"^\d+\.\d+\.\d+$", ErrorMessage = "Version must be in format X.Y.Z")]
        public string Version { get; set; } = "1.0.0";

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}
