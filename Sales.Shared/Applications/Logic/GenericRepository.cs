using Microsoft.EntityFrameworkCore;
using Sales.Shared.Applications.Interfaces;
using Sales.Shared.DataBase;
using Sales.Shared.Entities;
using Sales.Shared.Responses;

namespace Sales.Shared.Applications.Logic
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : ClaseBase
    {
        private readonly SalesDbContext _applicationDbContext;

        public GenericRepository(SalesDbContext salesDbContext)
        {
            _applicationDbContext = salesDbContext;
        }
        public async Task<GenericResponse<TEntity>> AddAsync(TEntity entity)
        {
            try
            {
                _applicationDbContext.Set<TEntity>().Add(entity);
                if (!await SaveAllAsync())
                {
                    return new GenericResponse<TEntity>
                    {
                        IsSuccess = false,
                        ErrorMessage = "problems with registration"
                    };
                }
                return new GenericResponse<TEntity>
                {
                    IsSuccess = true,
                };
            }
            catch (Exception exception)
            {
                return new GenericResponse<TEntity>
                {
                    IsSuccess = false,
                    ErrorMessage = exception.Message,
                };
            }
        }

        public void AddEntity(TEntity Entity)
        {
            _applicationDbContext.Set<TEntity>().Add(Entity);
            _applicationDbContext.SaveChanges();
        }

        public void DeleteEntity(TEntity Entity)
        {
            _applicationDbContext.Set<TEntity>().Remove(Entity);
            _applicationDbContext.SaveChanges();
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            try
            {
                return await _applicationDbContext.Set<TEntity>().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<GenericResponse<TEntity>> GetByIdAsync(int id)
        {
            try
            {
                var ResultObject = await _applicationDbContext.Set<TEntity>().FindAsync(id);

                return new GenericResponse<TEntity>
                {
                    IsSuccess = true,
                    Result = ResultObject,
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return new GenericResponse<TEntity>
                    {
                        IsSuccess = false,
                        ErrorMessage = dbUpdateException.InnerException.Message,
                    };
                }
                else
                {
                    return new GenericResponse<TEntity>
                    {
                        IsSuccess = false,
                        ErrorMessage = dbUpdateException.InnerException.Message,
                    };
                }
            }
            catch (Exception exception)
            {
                return new GenericResponse<TEntity>
                {
                    IsSuccess = false,
                    ErrorMessage = exception.InnerException.Message,
                };
            }
        }

        public async Task<GenericResponse<TEntity>> UpdateAsync(TEntity entity)
        {
            try
            {
                _applicationDbContext.Set<TEntity>().Update(entity);
                if (!await SaveAllAsync())
                {
                    return new GenericResponse<TEntity>
                    {
                        IsSuccess = false,
                        ErrorMessage = "problems with registration"
                    };
                }
                return new GenericResponse<TEntity>
                {
                    IsSuccess = true,
                };
            }
            catch (Exception exception)
            {

                return new GenericResponse<TEntity>
                {
                    IsSuccess = false,
                    ErrorMessage = exception.Message,
                }; ;
            }
        }

        public void UpdateEntity(TEntity Entity)
        {
            try
            {
                _applicationDbContext.Set<TEntity>().Update(Entity);
                _applicationDbContext.SaveChanges();
            }
            catch (Exception)
            {
            }
        }

        private async Task<bool> SaveAllAsync()
        {
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }
    }
}
