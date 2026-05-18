using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Temu_Catarig.Services;

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

    public async Task<string> UploadImageAsync(Stream stream, string fileName)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(fileName, stream),
            Folder = "temu_products"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult?.SecureUrl?.ToString() ?? string.Empty;
    }

    public async Task<string> UploadImageAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return string.Empty;

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(filePath),
            Folder = "temu_products"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult?.SecureUrl?.ToString() ?? string.Empty;
    }
}
