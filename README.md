# InventoryHub

A .NET/Blazor full-stack inventory management application.

## Overview
InventoryHub is a modern, high-performance inventory management system built with:
- **Backend:** ASP.NET Core 10 Web API
- **Frontend:** Blazor WebAssembly (Standalone)
- **Database:** In-Memory (Thread-safe repository)
- **Language:** C# 13 / .NET 10

## Features
- **Full CRUD Operations:** Create, Read, Update, Delete inventory items.
- **Performance Optimized:** Pagination, response compression, and async architecture.
- **Modern UI:** Minimal, custom CSS design with responsive layout.
- **Structured API:** Standardized JSON responses with validation.

## Prerequisites
- .NET 10.0 SDK or later

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd InvnetoryHub
   ```

2. **Run the Backend API:**
   ```bash
   cd InventoryHub.Api
   dotnet run
   ```
   The API will start at `http://localhost:5000` (HTTP) or `https://localhost:5001` (HTTPS).
   Swagger UI is available at `https://localhost:5001/swagger`.

3. **Run the Frontend Client:**
   Open a new terminal:
   ```bash
   cd InventoryHub.Client
   dotnet run
   ```
   The application will be available at `http://localhost:5173`.

## Architecture

- `InventoryHub.Api`: RESTful API with controllers, services, and repository.
- `InventoryHub.Client`: Blazor WebAssembly frontend.
- `InventoryHub.Shared`: Shared DTOs and models used by both client and server.

## AI Assistance
This project was developed with the assistance of AI. See [AI_REFLECTION.md](AI_REFLECTION.md) for details.
