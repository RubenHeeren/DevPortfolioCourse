using Shared.Models.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class PostDTO
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [MaxLength(128)]
        [NoPeriods(ErrorMessage = "The post Title field contains one or more period characters (.). Please remove all periods.")]
        [NoThreeOrMoreSpacesInARow(ErrorMessage = "The post Title field contains three or more spaces in a row. Please remove them.")]
        public string Title { get; set; }

        [Required]
        [MaxLength(256)]
        public string ThumbnailImagePath { get; set; }

        [Required]
        [MaxLength(512)]
        public string Excerpt { get; set; }

        [MaxLength(65536)]
        public string Content { get; set; }

        [Required]
        public bool Published { get; set; }

        [Required]
        [MaxLength(128)]
        public string Author { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}