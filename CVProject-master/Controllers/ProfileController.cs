using CVProject.Models.Repository;
using CVProject.Models.Repository.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVProject.Controllers
{
    public class ProfileController : Controller
    {
        private readonly List<string> _validImageFormats = [".png", ".jpeg", ".jpg"];
        private readonly CvContext _context;
        private UserManager<Profile> userManager;
        private SignInManager<Profile> signInManager;

        public ProfileController(CvContext context, UserManager<Profile> userMgr, SignInManager<Profile> signInmag)
        {
            _context = context;
            this.userManager = userMgr;
            this.signInManager = signInmag;
        }

        [HttpGet]
        public async Task<IActionResult> ShowProfile()
        {
            try
            {
                var profil = await userManager.GetUserAsync(User);
                var profileWithImage = _context.Profiles
                    .AsNoTracking()
                    .Include(c => c.Image)
                    .Where(c => c.Anvandarnamn == profil.Anvandarnamn)
                    .FirstOrDefault();

                return View(profileWithImage);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> ShowProfileOtherUser(string ID)
        {
            try
            {
                var profil = await userManager.FindByIdAsync(ID);
                return View(profil);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public IActionResult LogInUser()
        {
            try
            {
                Profile Loggprofil = new Profile();
                return View(Loggprofil);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogInUser(Profile Loggprofil)
        {
            try
            {
                if (string.IsNullOrEmpty(Loggprofil.Anvandarnamn) || string.IsNullOrEmpty(Loggprofil.Losenord))
                {
                    TempData["FelMeddelande"] = "Fel användarnamn eller lösenord.";
                    return View(Loggprofil);
                }


                var resultat = await signInManager.PasswordSignInAsync(
                Loggprofil.Anvandarnamn,
                Loggprofil.Losenord,
                isPersistent: false,
                lockoutOnFailure: false
                );

                if (resultat.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                TempData["FelMeddelande"] = "Fel användarnamn eller lösenord.";
                return View(Loggprofil);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(Profile profil, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    profil.UserName = profil.Anvandarnamn;
                    profil.PasswordHash = userManager.PasswordHasher.HashPassword(profil, profil.Losenord);

                    ////Skapar en bild on den finns
                    if (IsValidImageFormat(imageFile?.FileName) && imageFile?.Length > 0)
                    {
                        var image = CreateImage(imageFile);
                        profil.Image = image;
                    }
                    await userManager.CreateAsync(profil);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("RegisterUser");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> EditProfile()
        {
            var profil = await userManager.GetUserAsync(User);
            return View(profil);
        }

        public async Task<IActionResult> Update(string fornamn, string efternamn, string losenord, string email, string phonenumber, string adress, bool? privat)
        {
            try
            {
                var profil = await userManager.GetUserAsync(User);

                if (profil != null)
                {
                    if (!string.IsNullOrEmpty(fornamn))
                    {
                        profil.Fornamn = fornamn;
                    }

                    if (!string.IsNullOrEmpty(efternamn))
                    {
                        profil.Efternamn = efternamn;
                    }

                    if (!string.IsNullOrEmpty(losenord))
                    {
                        profil.Losenord = losenord;
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        profil.Email = email;
                    }

                    if (!string.IsNullOrEmpty(phonenumber))
                    {
                        profil.PhoneNumber = phonenumber;
                    }

                    if (!string.IsNullOrEmpty(adress))
                    {
                        profil.Adress = adress;
                    }

                    if (privat.HasValue)
                    {
                        profil.Privat = privat.Value;
                    }

                    profil.PasswordHash = userManager.PasswordHasher.HashPassword(profil, profil.Losenord);
                    _context.Profiles.Update(profil);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("ShowProfile");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> InactivateAccount()
        {
            try
            {
                var profil = await userManager.GetUserAsync(User);
                profil.ArInaktiverad = true;
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        private bool IsValidImageFormat(string? fileName)
        {
            if (fileName == null) return false;
            foreach (var format in _validImageFormats)
            {
                if (fileName.EndsWith(format, StringComparison.InvariantCultureIgnoreCase)) return true;
            }

            return false;
        }

        private Images CreateImage(IFormFile file)
        {
            var image = new Images();
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                image.Image = ms.ToArray();
            }
            image.FileName = file.FileName;
            image.FileType = file.FileName[(file.FileName.LastIndexOf('.') + 1)..].ToUpper();

            return image;
        }
    }
}
