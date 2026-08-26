using System.ComponentModel.DataAnnotations;

namespace Scheduler.API.Models.Organization
{
    public class OrganizationBillingSettingsRequest
    {
        [Required]
        public Guid OrganizationId { get; set; }
        
        [Required]
        [Range(1, 3, ErrorMessage = "ServiceRateForBilling must be 1 (Default), 2 (Service-Specific), or 3 (Time-Based)")]
        public int ServiceRateForBilling { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "DefaultBillingRate must be non-negative")]
        public decimal DefaultBillingRate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "DefaultWageRate must be non-negative")]
        public decimal DefaultWageRate { get; set; }
        
        public List<OrganizationTimeBasedRateRequest> TimeBasedRates { get; set; } = new List<OrganizationTimeBasedRateRequest>();
    }
}
