﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CIS655Project.Models
{
    public partial class UserRole
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserRole")]
        public virtual User User { get; set; }
    }
}