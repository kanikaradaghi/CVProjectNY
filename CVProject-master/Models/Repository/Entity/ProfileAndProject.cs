namespace CVProject.Models.Repository.Entity
{
    public class ProfileAndProject
    {
        public Project Projekt {  get; set; }
        public Profile Skapare { get; set; }
        public List<Profile> Deltagare { get; set; }

        public ProfileAndProject()
        {
            Deltagare = new List<Profile>();
        }
    }


    
}
