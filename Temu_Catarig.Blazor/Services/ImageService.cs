using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components.Forms;

namespace Temu_Catarig.Blazor.Services;

public class ImageService
{
    private const string CloudName = "ddmwb1hu9";
    private const string ApiKey = "234812939458311";
    private const string ApiSecret = "elGgmirWC8nd0xn3f6W8mWr2Bg4";

    private readonly Cloudinary _cloudinary;

    public ImageService()
    {
        Account account = new Account(CloudName, ApiKey, ApiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImageAsync(IBrowserFile file)
    {
        try
        {
            // Set max allowed size to 5MB
            using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.Name, stream),
                Folder = "temu_products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result?.SecureUrl?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ImageService: UploadImageAsync failed: {ex.Message}");
            return string.Empty;
        }
    }
}
