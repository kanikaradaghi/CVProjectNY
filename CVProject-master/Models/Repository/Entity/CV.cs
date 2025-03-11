using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVProject.Models.Repository.Entity
{
    public class CV
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public byte[]? BildData { get; set; }
        public int? Views { get; set; }
        public string? Kompetenser { get; set; }
        public string? Utbildningar { get; set; }
        public string? TidigareErfarenheter { get; set; }
        public List<Project>? Projekten { get; set; }
        public string AnvandarNamn { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset CreatedDate { get; set; }

        [ForeignKey(nameof(AnvandarNamn))]
        public virtual Profile Profil { get; set; }

        public int? ImageId { get; set; }

        [ForeignKey(nameof(ImageId))]
        public virtual Images? Image { get; set; }
    }
}
