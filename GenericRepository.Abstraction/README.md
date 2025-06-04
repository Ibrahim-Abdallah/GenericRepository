
# GenericRepository.Abstractions

This package contains clean, reusable abstractions for implementing the Generic Repository Pattern following SOLID principles.

## ✅ Features

- Generic repository interfaces for separation of concerns
- Support for Specification Pattern
- Paging support via `PagedResult<T>`
- Soft delete support with `ISoftDeletable`
- Extensible with bulk operations via `IBulkRepository`
- No external dependencies

---

## 📦 Installation

```bash
dotnet add package GenericRepository.Abstractions
```

---

## 🔍 Interfaces Overview

| Interface | Responsibility |
|----------|----------------|
| `IReadRepository<T>` | Read operations with filtering, specification, and pagination |
| `IWriteRepository<T>` | Add, update, delete, soft-delete |
| `IBulkRepository<T>` | Efficient batch insert/update/delete |
| `ISpecification<T>` | Encapsulates complex filtering logic |
| `IBulkConfigProvider` | Allows custom bulk behavior (e.g., EFCore.BulkExtensions) |
| `PagedResult<T>` | Encapsulates paginated response |

---

## 🕵️ Auditing Interfaces

You can apply these interfaces to your entities to automatically track audit metadata:

| Interface | Properties |
|----------|------------|
| `ICreatableAuditable` | `CreatedAt`, `CreatedBy` |
| `IUpdatableAuditable` | `UpdatedAt`, `UpdatedBy` |
| `IDeletableAuditable` | `DeletedAt`, `DeletedBy` |

These interfaces are optional and help ensure consistent auditing logic across your domain models.

---

## 🔧 Example

```csharp
public interface IProductService
{
    Task<PagedResult<Product>> GetPagedProductsAsync(ISpecification<Product> specification, int page = 1, int pageSize = 20);
}
```

---

## 🧱 Designed For

- Clean Architecture projects
- Domain-driven design
- Framework-agnostic usage
