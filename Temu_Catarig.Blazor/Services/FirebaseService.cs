using Firebase.Database;
using Firebase.Database.Query;
using Temu_Catarig.Blazor.Models;

namespace Temu_Catarig.Blazor.Services;

public class FirebaseService
{
    private const string FirebaseBaseUrl = "https://temu-catarig-default-rtdb.asia-southeast1.firebasedatabase.app";

    private readonly FirebaseClient _client;
    private readonly AuthService _auth;

    public FirebaseService(AuthService auth)
    {
        _auth = auth;
        _client = new FirebaseClient(FirebaseBaseUrl, new FirebaseOptions
        {
            AuthTokenAsyncFactory = () => _auth.GetFreshTokenAsync()
        });
    }

    #region Products
    public async Task<List<Product>> GetProductsAsync(string? status = null)
    {
        try
        {
            var products = await _client
                .Child("products")
                .OnceAsync<Product>();

            var list = products
                .Where(item => item?.Object != null)
                .Select(item => {
                    item.Object.Id = item.Key;
                    return item.Object;
                });

            if (!string.IsNullOrEmpty(status))
            {
                list = list.Where(p => p.Status == status);
            }

            return list.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: GetProductsAsync failed: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task AddProductAsync(Product product)
    {
        try
        {
            product.Status = "Pending";
            await _client
                .Child("products")
                .PostAsync(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: AddProductAsync failed: {ex.Message}");
        }
    }

    public async Task UpdateProductStatusAsync(string productId, string status)
    {
        try
        {
            await _client
                .Child("products")
                .Child(productId)
                .Child("Status")
                .PutAsync($"\"{status}\"");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: UpdateProductStatusAsync failed: {ex.Message}");
        }
    }

    public async Task DeleteProductAsync(string productId)
    {
        try
        {
            await _client
                .Child("products")
                .Child(productId)
                .DeleteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: DeleteProductAsync failed: {ex.Message}");
        }
    }
    #endregion

    #region Orders
    public async Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        try
        {
            var orders = await _client
                .Child("orders")
                .OnceAsync<Order>();

            return orders
                .Where(o => o?.Object != null && o.Object.BuyerId == userId)
                .Select(item => {
                    item.Object.Id = item.Key;
                    return item.Object;
                }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: GetUserOrdersAsync failed: {ex.Message}");
            return new List<Order>();
        }
    }

    public async Task<List<Order>> GetSellerOrdersAsync(string sellerId)
    {
        try
        {
            var orders = await _client
                .Child("orders")
                .OnceAsync<Order>();

            return orders
                .Where(o => o?.Object != null && o.Object.SellerId == sellerId)
                .Select(item => {
                    item.Object.Id = item.Key;
                    return item.Object;
                }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: GetSellerOrdersAsync failed: {ex.Message}");
            return new List<Order>();
        }
    }

    public async Task PlaceOrderAsync(Order order)
    {
        try
        {
            await _client
                .Child("orders")
                .PostAsync(order);

            foreach (var item in order.Items)
            {
                var product = await GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    if (product.Stock < 0) product.Stock = 0;
                    await UpdateProductAsync(product);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: PlaceOrderAsync failed: {ex.Message}");
        }
    }

    public async Task UpdateOrderStatusAsync(string orderId, string status)
    {
        try
        {
            await _client
                .Child("orders")
                .Child(orderId)
                .Child("Status")
                .PutAsync($"\"{status}\"");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: UpdateOrderStatusAsync failed: {ex.Message}");
        }
    }
    #endregion

    #region Cart
    public async Task<List<CartItem>> GetCartItemsAsync(string userId)
    {
        try
        {
            var items = await _client
                .Child("carts")
                .Child(userId)
                .OnceAsync<CartItem>();

            return items
                .Where(item => item?.Object != null)
                .Select(item => item.Object)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: GetCartItemsAsync failed: {ex.Message}");
            return new List<CartItem>();
        }
    }

    public async Task AddToCartAsync(string userId, CartItem item)
    {
        try
        {
            await _client
                .Child("carts")
                .Child(userId)
                .Child(item.ProductId)
                .PutAsync(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: AddToCartAsync failed: {ex.Message}");
        }
    }

    public async Task RemoveFromCartAsync(string userId, string productId)
    {
        try
        {
            await _client
                .Child("carts")
                .Child(userId)
                .Child(productId)
                .DeleteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: RemoveFromCartAsync failed: {ex.Message}");
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        try
        {
            await _client
                .Child("carts")
                .Child(userId)
                .DeleteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: ClearCartAsync failed: {ex.Message}");
        }
    }
    #endregion

    #region Helper Product Methods
    public async Task<Product> GetProductByIdAsync(string productId)
    {
        try
        {
            var product = await _client
                .Child("products")
                .Child(productId)
                .OnceSingleAsync<Product>();

            if (product != null)
            {
                product.Id = productId;
            }
            return product!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: GetProductByIdAsync failed: {ex.Message}");
            return null!;
        }
    }

    public async Task UpdateProductAsync(Product product)
    {
        try
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                throw new Exception("Cannot update product without a valid ID.");
            }

            await _client
                .Child("products")
                .Child(product.Id)
                .PutAsync(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: UpdateProductAsync failed: {ex.Message}");
        }
    }
    #endregion

    #region Messaging
    public async Task<List<Conversation>> GetConversationsAsync(string userId)
    {
        try
        {
            var conversations = await _client
                .Child("conversations")
                .OnceAsync<Conversation>();

            return conversations
                .Where(c => c?.Object != null && (c.Object.SellerId == userId || c.Object.BuyerId == userId))
                .Select(item => {
                    item.Object.Id = item.Key;
                    return item.Object;
                })
                .OrderByDescending(c => c.LastMessageDate)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: GetConversationsAsync failed: {ex.Message}");
            return new List<Conversation>();
        }
    }

    public async Task SendMessageAsync(string conversationId, Message message)
    {
        try
        {
            await _client
                .Child("messages")
                .Child(conversationId)
                .PostAsync(message);

            await _client
                .Child("conversations")
                .Child(conversationId)
                .Child("LastMessage")
                .PutAsync($"\"{message.Text}\"");

            await _client
                .Child("conversations")
                .Child(conversationId)
                .Child("LastMessageDate")
                .PutAsync($"\"{message.Timestamp:O}\"");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: SendMessageAsync failed: {ex.Message}");
        }
    }
    #endregion

    #region User Profile
    public async Task<UserProfile> GetUserProfileAsync(string userId)
    {
        try
        {
            var profile = await _client
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<UserProfile>();

            if (profile != null) profile.Id = userId;
            return profile ?? new UserProfile { Id = userId };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: GetUserProfileAsync failed: {ex.Message}");
            return new UserProfile { Id = userId };
        }
    }

    public async Task SaveUserProfileAsync(UserProfile profile)
    {
        try
        {
            await _client
                .Child("users")
                .Child(profile.Id)
                .PutAsync(profile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: SaveUserProfileAsync failed: {ex.Message}");
        }
    }
    #endregion

    #region Reviews
    public async Task<List<Review>> GetReviewsForProductAsync(string productId)
    {
        try
        {
            var reviews = await _client
                .Child("reviews")
                .OnceAsync<Review>();

            return reviews
                .Where(r => r?.Object != null && r.Object.ProductId == productId)
                .Select(item => {
                    item.Object.Id = item.Key;
                    return item.Object;
                })
                .OrderByDescending(r => r.Timestamp)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: GetReviewsForProductAsync failed: {ex.Message}");
            return new List<Review>();
        }
    }

    public async Task SubmitReviewAsync(Review review)
    {
        try
        {
            await _client
                .Child("reviews")
                .PostAsync(review);
                
            var product = await GetProductByIdAsync(review.ProductId);
            if (product != null)
            {
                double totalRating = product.Rating * product.ReviewsCount;
                product.ReviewsCount++;
                product.Rating = (totalRating + review.Rating) / product.ReviewsCount;
                await UpdateProductAsync(product);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: SubmitReviewAsync failed: {ex.Message}");
        }
    }

    public async Task UpdateOrderReviewStatusAsync(string orderId, bool isReviewed)
    {
        try
        {
            await _client
                .Child("orders")
                .Child(orderId)
                .Child("IsReviewed")
                .PutAsync(isReviewed);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FirebaseService: UpdateOrderReviewStatusAsync failed: {ex.Message}");
        }
    }
    #endregion
}
