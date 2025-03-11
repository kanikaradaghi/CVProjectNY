using CVProject.Models.Repository;
using CVProject.Models.Repository.Entity;
using CVProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVProject.Controllers
{
    public class MessageController : Controller
    {
        private readonly CvContext _context;
        private UserManager<Profile> _userManager;

        public MessageController(CvContext context, UserManager<Profile> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Inbox()
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var messages = await _context.Messages
                    .AsNoTracking()
                    .Include(_ => _.avsandare)
                    .Where(_ => _.mottagare.Id == user.Id.ToString())
                    .ToListAsync();

                var guestMessages = await _context.GuestMessages.Where(_ => _.mottagare.Id == user.Id.ToString()).ToListAsync();

                var viewModel = new MessageGuestMessageViewModel
                {
                    Messages = messages,
                    GuestMessages = guestMessages
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }


        [HttpPost]
        public IActionResult MarkAsRead(int messageId)
        {
            try
            {
                var meddelandet = _context.Messages.Find(messageId);

                var guesstMeddelandet = _context.GuestMessages.Find(messageId);

                if (meddelandet != null && meddelandet.LasDatum == false)
                {
                    meddelandet.LasDatum = true;
                }
                else if (meddelandet != null && meddelandet.LasDatum == true)
                {
                    meddelandet.LasDatum = false;
                }

                if (guesstMeddelandet != null && guesstMeddelandet.LasDatum == false)
                {
                    guesstMeddelandet.LasDatum = true;
                }
                else if (guesstMeddelandet != null && guesstMeddelandet.LasDatum == true)
                {
                    guesstMeddelandet.LasDatum = false;
                }

                _context.SaveChanges();

                return RedirectToAction("Inbox");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }


        [HttpPost]
        public IActionResult MarkAsDeleted(int messageId)
        {
            try
            {
                var meddelandet = _context.Messages.Find(messageId);

                var guestMeddelandet = _context.GuestMessages.Find(messageId);
                if (meddelandet != null)
                {
                    meddelandet.ArRaderad = true;
                }
                if (guestMeddelandet != null)
                {
                    guestMeddelandet.ArRaderad = true;
                }
                _context.SaveChanges();
                return RedirectToAction("Inbox");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public IActionResult SendMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageProfileViewModel viewModel, string id)
        {
            try
            {
                var profil = await _userManager.GetUserAsync(User);
                if (profil == null)
                {
                    if (ModelState.IsValid)
                    {
                        var message = new GuestMessage
                        {
                            Innehall = viewModel.Innehall,
                            Datum = DateTime.Now,
                            LasDatum = false,
                            Avsandare = viewModel.Avsandare,
                            Mottagare = id
                        };

                        _context.GuestMessages.Add(message);

                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var message = new Message
                        {
                            Innehall = viewModel.Innehall,
                            Datum = DateTime.Now,
                            LasDatum = false,
                            Avsandare = profil.Id,
                            Mottagare = id
                        };
                        _context.Messages.Add(message);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }
    }
}
