# Temu_Catarig Blazor Full Port Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Create a high-fidelity Blazor WASM version of the Temu_Catarig app with a modern Tailwind UI.

**Architecture:** A standalone Blazor WebAssembly project that ports logic from the MAUI project. Services are refactored for Dependency Injection and local storage persistence.

**Tech Stack:** .NET 9, Blazor WASM, Tailwind CSS, Firebase, Blazored.LocalStorage, Lucide.Blazor.

---

### Task 1: Project Scaffolding & Tailwind Setup

**Files:**
- Create: `Temu_Catarig.Blazor/Temu_Catarig.Blazor.csproj`
- Create: `Temu_Catarig.Blazor/wwwroot/index.html`
- Create: `Temu_Catarig.Blazor/tailwind.config.js`
- Create: `Temu_Catarig.Blazor/Styles/app.css`

- [ ] **Step 1: Create the Blazor WASM project**
Run: `dotnet new blazorwasm -o Temu_Catarig.Blazor --interactivity WebAssembly` (or standard standalone template)
Expected: Project directory created.

- [ ] **Step 2: Add Dependencies**
Add `Blazored.LocalStorage`, `FirebaseAuthentication.net`, `FirebaseDatabase.net`, `Lucide.Blazor`.

- [ ] **Step 3: Setup Tailwind CSS**
Initialize Tailwind in the project folder and configure `content` to include Razor and HTML files.
```javascript
module.exports = {
  content: ["./**/*.{razor,html,cshtml}"],
  theme: { extend: {} },
  plugins: [],
}
```

- [ ] **Step 4: Verify build**
Run: `dotnet build Temu_Catarig.Blazor`
Expected: Success.

- [ ] **Step 5: Commit**
`git add Temu_Catarig.Blazor && git commit -m "chore: scaffold blazor project and tailwind"`

### Task 2: Port Models & Refactor AuthService

**Files:**
- Create: `Temu_Catarig.Blazor/Models/` (copy from root)
- Create: `Temu_Catarig.Blazor/Services/AuthService.cs`
- Modify: `Temu_Catarig.Blazor/Program.cs`

- [ ] **Step 1: Copy Models**
Copy all `.cs` files from root `Models/` to `Temu_Catarig.Blazor/Models/`.

- [ ] **Step 2: Implement AuthService with LocalStorage**
Refactor the static `AuthService` to use `ILocalStorageService`.
```csharp
public class AuthService {
    private readonly ILocalStorageService _localStorage;
    // ... constructor and logic using _localStorage.GetItemAsync<string>("token")
}
```

- [ ] **Step 3: Register Services in Program.cs**
```csharp
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FirebaseService>();
```

- [ ] **Step 4: Commit**
`git commit -m "feat: port models and refactor auth service"`

### Task 3: Implement MainLayout & Navigation

**Files:**
- Modify: `Temu_Catarig.Blazor/Shared/MainLayout.razor`
- Modify: `Temu_Catarig.Blazor/Shared/NavMenu.razor`

- [ ] **Step 1: Design modern Navbar with Tailwind**
Include Search bar, User profile icon, and Cart icon with badge.

- [ ] **Step 2: Implement Mobile Bottom Bar**
Visible only on small screens using Tailwind `md:hidden`.

- [ ] **Step 3: Commit**
`git commit -m "ui: implement main layout and navigation"`

### Task 4: Landing Page & Product Grid

**Files:**
- Create: `Temu_Catarig.Blazor/Pages/Home.razor`
- Create: `Temu_Catarig.Blazor/Components/ProductCard.razor`

- [ ] **Step 1: Create ProductCard component**
Image, Title, Price, and "Add to Cart" button with Tailwind styling.

- [ ] **Step 2: Implement Home page grid**
Fetch products from `FirebaseService` and display using `ProductCard`.

- [ ] **Step 3: Commit**
`git commit -m "feat: landing page with product grid"`

### Task 5: Auth Pages (Login/Register)

**Files:**
- Create: `Temu_Catarig.Blazor/Pages/Login.razor`
- Create: `Temu_Catarig.Blazor/Pages/Register.razor`

- [ ] **Step 1: Build styled Auth forms**
Use Tailwind for centered, clean cards with validation feedback.

- [ ] **Step 2: Connect to AuthService**
Call `LoginUser` and redirect to home on success.

- [ ] **Step 3: Commit**
`git commit -m "feat: login and registration pages"`

### Task 6: Cart & Checkout Flow

**Files:**
- Create: `Temu_Catarig.Blazor/Pages/Cart.razor`
- Create: `Temu_Catarig.Blazor/Pages/Checkout.razor`

- [ ] **Step 1: Implement Cart management**
List items, update quantities, remove items.

- [ ] **Step 2: Implement Checkout**
Collect address (or placeholder) and place order via `FirebaseService`.

- [ ] **Step 3: Commit**
`git commit -m "feat: cart and checkout flow"`

### Task 7: Admin & Messaging (Final Polish)

**Files:**
- Create: `Temu_Catarig.Blazor/Pages/Admin/Dashboard.razor`
- Create: `Temu_Catarig.Blazor/Pages/Chat.razor`

- [ ] **Step 1: Build Admin Dashboard**
Table view of products with status toggles (Pending/Approved).

- [ ] **Step 2: Implement Messaging**
Real-time chat interface using `FirebaseService`.

- [ ] **Step 3: Commit**
`git commit -m "feat: admin dashboard and messaging"`
