# Spec: Order History & Fulfillment Port

Port the order history (buyer) and order fulfillment (seller) pages from MAUI to Blazor using Tailwind CSS.

## 1. My Orders Page (`/orders`)
- **File:** `Temu_Catarig.Blazor/Pages/Orders.razor`
- **Features:**
    - List of orders where `BuyerId == AuthService.UserId`.
    - Display Order ID (ShortId), Date, Status Badge, First Item Image/Name, and Total Price.
    - **Review System:**
        - If `Status == "Delivered"` and `!IsReviewed`, show "REVIEW" button.
        - Clicking "REVIEW" opens a modal to select rating (1-5) and enter a comment.
        - Submitting calls `FirebaseService.SubmitReviewAsync` and `UpdateOrderReviewStatusAsync`.

## 2. Order Fulfillment Page (`/admin/fulfillment`)
- **File:** `Temu_Catarig.Blazor/Pages/Admin/Fulfillment.razor`
- **Features:**
    - List of orders where `SellerId == AuthService.UserId`.
    - Grouped/Listed by "Pending Orders" or all orders.
    - Action button to progress status:
        - "Pending" -> "SHIP ORDER" (sets status to "Shipped")
        - "Shipped" -> "DELIVER" (sets status to "Delivered")
    - Visual feedback on success.

## 3. Data Flow
- **Fetch:** `GetUserOrdersAsync` and `GetSellerOrdersAsync`.
- **Update:** `UpdateOrderStatusAsync`, `SubmitReviewAsync`, `UpdateOrderReviewStatusAsync`.

## 4. UI/UX
- **Design:** Modern Tailwind cards.
- **Badges:**
    - Pending: Yellow (`bg-yellow-100 text-yellow-800`)
    - Shipped: Blue (`bg-blue-100 text-blue-800`)
    - Delivered: Green (`bg-green-100 text-green-800`)
    - Cancelled: Red (`bg-red-100 text-red-800`)
- **Empty States:** Show "No orders yet" or "No orders to fulfill".

## 5. Navigation
- Add "Orders" to `NavMenu.razor` (Desktop and Mobile).
- Add "Fulfillment" to Admin/Dashboard or NavMenu for sellers.
