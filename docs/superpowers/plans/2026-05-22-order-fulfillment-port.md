# Order History & Fulfillment Port Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Port order history and fulfillment pages from MAUI to Blazor using Tailwind CSS.

**Architecture:** Use Blazor components with Tailwind CSS for high-fidelity UI. Leverage `FirebaseService` for data and `AuthService` for user context.

**Tech Stack:** Blazor (C#), Tailwind CSS, Firebase Realtime Database.

---

### Task 1: Port My Orders Page

**Files:**
- Create: `Temu_Catarig.Blazor/Pages/Orders.razor`

- [ ] **Step 1: Create the Orders.razor page**

```razor
@page "/orders"
@inject FirebaseService FirebaseService
@inject AuthService AuthService
@inject NavigationManager Navigation

<PageTitle>My Orders | Temu</PageTitle>

<div class="max-w-4xl mx-auto py-8 px-4">
    <div class="flex items-center gap-4 mb-8">
        <button @onclick='() => Navigation.NavigateTo("/profile")' class="p-2 hover:bg-gray-100 rounded-full transition-colors">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m15 18-6-6 6-6"/></svg>
        </button>
        <h1 class="text-2xl font-bold text-gray-900">My Orders</h1>
    </div>

    @if (orders == null)
    {
        <div class="space-y-4">
            @for (int i = 0; i < 3; i++)
            {
                <div class="h-32 bg-gray-100 animate-pulse rounded-2xl"></div>
            }
        </div>
    }
    else if (!orders.Any())
    {
        <div class="text-center py-20 bg-white rounded-3xl border border-dashed border-gray-200">
            <p class="text-gray-500 font-medium">No orders yet.</p>
            <button @onclick='() => Navigation.NavigateTo("/")' class="mt-4 text-temu-orange font-bold hover:underline">Start shopping</button>
        </div>
    }
    else
    {
        <div class="space-y-4">
            @foreach (var order in orders)
            {
                <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-4 md:p-6 transition-all hover:shadow-md">
                    <div class="flex items-center justify-between mb-4">
                        <span class="text-sm font-bold text-gray-900">Order #@order.ShortId</span>
                        <span class="px-3 py-1 rounded-full text-xs font-bold @GetStatusClass(order.Status)">
                            @order.Status.ToUpper()
                        </span>
                    </div>
                    
                    <div class="flex gap-4">
                        <img src="@order.FirstItemImage" class="w-20 h-20 rounded-xl object-cover bg-gray-50 border border-gray-100" />
                        <div class="flex-1 flex flex-col justify-center">
                            <h3 class="text-sm font-medium text-gray-900 line-clamp-1">@order.ItemsSummary</h3>
                            <p class="text-xs text-gray-400 mt-1">@order.DateFormatted</p>
                            <p class="text-base font-bold text-gray-900 mt-2">@order.TotalPriceFormatted</p>
                        </div>
                    </div>

                    @if (order.ShowReviewButton)
                    {
                        <div class="mt-4 pt-4 border-t border-gray-50 flex justify-end">
                            <button @onclick="() => OpenReviewModal(order)" class="bg-temu-orange text-white px-6 py-2 rounded-full text-sm font-bold hover:bg-orange-600 transition-colors">
                                REVIEW
                            </button>
                        </div>
                    }
                </div>
            }
        </div>
    }
</div>

@if (selectedOrder != null)
{
    <!-- Simple Review Modal -->
    <div class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm">
        <div class="bg-white rounded-3xl w-full max-w-md p-6 shadow-2xl">
            <h2 class="text-xl font-bold mb-4">Rate this product</h2>
            <p class="text-sm text-gray-500 mb-6">How was your experience with "@selectedOrder.FirstItemName"?</p>
            
            <div class="flex justify-center gap-2 mb-8">
                @for (int i = 1; i <= 5; i++)
                {
                    var rating = i;
                    <button @onclick="() => currentRating = rating" class="text-3xl transition-transform hover:scale-110">
                        @(currentRating >= i ? "⭐" : "☆")
                    </button>
                }
            </div>

            <textarea @bind="reviewComment" placeholder="What do you think about the product?" 
                      class="w-full h-32 bg-gray-50 rounded-2xl p-4 text-sm focus:outline-none focus:ring-2 focus:ring-temu-orange/20 border-0 mb-6"></textarea>

            <div class="flex gap-3">
                <button @onclick="CloseReviewModal" class="flex-1 py-3 font-bold text-gray-500 hover:bg-gray-50 rounded-full transition-colors">Cancel</button>
                <button @onclick="SubmitReview" class="flex-1 py-3 bg-temu-orange text-white font-bold rounded-full hover:bg-orange-600 transition-colors">Submit</button>
            </div>
        </div>
    </div>
}

@code {
    private List<Order>? orders;
    private Order? selectedOrder;
    private int currentRating = 5;
    private string reviewComment = "";

    protected override async Task OnInitializedAsync()
    {
        if (!AuthService.IsLoggedIn)
        {
            Navigation.NavigateTo("/login");
            return;
        }

        await LoadOrders();
    }

    private async Task LoadOrders()
    {
        orders = await FirebaseService.GetUserOrdersAsync(AuthService.UserId!);
    }

    private void OpenReviewModal(Order order)
    {
        selectedOrder = order;
        currentRating = 5;
        reviewComment = "";
    }

    private void CloseReviewModal() => selectedOrder = null;

    private async Task SubmitReview()
    {
        if (selectedOrder == null || selectedOrder.Items.Count == 0) return;

        var review = new Review
        {
            ProductId = selectedOrder.Items[0].ProductId,
            BuyerId = AuthService.UserId!,
            BuyerName = AuthService.UserDisplayName ?? "Customer",
            Rating = currentRating,
            Comment = reviewComment,
            Timestamp = DateTime.Now
        };

        await FirebaseService.SubmitReviewAsync(review);
        await FirebaseService.UpdateOrderReviewStatusAsync(selectedOrder.Id, true);
        
        CloseReviewModal();
        await LoadOrders();
    }

    private string GetStatusClass(string status) => status switch
    {
        "Delivered" => "bg-green-50 text-green-700",
        "Pending" => "bg-yellow-50 text-yellow-700",
        "Shipped" => "bg-blue-50 text-blue-700",
        "Cancelled" => "bg-red-50 text-red-700",
        _ => "bg-gray-50 text-gray-500"
    };
}
```

- [ ] **Step 2: Run build to verify**

Run: `dotnet build Temu_Catarig.Blazor/Temu_Catarig.Blazor.csproj`
Expected: PASS

- [ ] **Step 3: Commit**

```bash
git add Temu_Catarig.Blazor/Pages/Orders.razor
git commit -m "feat: add My Orders page"
```

---

### Task 2: Port Order Fulfillment Page

**Files:**
- Create: `Temu_Catarig.Blazor/Pages/Admin/Fulfillment.razor`

- [ ] **Step 1: Create the Fulfillment.razor page**

```razor
@page "/admin/fulfillment"
@inject FirebaseService FirebaseService
@inject AuthService AuthService
@inject NavigationManager Navigation

<PageTitle>Order Fulfillment | Temu</PageTitle>

<div class="max-w-4xl mx-auto py-8 px-4">
    <div class="flex items-center gap-4 mb-8">
        <button @onclick='() => Navigation.NavigateTo("/admin/dashboard")' class="p-2 hover:bg-gray-100 rounded-full transition-colors">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m15 18-6-6 6-6"/></svg>
        </button>
        <h1 class="text-2xl font-bold text-gray-900">Order Fulfillment</h1>
    </div>

    @if (orders == null)
    {
        <div class="space-y-4">
            @for (int i = 0; i < 3; i++)
            {
                <div class="h-32 bg-gray-100 animate-pulse rounded-2xl"></div>
            }
        </div>
    }
    else if (!orders.Any())
    {
        <div class="text-center py-20 bg-white rounded-3xl border border-dashed border-gray-200">
            <p class="text-gray-500 font-medium">No orders to fulfill yet.</p>
        </div>
    }
    else
    {
        <div class="space-y-6">
            <h2 class="text-lg font-bold text-gray-900">Pending Orders</h2>
            <div class="space-y-4">
                @foreach (var order in orders)
                {
                    <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-4 md:p-6 transition-all hover:shadow-md">
                        <div class="flex items-center justify-between mb-4">
                            <span class="text-sm font-bold text-gray-900">Order #@order.ShortId</span>
                            <span class="px-3 py-1 rounded-full text-xs font-bold @GetStatusClass(order.Status)">
                                @order.Status.ToUpper()
                            </span>
                        </div>

                        <div class="flex gap-4 mb-4">
                            <img src="@order.FirstItemImage" class="w-16 h-16 rounded-xl object-cover bg-gray-50" />
                            <div class="flex-1">
                                <h3 class="text-sm font-medium text-gray-900 line-clamp-1">@order.ItemsSummary</h3>
                                <p class="text-xs text-gray-400 mt-1">@order.DateFormatted</p>
                            </div>
                        </div>

                        <div class="flex items-center justify-between pt-4 border-t border-gray-50">
                            <div class="text-sm">
                                <span class="text-gray-500">Buyer: </span>
                                <span class="font-medium">@order.ReceiverName</span>
                            </div>
                            
                            @if (order.IsFulfillmentActionVisible)
                            {
                                <button @onclick="() => FulfillOrder(order)" class="bg-temu-orange text-white px-6 py-2 rounded-full text-xs font-bold hover:bg-orange-600 transition-colors">
                                    @order.FulfillmentButtonText
                                </button>
                            }
                        </div>
                    </div>
                }
            </div>
        </div>
    }
</div>

@code {
    private List<Order>? orders;

    protected override async Task OnInitializedAsync()
    {
        if (!AuthService.IsLoggedIn)
        {
            Navigation.NavigateTo("/login");
            return;
        }

        await LoadOrders();
    }

    private async Task LoadOrders()
    {
        orders = await FirebaseService.GetSellerOrdersAsync(AuthService.UserId!);
    }

    private async Task FulfillOrder(Order order)
    {
        string nextStatus = order.Status == "Pending" ? "Shipped" : "Delivered";
        await FirebaseService.UpdateOrderStatusAsync(order.Id, nextStatus);
        await LoadOrders();
    }

    private string GetStatusClass(string status) => status switch
    {
        "Delivered" => "bg-green-50 text-green-700",
        "Pending" => "bg-yellow-50 text-yellow-700",
        "Shipped" => "bg-blue-50 text-blue-700",
        "Cancelled" => "bg-red-50 text-red-700",
        _ => "bg-gray-50 text-gray-500"
    };
}
```

- [ ] **Step 2: Run build to verify**

Run: `dotnet build Temu_Catarig.Blazor/Temu_Catarig.Blazor.csproj`
Expected: PASS

- [ ] **Step 3: Commit**

```bash
git add Temu_Catarig.Blazor/Pages/Admin/Fulfillment.razor
git commit -m "feat: add Order Fulfillment page"
```

---

### Task 3: Update Navigation

**Files:**
- Modify: `Temu_Catarig.Blazor/Layout/NavMenu.razor`
- Modify: `Temu_Catarig.Blazor/Pages/Admin/Dashboard.razor`

- [ ] **Step 1: Add Fulfillment link to Admin Dashboard**

```razor
// In Temu_Catarig.Blazor/Pages/Admin/Dashboard.razor, after the header div
<div class="flex gap-4 mb-8">
    <button @onclick='() => Navigation.NavigateTo("/admin/fulfillment")' 
            class="flex-1 bg-white p-6 rounded-3xl border border-gray-100 shadow-sm hover:shadow-md transition-all text-left group">
        <div class="w-12 h-12 bg-orange-50 rounded-2xl flex items-center justify-center mb-4 group-hover:bg-orange-100 transition-colors">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#ff4700" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
        </div>
        <h3 class="font-bold text-gray-900">Order Fulfillment</h3>
        <p class="text-xs text-gray-500 mt-1">Manage shipping and delivery.</p>
    </button>
</div>
```

- [ ] **Step 2: Update NavMenu.razor (Mobile and Desktop)**

Ensure the "Orders" link points to `/orders`.

- [ ] **Step 3: Run build to verify**

Run: `dotnet build Temu_Catarig.Blazor/Temu_Catarig.Blazor.csproj`
Expected: PASS

- [ ] **Step 4: Commit**

```bash
git add Temu_Catarig.Blazor/Layout/NavMenu.razor Temu_Catarig.Blazor/Pages/Admin/Dashboard.razor
git commit -m "feat: update navigation for orders and fulfillment"
```
