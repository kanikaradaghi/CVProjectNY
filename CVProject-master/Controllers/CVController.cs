
using CVProject.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CVProject.Models.Repository.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace CVProject.Controllers
{
    public class CVController : Controller
    {
        private readonly List<string> _validImageFormats = [".png", ".jpeg", ".jpg"];
        private readonly CvContext _context;
        private UserManager<Profile> _userManager;

        public CVController(CvContext context, UserManager<Profile> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ShowCV()
        {
            try
            {
                var cvViewModelList = new List<CVViewModel>();

                var profil = await _userManager.GetUserAsync(User);

                var cv = _context.CVs
                    .AsNoTracking()
                    .Include(c => c.Profil)
                    .Include(c => c.Image)
                    .Where(c => c.AnvandarNamn == profil.Id)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();

                foreach (var cV in cv)
                {
                    var cvViewModel = new CVViewModel();
                    cvViewModel.CV = cV;

                    var projects = _context.CVProject
                    .Where(cp => cp.Cvid == cV.Id)
                    .Select(pp => pp.Project)
                    .ToList();

                    cvViewModel.Projects.AddRange(projects);

                    cvViewModelList.Add(cvViewModel);
                }

                return View(cvViewModelList);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> ShowSingleCVAndIncrement(CV cv)
        {
            try
            {
                var cvViewModel = new CVViewModel();

                var projects = _context.CVProject
                .Where(cp => cp.Cvid == cv.Id)
                .Select(pp => pp.Project)
                .ToList();

                cvViewModel.Projects.AddRange(projects);

                var updatedCv = await _context.CVs.Where(_ => _.Id == cv.Id)
                    .Include(_ => _.Profil)
                    .Include(_ => _.Image)
                    .FirstOrDefaultAsync();

                updatedCv.Views = (updatedCv.Views != null) ? updatedCv.Views + 1 : 1;

                cvViewModel.CV = updatedCv;

                await _context.SaveChangesAsync();

                return View("CvDetails", cvViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }


        public async Task<IActionResult> ShowCVOtherUser(string ID)
        {
            try
            {
                var cvViewModelList = new List<CVViewModel>();

                var profil = await _userManager.FindByIdAsync(ID);

                var cv = _context.CVs
                    .AsNoTracking()
                    .Include(c => c.Profil)
                    .Include(c => c.Image)
                    .Where(c => c.AnvandarNamn == profil.Id)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();

                foreach (var cV in cv)
                {
                    var cvViewModel = new CVViewModel();
                    cvViewModel.CV = cV;

                    var projects = _context.CVProject
                    .Where(cp => cp.Cvid == cV.Id)
                    .Select(pp => pp.Project)
                    .ToList();

                    cvViewModel.Projects.AddRange(projects);

                    cvViewModelList.Add(cvViewModel);
                }

                return View(cvViewModelList);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> ShowSimilarCV(string ID)
        {
            try
            {
                var cvViewModelList = new List<CVViewModel>();

                var profil = await _userManager.FindByIdAsync(ID);

                // Hämta de första tre bokstäverna av förnamn och efternamn
                var firstNamePrefix = profil.Fornamn?.Substring(0, Math.Min(3, profil.Fornamn.Length));
                var lastNamePrefix = profil.Efternamn?.Substring(0, Math.Min(3, profil.Efternamn.Length));

                // Hämta CV:en med matchande förnamn eller efternamn (tre första bokstäver)
                var similarCVs = _context.CVs
                    .AsNoTracking()
                    .Include(c => c.Profil)
                    .Include(c => c.Image)
                    .Where(c =>
                        (c.Profil.Fornamn != null && c.Profil.Fornamn.StartsWith(firstNamePrefix)) ||
                        (c.Profil.Efternamn != null && c.Profil.Efternamn.StartsWith(lastNamePrefix)))
                    .OrderByDescending(c => c.CreatedDate)
                    .ToList();

                foreach (var cV in similarCVs)
                {
                    var cvViewModel = new CVViewModel();
                    cvViewModel.CV = cV;

                    var projects = _context.CVProject
                    .Where(cp => cp.Cvid == cV.Id)
                    .Select(pp => pp.Project)
                    .ToList();

                    cvViewModel.Projects.AddRange(projects);

                    cvViewModelList.Add(cvViewModel);
                }

                return View(cvViewModelList);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCV(int id)
        {
            try
            {
                var existingCV = await _context.CVs
                    .Include(c => c.Image)
                    .FirstOrDefaultAsync(c => c.Id == id);

                var profile = await _userManager.GetUserAsync(User);

                var profileProjects = _context.ProfileProject
                    .Where(pp => pp.Profileid == profile.Id)
                    .Select(pp => pp.Project)
                    .ToList();

                var CVProjects = _context.CVProject
                    .Where(cp => cp.Cvid == id)
                    .Select(pp => pp.Project)
                    .ToList();

                var allProjects = new List<Project>();
                allProjects.AddRange(CVProjects);
                allProjects.AddRange(profileProjects);

                var uniqueProjects = allProjects.DistinctBy(project => project.ProjectID).ToList();

                if (existingCV != null)
                {
                    var cvViewModel = new CVViewModel
                    {
                        CV = existingCV,
                    
                    // Hämtar den deta vi vill visa på redigeringssidan
                    };

                    cvViewModel.Projects.AddRange(uniqueProjects);

                    return View(cvViewModel);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> CreateCV()
        {
            try
            {
                CVViewModel nyModel = new CVViewModel();
                var profile = await _userManager.GetUserAsync(User);
                var profileProjects = _context.ProfileProject
                    .Where(pp => pp.Profileid == profile.Id)
                    .Select(pp => pp.Project)
                    .ToList();

                nyModel.Projects.AddRange(profileProjects);
                return View(nyModel);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateCV(CVViewModel cvViewModel)
        {
            try
            {
                var profil = await _userManager.GetUserAsync(User) ?? throw new Exception("Could not find user");
                cvViewModel.CV.AnvandarNamn = profil.Id;

                var imageFile = cvViewModel.ImageFile;
                cvViewModel.CV.Views = 0;

                //Skapar en bild on den finns
                if (IsValidImageFormat(imageFile?.FileName) && imageFile?.Length > 0)
                {
                    var image = CreateImage(imageFile);
                    cvViewModel.CV.Image = image;
                }

                _context.CVs.Add(cvViewModel.CV);
                await _context.SaveChangesAsync();

                foreach (var projectID in cvViewModel.SelectedProjectIds)
                {
                    var cvProject = new CV_Project();
                    cvProject.Cvid = cvViewModel.CV.Id;
                    cvProject.Projectid = projectID;
                    _context.CVProject.Add(cvProject);
                }
                await _context.SaveChangesAsync();

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

        [HttpPost]
        public async Task<IActionResult> EditCV(CVViewModel cvViewModel)
        {
            try
            {
                var profil = await _userManager.GetUserAsync(User);
                var existingCV = _context.CVs.FirstOrDefault(c => c.Id == cvViewModel.CV.Id && c.AnvandarNamn == profil.Id);

                var CVProjects = _context.CVProject
                    .Where(cp => cp.Cvid == cvViewModel.CV.Id)
                    .Select(pp => pp.Project)
                    .ToList();

                var profileProjects = _context.ProfileProject
                    .Where(pp => pp.Profileid == profil.Id)
                    .Select(pp => pp.Project)
                    .ToList();

                if (existingCV != null)
                {
                    if (cvViewModel.ImageFile != null && cvViewModel.ImageFile.Length > 0)
                    {
						var newImage = CreateImage(cvViewModel.ImageFile);
						existingCV.Image = newImage;  

					}

                    existingCV.Kompetenser = cvViewModel.CV.Kompetenser;
                    existingCV.Utbildningar = cvViewModel.CV.Utbildningar;
                    existingCV.TidigareErfarenheter = cvViewModel.CV.TidigareErfarenheter;

                    foreach (var projectID in cvViewModel.SelectedProjectIds)
                    {
                        if (!CVProjects.Any(Project => Project.ProjectID == projectID))
                        {
                            var cvProject = new CV_Project();
                            cvProject.Cvid = cvViewModel.CV.Id;
                            cvProject.Projectid = projectID;
                            _context.CVProject.Add(cvProject);
                        }
                    }

                    foreach (var project in CVProjects)
                    {
                        if (!cvViewModel.SelectedProjectIds.Contains( project.ProjectID ))
                        {
                            var projectToRemove = _context.CVProject.Find(cvViewModel.CV.Id, project.ProjectID);

                            if (projectToRemove != null)
                            {
                                _context.CVProject.Remove(projectToRemove);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction("ShowCV", "CV");
                }

                return NotFound();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }
    }
}
