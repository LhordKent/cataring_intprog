# Multi-Product Order Splitting Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Each unique product in the cart results in a separate order record, with a single ₱80 shipping fee charged per unique seller.

**Architecture:** Modify `CheckoutPage.xaml.cs` to calculate totals correctly based on unique sellers and refactor the order placement logic to group by `ProductId` and distribute shipping fees.

**Tech Stack:** .NET MAUI, C#, FirebaseService

---

### Task 1: Update Shipping and Total Calculation

**Files:**
- Modify: `Pages/CheckoutPage.xaml.cs:26-31`

- [ ] **Step 1: Update LoadCartDetails to calculate seller-based shipping**

```csharp
    private async Task LoadCartDetails()
    {
        _items = await _firebaseService.GetCartItemsAsync(AuthService.UserId!);
        
        // Calculate unique sellers and total shipping
        var uniqueSellersCount = _items.Select(i => i.SellerId).Distinct().Count();
        double shippingFee = uniqueSellersCount * 80;
        
        _totalAmount = _items.Sum(i => i.Price * i.Quantity) + shippingFee;
        TotalLabel.Text = $"₱{_totalAmount:N2}";
    }
```

- [ ] **Step 2: Verify shipping calculation**
Add items from same/different sellers to cart and verify the displayed total in Checkout.

- [ ] **Step 3: Commit**

```bash
git add Pages/CheckoutPage.xaml.cs
git commit -m "feat: update checkout total calculation to be seller-based"
```

---

### Task 2: Implement Per-Product Order Splitting Logic

**Files:**
- Modify: `Pages/CheckoutPage.xaml.cs:45-79`

- [ ] **Step 1: Refactor OnPlaceOrderClicked to split by ProductId**

```csharp
        try
        {
            // Group by ProductId to ensure each product gets its own order
            var itemsByProduct = _items.GroupBy(i => i.ProductId);
            var processedSellers = new HashSet<string>();

            foreach (var group in itemsByProduct)
            {
                var firstItem = group.First();
                double itemShipping = 0;

                // Only the first product processed for a seller carries the ₱80 fee
                if (!processedSellers.Contains(firstItem.SellerId))
                {
                    itemShipping = 80;
                    processedSellers.Add(firstItem.SellerId);
                }

                var order = new Order
                {
                    BuyerId = AuthService.UserId!,
                    ReceiverName = NameEntry.Text,
                    ReceiverPhone = PhoneEntry.Text,
                    SellerId = firstItem.SellerId,
                    SellerName = firstItem.SellerName,
                    Items = group.ToList(), // This list will contain the same product (multiple quantities if any)
                    Subtotal = group.Sum(i => i.Price * i.Quantity),
                    ShippingFee = itemShipping,
                    TotalPrice = group.Sum(i => i.Price * i.Quantity) + itemShipping,
                    Status = "Pending",
                    PaymentMethod = paymentMethod,
                    OrderDate = DateTime.Now
                };

                await _firebaseService.PlaceOrderAsync(order);
            }

            await _firebaseService.ClearCartAsync(AuthService.UserId!);
            await DisplayAlert("Success", "Order placed successfully!", "OK");
            await Shell.Current.GoToAsync("//LandingPage");
        }
```

- [ ] **Step 2: Verify order splitting**
Place an order with 2 different products from the same seller. Verify "My Orders" shows two separate records.

- [ ] **Step 3: Commit**

```bash
git add Pages/CheckoutPage.xaml.cs
git commit -m "feat: implement per-product order splitting with seller-based shipping distribution"
```
