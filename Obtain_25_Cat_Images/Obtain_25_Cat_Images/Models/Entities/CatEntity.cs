using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Obtain_25_Cat_Images.Models.Entities {
    public class CatEntity {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CatId { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; } = string.Empty; 
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<CatTagEntity> CatTags { get; set; } = [];
    }
}