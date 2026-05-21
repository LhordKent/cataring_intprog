# Design Spec: Temu_Catarig Blazor WASM (Full Port)

**Date:** 2026-05-21  
**Status:** Draft  
**Topic:** Creating a modern Blazor WebAssembly full-feature port of the Temu_Catarig MAUI app.

## 1. Goal
To deliver a high-fidelity, functional web version of the Temu_Catarig application using Blazor WebAssembly. This version will reuse existing models and core logic while providing a modern, responsive UI tailored for the web using Tailwind CSS.

## 2. Architecture
- **Project Name:** `Temu_Catarig.Blazor`
- **Type:** Blazor WebAssembly Standalone
- **Location:** Sub-project folder within the root directory.
- **Styling:** Tailwind CSS (via CLI build)
- **Icons:** Lucide Icons (Lucide.Blazor)
- **Persistence:** `Blazored.LocalStorage` for auth state and user preferences.

## 3. Implementation Strategy

### 3.1 Data & Logic
- **Models:** Direct port of all classes from `Models/`.
- **AuthService:** 
    - Refactored from static to instance-based for DI.
    - Integrated with `ILocalStorageService` to persist `UserToken` and `UserId`.
    - Handle session restoration on app initialization.
- **FirebaseService:**
    - Registered as Scoped.
    - Consumes `AuthService` for token generation.
- **ImageService:**
    - Adapted for Blazor `InputFile` support (Stream-based uploads to Cloudinary).

### 3.2 UI Design (Modern Tailwind)
- **MainLayout:**
    - Adaptive navigation (Side-nav or Top-nav for desktop, Bottom-tab-bar for mobile).
    - Shopping cart count badge in navbar.
- **Key Views:**
    - **Home:** Hero section with featured categories and product grid.
    - **Product Detail:** Large image gallery, interactive rating summary, and "Related Products".
    - **Checkout:** Streamlined multi-step or single-page checkout flow.
    - **Admin Dashboard:** Sidebar navigation for managing listings and viewing orders.
- **Responsiveness:** Mobile-first approach, scaling gracefully to ultra-wide monitors.

## 4. Technical Dependencies
- `FirebaseAuthentication.net` (4.1.0)
- `FirebaseDatabase.net` (5.0.0)
- `Blazored.LocalStorage`
- `Lucide.Blazor`
- `CloudinaryDotNet` (1.29.0)

## 5. Security
- Reuse existing Firebase API keys.
- Add `localhost` and deployment domains to Firebase Authentication authorized domains.
- Client-side route protection using standard Blazor `[Authorize]` or custom auth state provider.

## 6. Success Criteria
- [ ] User can log in/register and stay logged in after refresh.
- [ ] Complete shopping flow: Browse -> Add to Cart -> Checkout.
- [ ] Admin can approve/delete products and manage fulfillment.
- [ ] Real-time messaging works between buyer and seller.
- [ ] Visual aesthetic matches modern "Temu" branding with Tailwind's polish.
