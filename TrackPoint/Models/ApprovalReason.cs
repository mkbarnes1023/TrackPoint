using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace TrackPoint.Models
{
    public class ApprovalReason
    {
        [Key]
        public int ReasonId { get; set; } 
        public string ReasonName { get; set; }
        public string ReasonCode { get; set; }
    }
}
