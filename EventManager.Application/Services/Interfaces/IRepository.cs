using EventManager.Domain.Models;

namespace EventManager.Application.Services.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    TEntity? Get(Guid id);
    List<TEntity> GetList();
    void Create(TEntity entity);
    bool Update(TEntity entity);
    bool Delete(Guid id);
}
