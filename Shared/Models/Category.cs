using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Shared.Models.CustomValidations;

namespace Shared.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(256)]
        public string ThumbnailImagePath { get; set; }

        [Required]
        [MaxLength(128)]
        [NoPeriods(ErrorMessage = "The category Name field contains one or more period characters (.). Please remove all periods.")]
        [NoThreeOrMoreSpacesInARow(ErrorMessage = "The category Name field contains three or more spaces in a row. Please remove them.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Description { get; set; }

        public List<Post> Posts { get; set; }
    }
}