namespace Obtain_25_Cat_Images.Models.Entities {
    public class CatTagEntity {
        public int CatId { get; set; }
        public CatEntity Cat { get; set; } = null!;

        public int TagId { get; set; }
        public TagEntity Tag { get; set; } = null!; 
    }
}
