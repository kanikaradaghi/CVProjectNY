using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVProject.Models.Repository.Entity
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Meddelande får inte vara tomt.")]
        [Display(Name = "Meddelande:")]
        public string? Innehall { get; set; }
        public DateTime Datum { get; set; }
        public bool LasDatum { get; set; }
        public string? Avsandare { get; set; } 
        public string Mottagare { get; set; }
        public bool ArRaderad { get; set; }

        [ForeignKey(nameof(Avsandare))]
        public virtual Profile avsandare { get; set; }
        [ForeignKey(nameof(Mottagare))]
        public virtual Profile mottagare { get; set; }
    }
}
