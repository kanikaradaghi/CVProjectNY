using CVProject.Models.Repository.Entity;

namespace CVProject.Models.ViewModels
{
    public class MessageGuestMessageViewModel
    {
        public Message? Message { get; set; }
        public GuestMessage? GuestMessage { get; set; }
        public List<Message>? Messages { get; set; }
        public List<GuestMessage>? GuestMessages { get; set; }

    }
}
