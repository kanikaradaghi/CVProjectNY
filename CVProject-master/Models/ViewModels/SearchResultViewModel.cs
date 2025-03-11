using CVProject.Models.Repository.Entity;

namespace CVProject.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public Profile Profil { get; set; } = null!;
        public List<CV>? CV { get; set; }
    }
}
