using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CVProject.Models.Repository.Entity
{
    public class Profile : IdentityUser
    {
        [Required(ErrorMessage = "Vänligen välj ett användarnamn.")]
        [Display(Name = "Användarnamn:")]
        [MinLength(3, ErrorMessage = "Användarnamnet måste vara minst 3 tecken långt.")]
        [Key]
        public string Anvandarnamn { get; set; }

        [Required(ErrorMessage = "Vänligen skriv förnamn.")]
        [Display(Name = "Förnamn:")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ\s]+$", ErrorMessage = "Förnamn får bara innehålla bokstäver och mellanrum.")]
        public string Fornamn { get; set; }

        [Required(ErrorMessage = "Vänligen skriv efternamn.")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ\s]+$", ErrorMessage = "Efternamn får bara innehålla bokstäver och mellanrum.")]
        public string Efternamn { get; set; }

        [Required(ErrorMessage = "Vänligen skriv ett lösenord.")]
        [Display(Name = "Lösenord:")]
        [MinLength(3, ErrorMessage = "Lösenordet måste vara minst 3 tecken långt.")] //tog kort lösen nu pga enklare kommma ihåg under utvecklingen
        [DataType(DataType.Password)]
        public string Losenord { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i E-postadress.")]
        [Display(Name = "E-postadress:")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9åäöÅÄÖ._-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Fyll i rätt email.")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i telefonnummer.")]
        [Display(Name = "Telefonnummer:")]
        [RegularExpression(@"^[0-9]{3,10}$", ErrorMessage = "Telefonnummer måste vara mellan 3 och 10 siffror.")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Vänligen skriv din adress.")]
        [RegularExpression(@"^[a-zA-Z0-9åäöÅÄÖ\s]+$", ErrorMessage = "Adressen får bara innehålla bokstäver, mellanrum och siffror.")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Vänligen välj om din profil ska vara öppen eller privat.")]
        
        public bool Privat { get; set; }


        public bool  ArInaktiverad { get; set; }

        public int? ImageId { get; set; }

        [ForeignKey(nameof(ImageId))]
        public virtual Images? Image { get; set; }
    }
}
