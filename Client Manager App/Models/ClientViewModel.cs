using Client_Manager_App.Entities;
using System.ComponentModel.DataAnnotations;

namespace Client_Manager_App.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string? PlatformUrl { get; set; }

        public DateTime TimeEmailSent { get; set; } = DateTime.Now;

        public bool IsRejected { get; set; } = false;

        public string? MaxOffer { get; set; }

        public ClientType? ClientType { get; set; }
    }
}
