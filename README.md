
# GenericRepository

A clean, extensible, and dependency-free Generic Repository implementation based on SOLID principles and Clean Architecture.  
Supports Specification Pattern, Soft Delete, Paging, and Bulk Operations with EF Core 8+.

---

## 📁 Projects

| Project | Description |
|--------|-------------|
| `GenericRepository.Abstractions` | Core interfaces for read/write/bulk repository operations. No external dependencies. |
| `GenericRepository.EFCore` | EF Core implementation including specification support, paging, soft delete, and bulk operations.

---

## ✅ Features

- Clean architecture & SOLID-compliant
- Fully async repository methods
- Soft delete support (IsDeleted)
- Pagination with total count via `PagedResult<T>`
- Specification pattern support
- Efficient bulk inserts using batching
- Dependency Injection ready via `AddGenericRepository()`

---

## 📦 Installation

```bash
dotnet add package GenericRepository.Abstractions
dotnet add package GenericRepository.EFCore
```

---

## 🧪 Usage

```csharp
// Register repository in DI
services.AddGenericRepository();

// Example usage in service
public class ProductService
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _repository.GetListAsync();
    }
}
```

---

## 🔧 Bulk Configuration (Optional)

You can override default EFCore.BulkExtensions settings:

```csharp
services.AddGenericRepository();
```

---

## 📜 License

MIT License
