using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CVProject.Models.Repository.Entity
{
    public class CV_Project
    {
        [Key, Column(Order = 0)]
        public int Cvid { get; set; }

        [Key, Column(Order = 1)]
        public int Projectid { get; set; }

        [ForeignKey(nameof(Cvid))]
        public virtual CV CV { get; set; }

        [ForeignKey(nameof(Projectid))]
        public virtual Project Project { get; set; }
    }
}
