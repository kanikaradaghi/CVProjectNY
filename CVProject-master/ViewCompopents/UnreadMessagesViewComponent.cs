using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CVProject.Models.Repository;
using CVProject.Models.Repository.Entity;
using Microsoft.AspNetCore.Identity;

public class UnreadMessagesViewComponent : ViewComponent
{
    private readonly CvContext _context;
    private readonly UserManager<Profile> _userManager;

    public UnreadMessagesViewComponent(CvContext context, UserManager<Profile> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);

        if (user != null)
        {
            // Räkna olästa meddelanden från aktiva användare
            var unreadMessagesCount = await _context.Messages.CountAsync(m => m.mottagare.Id == user.Id && !m.LasDatum && m.ArRaderad == false && m.avsandare.ArInaktiverad == false);

            // Räkna olästa gästmeddelanden
            var guessUnreadMessagesCount = await _context.GuestMessages.CountAsync(m => m.mottagare.Id == user.Id && !m.LasDatum && m.ArRaderad == false);

            // Skickar datan till vyn
            ViewBag.UnreadMessagesCount = unreadMessagesCount + guessUnreadMessagesCount;
        }
        return View();
    }



}

