
# GenericRepository.EFCore

This package provides an Entity Framework Core implementation of the interfaces defined in `GenericRepository.Abstractions`.

## ✅ Features

- Full support for `IRepository<T>` operations
- Implements Specification Pattern for querying
- Paging support with `PagedResult<T>`
- Soft delete by marking `IsDeleted = true`
- Bulk operations using EFCore.BulkExtensions
- Dependency Injection ready via `AddGenericRepository()`

---

## 📦 Installation

```bash
dotnet add package GenericRepository.EFCore
```

---

## 🔗 Dependencies

- `GenericRepository.Abstractions`
- `Microsoft.EntityFrameworkCore`
- `EFCore.BulkExtensions`

---

## 🧪 Usage

```csharp
services.AddGenericRepository();

public class ProductService
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Product>> GetPagedAsync()
    {
        var spec = new ProductByCategorySpecification("Books");
        return await _repository.GetPagedAsync(spec, page: 1, pageSize: 10);
    }
}
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

---

## 🔧 Custom Bulk Config

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

Then register:

```csharp
services.AddGenericRepository();
```

Then usage in your service:

```csharp
public class ProductService
{
    private readonly IRepository<Product> _repository;
    private readonly IBulkConfigProvider _bulkConfigProvider // Optional, if you want using bulk operations. this is default config, also you can use custom config
    public ProductService(IRepository<Product> repository, IBulkConfigProvider bulkConfigProvider)
    {
        _repository = repository;
        _bulkConfigProvider = bulkConfigProvider;
    }
    public async Task BulkInsertAsync(List<Product> products)
    {
        await _repository.BulkInsertAsync(products); // or _repository.BulkInsertAsync(products, userId); if using auditing
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

public class ProductByCategorySpecification() : Specification<ProductByCategory>
{
    public ProductByCategorySpecification(string category)
    {
        AddCriteria(p => p.Category == category);
    }
}
```

---

## 🛠️ Requirements

- .NET 9.0 or later
- EF Core 9
