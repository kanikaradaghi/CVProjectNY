using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CVProject.Models.Repository.Entity
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i titel.")]
        [Display(Name = "Titel:")]
        [RegularExpression(@"^[a-zA-Z0-9åäöÅÄÖ\s]+$", ErrorMessage = "Titel får bara innehålla bokstäver och mellanrum")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i beskrivning.")]
        [Display(Name = "Beskrivning:")]
        [RegularExpression(@"^[a-zA-Z0-9åäöÅÄÖ\s]+$", ErrorMessage = "Beskrivning får bara innehålla bokstäver och mellanrum")]
        public string? Description { get; set; }
        public string CreatorID { get; set; }

        [ForeignKey(nameof(CreatorID))]
        public virtual Profile AnvandarNamn { get; set; }

        public int? CVid { get; set; }

        [Display(Name = "Start datum:")]
        public DateTime? DatumProjekt { get; set; }
    }
}

