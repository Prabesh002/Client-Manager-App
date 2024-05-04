using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace Client_Manager_App_Models
{
    public enum ClientType
    {
        empty,
        Low,
        Medium,
        High
    }
    public enum Gender
    {
        NotDisclosed,
        Male,
        Female,
        Other
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
        public string? MaxOffer { get; set; } = "N/D";
        public ClientType? ClientType { get; set; } = Client_Manager_App_Models.ClientType.empty;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public string? ReWorkingRate { get; set; } = "N/D";
        public Gender? Gender { get; set; }  
        public Country? Country { get; set; }
        public string? EditingType { get; set; } = "N/D";
        public bool IsScammer { get; set; } = false;
        public bool HasAgency { get; set; } = false;
        public string? PaymentType { get; set; } = "N/D";
        

    }
}
