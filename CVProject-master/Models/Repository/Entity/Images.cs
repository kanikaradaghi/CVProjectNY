using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CVProject.Models.Repository.Entity
{
    public class Images
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public byte[]? Image { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }
}
