# Robustness Improvements for Blazor Project

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor services and UI components to handle nulls and errors gracefully, preventing unhandled "unpleasant" errors in the Blazor WASM app.

**Architecture:** Use defensive programming patterns, including comprehensive try-catch blocks and explicit null checks, especially around third-party service calls (Firebase, LocalStorage).

**Tech Stack:** .NET 9, Blazor WASM, Firebase, Blazored.LocalStorage.

---

### Task 1: Refactor AuthService.cs

**Files:**
- Modify: `C:\AntigravityProjects\Temu_Catarig\Temu_Catarig.Blazor\Services\AuthService.cs`

- [ ] **Step 1: Make EnsureClientInitialized more robust**
- [ ] **Step 2: Update UserEmail and UserDisplayName properties for maximum safety**
- [ ] **Step 3: Add try-catch blocks to all async methods and EnsureClientInitialized calls**
- [ ] **Step 4: Ensure GetFreshTokenAsync never throws**

### Task 2: Refactor FirebaseService.cs

**Files:**
- Modify: `C:\AntigravityProjects\Temu_Catarig\Temu_Catarig.Blazor\Services\FirebaseService.cs`

- [ ] **Step 1: Add null checks for item.Object in all methods using Select on OnceAsync results**
- [ ] **Step 2: Add try-catch blocks to all public methods and log errors to Console.WriteLine**

### Task 3: Refactor App.razor and NavMenu.razor

**Files:**
- Modify: `C:\AntigravityProjects\Temu_Catarig\Temu_Catarig.Blazor\App.razor`
- Modify: `C:\AntigravityProjects\Temu_Catarig\Temu_Catarig.Blazor\Layout\NavMenu.razor`

- [ ] **Step 1: Ensure App.razor handles initialization failures without breaking the UI**
- [ ] **Step 2: Verify NavMenu.razor properties are safe against null AuthService properties**
