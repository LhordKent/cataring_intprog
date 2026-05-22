using Firebase.Database;
using Firebase.Database.Query;
using Temu_Catarig.Models;

namespace Temu_Catarig.Services;

public class FirebaseService
{
    private const string FirebaseBaseUrl = "https://temu-catarig-default-rtdb.asia-southeast1.firebasedatabase.app/";

    private readonly FirebaseClient _client;

    public FirebaseService()
    {
        _client = new FirebaseClient(FirebaseBaseUrl, new FirebaseOptions
        {
            AuthTokenAsyncFactory = () => AuthService.GetFreshTokenAsync()
        });
    }

    #region Products
    public async Task<List<Product>> GetProductsAsync(string? status = null)
    {
        var products = await _client
            .Child("products")
            .OnceAsync<Product>();

        var list = products.Select(item => {
            item.Object.Id = item.Key;
            return item.Object;
        });

        if (!string.IsNullOrEmpty(status))
        {
            list = list.Where(p => p.Status == status);
        }

        return list.ToList();
    }

    public async Task AddProductAsync(Product product)
    {
        product.Status = "Approved"; // Auto-approve all new products
        await _client
            .Child("products")
            .PostAsync(product);
    }

    public async Task UpdateProductStatusAsync(string productId, string status)
    {
        await _client
            .Child("products")
            .Child(productId)
            .Child("Status")
            .PutAsync($"\"{status}\"");
    }

    public async Task DeleteProductAsync(string productId)
    {
        await _client
            .Child("products")
            .Child(productId)
            .DeleteAsync();
    }
    #endregion

    #region Orders
    public async Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        var orders = await _client
            .Child("orders")
            .OnceAsync<Order>();

        return orders
            .Where(o => o.Object.BuyerId == userId)
            .Select(item => {
                item.Object.Id = item.Key;
                return item.Object;
            }).ToList();
    }

    public async Task<List<Order>> GetSellerOrdersAsync(string sellerId)
    {
        var orders = await _client
            .Child("orders")
            .OnceAsync<Order>();

        return orders
            .Where(o => o.Object.SellerId == sellerId)
            .Select(item => {
                item.Object.Id = item.Key;
                return item.Object;
            }).ToList();
    }

    public async Task PlaceOrderAsync(Order order)
    {
        // 1. Post the order
        await _client
            .Child("orders")
            .PostAsync(order);

        // 2. Deduct stock for each item
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

    public async Task UpdateOrderStatusAsync(string orderId, string status)
    {
        await _client
            .Child("orders")
            .Child(orderId)
            .Child("Status")
            .PutAsync($"\"{status}\"");
    }
    #endregion

    #region Cart
    public async Task<List<CartItem>> GetCartItemsAsync(string userId)
    {
        var items = await _client
            .Child("carts")
            .Child(userId)
            .OnceAsync<CartItem>();

        return items.Select(item => item.Object).ToList();
    }

    public async Task AddToCartAsync(string userId, CartItem item)
    {
        await _client
            .Child("carts")
            .Child(userId)
            .Child(item.ProductId)
            .PutAsync(item);
    }

    public async Task RemoveFromCartAsync(string userId, string productId)
    {
        await _client
            .Child("carts")
            .Child(userId)
            .Child(productId)
            .DeleteAsync();
    }

    public async Task ClearCartAsync(string userId)
    {
        await _client
            .Child("carts")
            .Child(userId)
            .DeleteAsync();
    }
    #endregion

    #region Helper Product Methods
    public async Task<Product> GetProductByIdAsync(string productId)
    {
        var product = await _client
            .Child("products")
            .Child(productId)
            .OnceSingleAsync<Product>();

        if (product != null)
        {
            product.Id = productId;
        }
        return product;
    }

    public async Task UpdateProductAsync(Product product)
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
    #endregion

    #region Messaging
    public async Task<List<Conversation>> GetConversationsAsync(string userId)
    {
        var conversations = await _client
            .Child("conversations")
            .OnceAsync<Conversation>();

        return conversations
            .Where(c => c.Object.SellerId == userId || c.Object.BuyerId == userId)
            .Select(item => {
                item.Object.Id = item.Key;
                return item.Object;
            })
            .OrderByDescending(c => c.LastMessageDate)
            .ToList();
    }

    public async Task SendMessageAsync(string conversationId, Message message)
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
    #endregion
    #region User Profile
    public async Task<UserProfile> GetUserProfileAsync(string userId)
    {
        var profile = await _client
            .Child("users")
            .Child(userId)
            .OnceSingleAsync<UserProfile>();

        if (profile != null) profile.Id = userId;
        return profile ?? new UserProfile { Id = userId };
    }

    public async Task SaveUserProfileAsync(UserProfile profile)
    {
        await _client
            .Child("users")
            .Child(profile.Id)
            .PutAsync(profile);
    }
    #endregion
    #region Reviews
    public async Task<List<Review>> GetReviewsForProductAsync(string productId)
    {
        var reviews = await _client
            .Child("reviews")
            .OnceAsync<Review>();

        return reviews
            .Where(r => r.Object.ProductId == productId)
            .Select(item => {
                item.Object.Id = item.Key;
                return item.Object;
            })
            .OrderByDescending(r => r.Timestamp)
            .ToList();
    }

    public async Task SubmitReviewAsync(Review review)
    {
        await _client
            .Child("reviews")
            .PostAsync(review);
            
        // Optional: Update product aggregate rating
        var product = await GetProductByIdAsync(review.ProductId);
        if (product != null)
        {
            double totalRating = product.Rating * product.ReviewsCount;
            product.ReviewsCount++;
            product.Rating = (totalRating + review.Rating) / product.ReviewsCount;
            await UpdateProductAsync(product);
        }
    }

    public async Task UpdateOrderReviewStatusAsync(string orderId, bool isReviewed)
    {
        await _client
            .Child("orders")
            .Child(orderId)
            .Child("IsReviewed")
            .PutAsync(isReviewed);
    }
    #endregion
}
