using System.Net.Http;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;

namespace Temu_Catarig.Blazor.Services;

public class ImageService
{
    private const string CloudName = "ddmwb1hu9";
    private const string ApiKey = "234812939458311";
    private const string ApiSecret = "elGgmirWC8nd0xn3f6W8mWr2Bg4";

    private readonly HttpClient _httpClient;

    public ImageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> UploadImageAsync(IBrowserFile file)
    {
        try
        {
            if (file == null) return string.Empty;

            // 1. Read stream into memory safely (Max 5MB)
            using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            // 2. Convert to Base64 (Data URI)
            string base64 = Convert.ToBase64String(bytes);
            string dataUri = $"data:{file.ContentType};base64,{base64}";

            // 3. Prepare Timestamp
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            // 4. Create Signature
            // String format: timestamp=[value][secret]
            var signatureString = $"timestamp={timestamp}{ApiSecret}";
            var signature = ComputeSha1Hash(signatureString);

            // 5. Prepare Form Content
            var formData = new Dictionary<string, string>
            {
                { "file", dataUri },
                { "api_key", ApiKey },
                { "timestamp", timestamp },
                { "signature", signature }
            };

            var content = new FormUrlEncodedContent(formData);

            // 6. Send Request
            var response = await _httpClient.PostAsync($"https://api.cloudinary.com/v1_1/{CloudName}/image/upload", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"ImageService: Cloudinary Error Body: {errorBody}");
                return string.Empty;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("secure_url").GetString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ImageService: UploadImageAsync failed: {ex.Message}");
            return string.Empty;
        }
    }

    private string ComputeSha1Hash(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA1.HashData(inputBytes);
        
        // Convert to lowercase hex string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
