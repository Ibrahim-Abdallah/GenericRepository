
# GenericRepository

A clean, extensible, and dependency-free Generic Repository implementation based on SOLID principles and Clean Architecture.  
Supports Specification Pattern, Soft Delete, Paging, and Bulk Operations with EF Core 9+.

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
- Full auditing support with `ICreatableAuditable`, `IUpdatableAuditable`, and `IDeletableAuditable`
- Pagination with total count via `PagedResult<T>`
- Specification pattern support
- Bulk operations using EFCore.BulkExtensions
- Efficient bulk inserts using batching
- Dependency Injection ready via `AddGenericRepository()`
- Transactional support

---

## 🛠️ Requirements

- .NET 9.0 or later
- EF Core 9

---

## 📦 Installation

```bash
dotnet add package GenericRepository.Abstractions
dotnet add package GenericRepository.EFCore9
```

---

## 🕵️ Auditing Support

The EFCore implementation automatically handles audit fields if your entities implement:

| Interface | Automatically set on |
|----------|-----------------------|
| `ICreatableAuditable` | `CreatedAt`, `CreatedBy` during Add |
| `IUpdatableAuditable` | `UpdatedAt`, `UpdatedBy` during Update |
| `IDeletableAuditable` | `DeletedAt`, `DeletedBy` during Delete |

Make sure to pass the current user context to support `CreatedBy`, `UpdatedBy`, etc.
> **Note:** Bulk operations bypass SaveChangesAsync and do not trigger EF interceptors or audit tracking. You must apply auditing manually using interfaces like ICreatableAuditable.

---

## 🧪 Usage

```csharp
// Register repository in DI
services.AddGenericRepository();

// Example usage in service
public class ProductService
{
    private readonly IRepository<Product> _repository;
    private readonly IBulkConfigProvider _bulkConfigProvider; // Optional, if you want using bulk operations. this is default config, also you can use custom config
    public ProductService(IRepository<Product> repository, IBulkConfigProvider bulkConfigProvider)
    {
        _repository = repository;
        _bulkConfigProvider = bulkConfigProvider;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _repository.GetListAsync();
    }

    public async Task<PagedResult<Product>> GetPagedAsync()
    {
        var spec = new ProductByCategorySpecification("Books");
        return await _repository.GetPagedAsync(spec, page: 1, pageSize: 10);
    }

    public async Task BulkInsertAsync(List<Product> products)
    {
        await _repository.BulkInsertAsync(products);
        await _repository.BulkInsertAsync(products, userId); // Required if auditing is enabled
    }

    public async Task<ProductResponse> AddAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };
        await _repository.AddAsync(product, cancellationToken);
        return new ProductResponse { Id = product.Id, Name = product.Name };
    }
}

public abstract class AuditableEntity : ICreatableAuditable<string>, IUpdatableAuditable<string?>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class ApplicationDbContext : DbContext
{
    // override SaveChangerAsync to configure aduiting fields. Note that: this didn't work with bulk operations
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entityEntry in entries)
        {
            // Get the user ID dynamically at save time
            var userId = _contextAccessor.GetUserId();
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.CreatedBy = userId!;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Entity.UpdatedAt = DateTime.UtcNow;
                entityEntry.Entity.UpdatedBy = userId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

public class ProductByCategorySpecification() : Specification<Product>
{
    public ProductByCategorySpecification(string category)
    {
        AddCriteria(p => p.Category == category);
    }
}
```

---

## 🔧 Bulk Configuration (Optional)

You can override default EFCore.BulkExtensions settings:

```csharp
public class CustomBulkConfigProvider : IBulkConfigProvider
{
    public BulkConfig GetBulkConfig()
    {
        return new BulkConfig
        {
            BatchSize = 500, // Default batch size for bulk operations 1000
            PreserveInsertOrder = true
        };
    }
}
```

---

## 📜 License

MIT License
