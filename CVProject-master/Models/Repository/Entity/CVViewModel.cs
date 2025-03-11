namespace CVProject.Models.Repository.Entity
{
    public class CVViewModel
    {
        public CV CV { get; set; }
        public List<Project>? Projects { get; set; }
        public List<int>? SelectedProjectIds { get; set; }
        public IFormFile ImageFile { get; set; }
        public CVViewModel()
        {
            Projects = new List<Project>();
            SelectedProjectIds = new List<int>();
        }
    }
}
