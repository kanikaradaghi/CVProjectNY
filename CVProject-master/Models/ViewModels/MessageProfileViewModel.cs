using CVProject.Models.Repository.Entity;
using System.ComponentModel.DataAnnotations;

namespace CVProject.Models.ViewModels
{
    public class MessageProfileViewModel
    {
        public Message? Message { get; set; }
        public Profile? Profile { get; set; }

        public GuestMessage? GuestMessage { get; set; }

        [Required(ErrorMessage = "Meddelande får inte vara tomt.")]
        [Display(Name = "Meddelande:")]
        public string? Innehall { get; set; }
        public List<Message>? Messages { get; }

        public string? Avsandare { get; set; }

    }
}
