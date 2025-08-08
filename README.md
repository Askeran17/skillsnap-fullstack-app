# SkillSnap 🧠💼

SkillSnap is a portfolio-driven web application that allows users to showcase their skills, projects, and professional growth. Built with ASP.NET Core and Blazor, it provides a clean interface for developers to present their work and for recruiters to explore talent.

---

## 🚀 Features

- 🧑‍💻 Portfolio management (skills, projects, bio)
- 🔐 Authentication & role-based authorization
- ⚡ In-memory caching for fast API responses
- 📦 RESTful API built with ASP.NET Core
- 🧪 Unit & integration tests with xUnit and EF Core InMemory

---

## 🔐 Access Control

SkillSnap uses role-based access to protect sensitive operations:

| Role       | Permissions                                                                 |
|------------|------------------------------------------------------------------------------|
| **Admin**  | ✅ Create, update, and delete projects and skills                            |
| **User**   | 👀 View all public projects and skills, but cannot modify or delete anything |

Authorization is handled via ASP.NET Identity and `[Authorize(Roles = "Admin")]` attributes on protected endpoints.

---

## 🔗 API & Client Integration

The Blazor WebAssembly client communicates with the ASP.NET Core API server via HTTP.

To ensure proper integration:

1. **Start the API server with the correct launch profile:**

```bash
cd SkillSnap.Api
dotnet run --launch-profile http
```

This will start the API on the expected port (e.g. `http://localhost:5000`) and expose endpoints for the client.

2. **Start the Blazor client:**

```bash
cd SkillSnap.Client
dotnet run
```

The client will automatically connect to the API server if configured correctly in `appsettings.json` or `Program.cs`.

---

## 🛠 Tech Stack

| Layer            | Technology                     |
|------------------|--------------------------------|
| Backend API      | ASP.NET Core Web API (.NET 8)  |
| Frontend Client  | Blazor WebAssembly             |
| Database         | Entity Framework Core          |
| Testing          | xUnit, Moq, EF Core InMemory   |
| Auth             | ASP.NET Identity + JWT         |
| Caching          | IMemoryCache                   |

---

## 📁 Project Structure

```
SkillSnap/
├── SkillSnap.Api/           # ASP.NET Core Web API
├── SkillSnap.Client/        # Blazor WebAssembly frontend
├── SkillSnap.Tests.Api/     # Unit tests for API
├── SkillSnap.Tests.Client/  # Unit tests for Blazor components
└── SkillSnap.sln            # Solution file
```

---

## 🧪 Running Tests

To run all tests:

```bash
dotnet test
```

Tests include:

- ✅ Controller logic
- ✅ Authorization via Claims
- ✅ EF Core InMemory database isolation
- ✅ Caching behavior

---

## 🧰 Setup Instructions

1. **Clone the repo**

```bash
git clone https://github.com/your-username/SkillSnap.git
cd SkillSnap
```

2. **Restore dependencies**

```bash
dotnet restore
```

3. **Run the API**

```bash
cd SkillSnap.Api
dotnet run --launch-profile http
```

4. **Run the Blazor client**

```bash
cd SkillSnap.Client
dotnet run
```

---

## 👨‍🔬 Development Notes

- Controllers use `ProjectDto` and `SkillDto` for clean separation of concerns.
- Authorization is handled via `ClaimTypes.NameIdentifier` and role-based `[Authorize(Roles = "Admin")]`.
- Tests use isolated InMemory databases to prevent cross-test contamination.

---

## 📄 License

This project is licensed under the MIT License. Feel free to use, modify, and contribute.
