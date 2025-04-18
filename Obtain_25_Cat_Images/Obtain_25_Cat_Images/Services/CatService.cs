using FluentResults;
using Obtain_25_Cat_Images.DTOs;
using Obtain_25_Cat_Images.Interfaces;

namespace Obtain_25_Cat_Images.Services {
    public class CatService : ICatService {
        public Task<Result<List<CatResponseDTO>>> FetchAndSaveCatsAsync() {
            throw new NotImplementedException();
        }

        public Task<Result<CatResponseDTO>> GetCatByIdAsync(int id) {
            throw new NotImplementedException();
        }

        public Task<Result<List<CatResponseDTO>>> GetCatsAsync(string? tag = null, int page = 1, int pageSize = 10) {
            throw new NotImplementedException();
        }
    }
}
