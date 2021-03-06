// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CIS655Project.Models
{
    [Table("JobCategory")]
    public partial class JobCategory
    {
        public JobCategory()
        {
            Jobs = new HashSet<Job>();
        }

        [Key]
        public int JobCatId { get; set; }
        [StringLength(50)]
        public string Category { get; set; }

        [InverseProperty(nameof(Job.JobCat))]
        public virtual ICollection<Job> Jobs { get; set; }
    }
}