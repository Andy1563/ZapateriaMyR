using Microsoft.AspNetCore.Http;

namespace ZapateriaMR.Web.Services;

public interface IImageStorageService
{
    Task<string?> SaveProductImageAsync(IFormFile? imageFile);
}