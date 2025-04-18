namespace Obtain_25_Cat_Images.DTOs {
    public class CatResponseDTO {
        public string Id { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public List<TagResponseDTO> Tags { get; set; } = [];
    }

    public class TagResponseDTO {
        public string Name { get; set; } = string.Empty;
    }
}
