﻿using System.ComponentModel.DataAnnotations;

namespace Client_Manager_App.Entities
{
    public enum ClientType
    {
        empty,
        Low,
        Medium,
        High
    }
    public class ClientModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        public string? PlatformUrl { get; set; }

        public DateTime TimeEmailSent { get; set; } = DateTime.Now;

        public bool IsRejected { get; set; } = false;

        public string? MaxOffer {  get; set; }

        public ClientType? ClientType { get; set; } = Entities.ClientType.empty;
        
    }
}
