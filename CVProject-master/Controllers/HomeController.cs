using CVProject.Models;
using CVProject.Models.Repository;
using CVProject.Models.Repository.Entity;
using CVProject.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CVProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly CvContext _context;

        public HomeController(CvContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoggedInStart");
            }

            return RedirectToAction("LoggedOutStart");
        }

        public IActionResult LoggedInStart()
        {
            try
            {
                var CVList = _context.CVs
                    .AsNoTracking()
                    .Include(c => c.Profil)
                    .Include(c => c.Projekten)
                    .Include(c => c.Image)
                    .Where(c => c.Profil.ArInaktiverad == false)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();

                var project = _context.Projects
                    .AsNoTracking()
                    .OrderByDescending(p => p.DatumProjekt)
                    .FirstOrDefault();

                var AllCvProject = new All_CV_Project { CVs = CVList, Project = project };
                return View(AllCvProject);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public IActionResult LoggedOutStart()
        {
            try
            {
                var CVList = _context.CVs
                    .AsNoTracking()
                    .Include(c => c.Profil)
                    .Include(c => c.Projekten)
                    .Include(c => c.Image)
                    .Where(c => c.Profil.Privat == false && c.Profil.ArInaktiverad == false)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();

                var AllCvProject = new All_CV_Project { CVs = CVList };

                return View(AllCvProject);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Search([FromQuery] string profileName)
        {
            try
            {
                if (profileName == null)
                {
                    return View(new List<SearchResultViewModel>());
                }

                var upperProfileName = profileName.ToUpper();
                var usersQuery = _context.Profiles
                    .AsNoTracking()
                    .Where(p =>
                        p.NormalizedUserName.StartsWith(upperProfileName) ||
                        p.Fornamn.ToUpper().StartsWith(upperProfileName) ||
                        _context.CVs.Any(cv => cv.Profil != null && cv.Profil.Id == p.Id &&
                            (cv.Kompetenser != null && (cv.Profil.Fornamn + " " + cv.Kompetenser).StartsWith(profileName) ||
                            cv.Kompetenser.StartsWith(profileName))));

                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    usersQuery = usersQuery.Where(p => !p.ArInaktiverad);
                }
                else
                {
                    usersQuery = usersQuery.Where(p => !p.Privat && !p.ArInaktiverad);
                }

                var profiles = await usersQuery.ToListAsync();

                var resultView = new List<SearchResultViewModel>();
                foreach (var profile in profiles)
                {
                    var result = new SearchResultViewModel() { Profil = profile };
                    var cvs = _context.CVs
                        .AsNoTracking()
                        .Where(cv => cv.Profil != null && cv.Profil.Id == profile.Id)
                        .ToList();

                    result.CV = cvs ?? new List<CV>();
                    resultView.Add(result);
                }

                return View(resultView);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }
    }
}
