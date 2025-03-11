using CVProject.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CVProject.Models.Repository.Entity;
using System.Text.RegularExpressions;

namespace CVProject.Controllers
{
    public class ProjectController : Controller
    {
        private readonly CvContext _context;
        private UserManager<Profile> _userManager;

        public ProjectController(CvContext context, UserManager<Profile> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult CreateProject()
        {
            return View();
        }

        public async Task<IActionResult> ShowProject()
        {
            try
            {
                var profileAndProjectList = new List<ProfileAndProject>();
                var projects = _context.Projects.AsNoTracking().ToList();

                foreach (var project in projects)
                {
                    var profileAndProject = new ProfileAndProject();
                    profileAndProject.Projekt = project;
                    profileAndProject.Skapare = await _userManager.FindByIdAsync(project.CreatorID);

                    foreach (var profileProject in _context.ProfileProject.Where(pp => pp.Projectid == project.ProjectID).AsNoTracking().ToList())
                    {
                        var deltagare = await _userManager.FindByIdAsync(profileProject.Profileid);

                        // Kolla om användaren finns, inte är privat,inte avaktiverad eller om användaren är inloggad
                        if (deltagare != null && (!deltagare.Privat || User.Identity.IsAuthenticated) && !deltagare.ArInaktiverad)
                        {
                            profileAndProject.Deltagare.Add(deltagare);
                        }
                    }
                    profileAndProjectList.Add(profileAndProject);
                }

                return View(profileAndProjectList);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }


        public async Task<IActionResult> ShowCreatedProject()
        {
            try
            {
                var profileAndProjectList = new List<ProfileAndProject>();

                var signedInUserId = _userManager.GetUserId(User);

                var projects = _context.Projects
                .Where(p => p.CreatorID == signedInUserId)
                .AsNoTracking()
                .ToList();

                foreach (var project in projects)
                {
                    var profileAndProject = new ProfileAndProject();
                    profileAndProject.Projekt = project;
                    profileAndProject.Skapare = await _userManager.FindByIdAsync(project.CreatorID);
                    foreach (var profileProject in _context.ProfileProject.Where(pp => pp.Projectid == project.ProjectID).AsNoTracking().ToList())
                    {
                        profileAndProject.Deltagare.Add(await _userManager.FindByIdAsync(profileProject.Profileid));
                    }
                    profileAndProjectList.Add(profileAndProject);
                }

                return View(profileAndProjectList);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }
        public IActionResult EditProject(int id)
        {
            try
            {
                var project = _context.Projects.Find(id);

                if (project == null)
                {
                    return NotFound();
                }

                return View(project);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(string Title, string Description)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.Title = Title;
                project.Description = Description;

                Profile creator = await _userManager.GetUserAsync(User);
                project.CreatorID = creator.Id;
                project.DatumProjekt = DateTime.Now;
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                return RedirectToAction("ShowProject", "Project");
            }
            else
            {
                return View("CreateProject");
            }
        }

        public async Task<IActionResult> JoinProject(Project project)
        {
            try
            {
                var profile = await _userManager.GetUserAsync(User);
                var profileProjectOld = _context.ProfileProject.FirstOrDefault(pp => pp.Projectid == project.ProjectID && pp.Profileid == profile.Id);
                if (profileProjectOld == null)
                {
                    var profileProjectNew = new ProfileProject();
                    profileProjectNew.Profileid = profile.Id;
                    profileProjectNew.Projectid = project.ProjectID;
                    _context.ProfileProject.Add(profileProjectNew);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("ShowProject", "Project");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> LeaveProject(Project project)
        {
            try
            {
                var profile = await _userManager.GetUserAsync(User);
                var profileProject = _context.ProfileProject.FirstOrDefault(pp => pp.Projectid == project.ProjectID && pp.Profileid == profile.Id);
                if (profileProject != null)
                {
                    _context.ProfileProject.Remove(profileProject);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("ShowProject", "Project");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(int projectId, string title, string description)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);

                if (project == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(title))
                {
                    project.Title = title;
                }

                if (!string.IsNullOrEmpty(description))
                {
                    project.Description = description;
                }

                _context.Projects.Update(project);
                await _context.SaveChangesAsync();

                return RedirectToAction("ShowCreatedProject");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return RedirectToAction("Error");
            }
        }


    }
}


