using FluentResults;
using Obtain_25_Cat_Images.DTOs;

namespace Obtain_25_Cat_Images.Interfaces {
    public interface ICatService {
        Task<Result<List<CatResponseDTO>>> FetchAndSaveCatsAsync();
        Task<Result<CatResponseDTO>> GetCatByIdAsync(int id);
        Task<Result<List<CatResponseDTO>>> GetCatsAsync(string? tag = null, int page = 1, int pageSize = 10);
    }
}
