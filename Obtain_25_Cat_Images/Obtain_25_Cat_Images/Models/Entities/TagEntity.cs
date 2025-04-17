namespace Obtain_25_Cat_Images.Models.Entities {
    public class TagEntity {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<CatTagEntity> CatTags { get; set; } = new List<CatTagEntity>();
    }
}
