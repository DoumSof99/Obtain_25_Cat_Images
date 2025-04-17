namespace Obtain_25_Cat_Images.Models.Entities {
    public class CatEntity {
        public int Id { get; set; }
        public string CatId { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; } = string.Empty; //store the image URL rather than saving byte[] to the database
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<CatTagEntity> CatTags { get; set; } = [];
    }
}