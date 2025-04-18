namespace Obtain_25_Cat_Images.DTOs {
    public class CatApiResponseDTO {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public List<CatBreadDTO> Breads { get; set; } = [];
    }

    public class CatBreadDTO {
        public string Temperament { get; set; } = string.Empty;
    }
}
