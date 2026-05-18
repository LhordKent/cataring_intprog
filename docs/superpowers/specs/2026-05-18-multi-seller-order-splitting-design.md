# Design Spec: Multi-Product Order Splitting

## Problem Statement
Users expect each distinct product they purchase to appear as a separate order in their history. However, shipping fees should be fair: if multiple products come from the same seller, the user should only be charged one shipping fee (₱80) for that seller's entire batch.

## Proposed Solution: Per-Product Order Splitting
Every unique product (by `ProductId`) in the cart results in an independent `Order` record. Shipping fees are calculated per unique `SellerId`.

### Architecture & Data Flow
The implementation will be focused on `Pages/CheckoutPage.xaml.cs`.

1.  **Checkout Preparation (`LoadCartDetails`)**:
    *   Retrieve cart items.
    *   **Shipping Fee Calculation**:
        *   `UniqueSellersCount` = Number of unique `SellerId`s in the cart.
        *   `TotalShippingFee` = `UniqueSellersCount` * 80.
    *   **Grand Total Calculation**:
        *   `TotalAmount` = `Sum(Item.Price * Item.Quantity) + TotalShippingFee`.
    *   Display `TotalAmount` in the `TotalLabel`.

2.  **Order Placement (`OnPlaceOrderClicked`)**:
    *   **Grouping**: Group `_items` by `ProductId`.
    *   **Shipping Distribution Logic**:
        *   Use a `HashSet<string>` (e.g., `processedSellers`) to track sellers that have already been charged the shipping fee.
        *   Iterate through each product group:
            *   Create a new `Order` object.
            *   `Items` = List containing the single product's details.
            *   If `SellerId` is not in `processedSellers`:
                *   `ShippingFee = 80`.
                *   Add `SellerId` to `processedSellers`.
            *   Else:
                *   `ShippingFee = 0`.
            *   `TotalPrice = (ItemPrice * Quantity) + ShippingFee`.
            *   Save the `Order` via `FirebaseService.PlaceOrderAsync`.
    *   Clear the cart after all orders are placed.

### Success Criteria
*   Buying 2 different products from the same seller results in **two separate orders**.
*   Total shipping fee for the above is **₱80**.
*   Buying 1 product from "Seller A" and 1 product from "Seller B" results in **two separate orders**.
*   Total shipping fee for the above is **₱160**.
*   Multiple quantities of the **same product** result in a **single order** for that product.

### Testing Plan
1.  Add "Product A" (Seller 1) and "Product B" (Seller 1) to cart.
2.  Add "Product C" (Seller 2) to cart.
3.  Verify Checkout shows `Sum(A, B, C) + 160`.
4.  Place order and verify **three separate orders** appear in "My Orders".
5.  Check that "Order A" or "Order B" has ₱80 shipping, the other has ₱0, and "Order C" has ₱80.
