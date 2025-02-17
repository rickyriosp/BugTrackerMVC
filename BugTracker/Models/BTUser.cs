﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class BTUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName { get => $"{FirstName} {LastName}"; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile AvatarFormFile { get; set; }

        [DisplayName("Avatar")]
        public string? AvatarFileName { get; set; }

        [DisplayName("File Data")]
        public byte[]? AvatarFileData { get; set; }

        [DisplayName("File Extension")]
        public string? AvatarContentType { get; set; }

        [Required]
        [DisplayName("Company")]
        public int CompanyId { get; set; }


        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    }
}
