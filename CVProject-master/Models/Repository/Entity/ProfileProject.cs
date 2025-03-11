
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVProject.Models.Repository.Entity
{
    public class ProfileProject
    {
        [Key, Column(Order = 0)]
        public string Profileid { get; set; }

        [Key, Column(Order = 1)]
        public int Projectid { get; set; }

        [ForeignKey(nameof(Profileid))]
        public virtual Profile Profile { get; set; }

        [ForeignKey(nameof(Projectid))]
        public virtual Project Project { get; set; }
    }
}
