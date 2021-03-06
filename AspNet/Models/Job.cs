// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CIS655Project.Models
{
    [Table("Job")]
    public partial class Job
    {
        public Job()
        {
            JobApplications = new HashSet<JobApplication>();
        }

        [Key]
        public int JobId { get; set; }
        public int BusId { get; set; }
        public int JobCatId { get; set; }
        [Required]
        [StringLength(20)]
        public string JobTitle { get; set; }
        [Required]
        [StringLength(50)]
        public string Summary { get; set; }
        [Column(TypeName = "date")]
        public DateTime JobPostDate { get; set; }
        [Required]
        [StringLength(20)]
        public string Location { get; set; }
        [StringLength(20)]
        public string JobType { get; set; }

        [ForeignKey(nameof(BusId))]
        [InverseProperty(nameof(Business.Jobs))]
        public virtual Business Bus { get; set; }
        [ForeignKey(nameof(JobCatId))]
        [InverseProperty(nameof(JobCategory.Jobs))]
        public virtual JobCategory JobCat { get; set; }
        [InverseProperty(nameof(JobApplication.Job))]
        public virtual ICollection<JobApplication> JobApplications { get; set; }
    }
}