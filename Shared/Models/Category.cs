﻿using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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
        public string Name { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Description { get; set; }

        public List<Post> Posts { get; set; }
    }
}