# Design Specification: Temu_Catarig Blazor Port (1:1 Logic & Web-Optimized UI)

**Date:** 2026-05-21
**Topic:** High-fidelity port of MAUI mobile app to Blazor WASM.

## 1. Vision & Architecture
The Blazor version of Temu_Catarig will be a behaviorally identical (1:1) port of the MAUI mobile application, ensuring consistency in data, authentication, and business rules. The UI will be adapted from the mobile screenshots to a professional "Account Hub" web layout.

### Tech Stack
- **Frontend:** Blazor WebAssembly (.NET 9)
- **Styling:** Tailwind CSS (CDN-based for this prototype)
- **Persistence:** Blazored.LocalStorage
- **Backend:** Firebase (Auth, Realtime Database)
- **Storage:** Cloudinary (via `CloudinaryDotNet`)

## 2. Core Service Port (1:1 Logic)

### AuthService
- **State Management:** Mirrored from MAUI (UserId, UserToken, IsAdmin).
- **Persistence:** Uses `ILocalStorage` to persist auth state across reloads (equivalent to MAUI's `SecureStorage` or static state persistence).
- **Hardcoded Admin:** Maintained 1:1 (`johnadmin@gmail.com` / `123123`).
- **Token Refresh:** Logic ported from MAUI's `GetFreshTokenAsync`.

### FirebaseService
- **Methods:** Port all methods from MAUI `FirebaseService.cs` exactly, including:
  - `GetProductsAsync(status)`
  - `PlaceOrderAsync` (including stock deduction logic)
  - `GetUserProfileAsync` / `SaveUserProfileAsync`
  - `GetConversationsAsync` / `SendMessageAsync`
  - `SubmitReviewAsync` (including aggregate rating calculation)
- **Consistency:** Ensure the Realtime Database structure is identical to MAUI's.

### ImageService
- **Implementation:** Port the `Cloudinary` configuration from MAUI.
- **WASM Support:** Adapted to use `IBrowserFile` streams for uploads instead of MAUI's `FilePicker` result streams.

## 3. UI/UX Design: "Account Hub"

### Global Theme
- **Primary Color:** `#ff6000` (Temu Orange)
- **Background:** `bg-slate-50`
- **Container:** All main content centered in a `max-w-[1200px]` container.

### Layout: Centered Account Hub
- **Header:**
  - Sticky, white background with border.
  - Bold italic "TEMU" logo.
  - Wide rounded search bar in the center.
  - Right-aligned icons with labels: Orders, Account (Dropdown), Cart (Badge).
- **Profile Page (The Hub):**
  - **Banner:** Wide orange gradient banner with user avatar and name.
  - **Navigation Grid:** A 2-column (mobile) or 3-column (desktop) grid of clickable cards (tiles):
    - **My Orders:** View order history and status.
    - **Edit Profile:** Manage personal info and shipping address.
    - **Seller Center:** Grouped section for sellers (Add Product, Manage Products, Fulfillment).
- **Product Management:**
  - **Form:** A clean 2-column layout for Add/Edit product (Image upload on left, fields on right).
  - **List:** A responsive table view for "Manage Products" with status badges (Pending/Approved).

## 4. Navigation Mapping (1:1)
- `//LandingPage` -> `/`
- `LoginPage` -> `/login`
- `RegisterPage` -> `/register`
- `ProfilePage` -> `/profile`
- `CartPage` -> `/cart`
- `AddEditProductPage` -> `/admin/product/add` or `/admin/product/edit/{id}`
- `OrderFulfillmentPage` -> `/admin/fulfillment`
- `MyOrdersPage` -> `/orders`

## 5. Success Criteria
- [ ] Authentication state persists across browser refreshes.
- [ ] Product images are uploaded successfully to Cloudinary.
- [ ] Stock is correctly deducted when an order is placed.
- [ ] The UI feels like a professional desktop version of the mobile app screenshots.
