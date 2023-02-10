using Microsoft.EntityFrameworkCore;
using Sales.Shared.Applications.Interfaces;
using Sales.Shared.DataBase;
using Sales.Shared.Responses;

namespace Sales.Shared.Applications.Logic
{
    public class GenericRepositoryFactory<TEntity> : IGenericRepositoryFactory<TEntity> where TEntity : class
    {
        private readonly SalesDbContext _dbContext;

        public GenericRepositoryFactory(SalesDbContext salesDbContext)
        {
            _dbContext = salesDbContext;
        }
        public async Task<GenericResponse<TEntity>> AddDataAsync(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Add(entity);
                await _dbContext.SaveChangesAsync();
                return new GenericResponse<TEntity>
                {
                    IsSuccess = true,
                    Result = entity,
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return new GenericResponse<TEntity>
                    {
                        IsSuccess = false,
                        Message = "Ya existe en el sistema verifique los datos!",
                    };
                }
                else
                {
                    return new GenericResponse<TEntity>
                    {
                        IsSuccess = false,
                        Message = dbUpdateException.InnerException.Message,
                    };
                }
            }
            catch (Exception exc)
            {
                return new GenericResponse<TEntity>
                {
                    IsSuccess = false,
                    Message = exc.Message,
                };
            }
        }

        public async Task<GenericResponse<TEntity>> DeleteDataAsync(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
                return new GenericResponse<TEntity>
                {
                    IsSuccess = true,
                    Result = entity,
                };
            }
            catch (Exception exc)
            {
                return new GenericResponse<TEntity>
                {
                    IsSuccess = false,
                    Message = exc.Message,
                };
            }
        }

        public async Task<IReadOnlyList<TEntity>> GetAllDataAsync()
        {
            IReadOnlyList<TEntity> queryEntity = await _dbContext.Set<TEntity>().ToListAsync();
            return queryEntity;
        }


        public async Task<GenericResponse<TEntity>> UpdateDataAsync(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Update(entity);
                await _dbContext.SaveChangesAsync();
                return new GenericResponse<TEntity>
                {
                    IsSuccess = true,
                    Result = entity,
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    return new GenericResponse<TEntity>
                    {
                        IsSuccess = false,
                        Message = "Ya existe en el sistema verifique los datos!",
                    };
                }
                else
                {
                    return new GenericResponse<TEntity>
                    {
                        IsSuccess = false,
                        Message = dbUpdateException.InnerException.Message,
                    };
                }
            }
            catch (Exception exc)
            {
                return new GenericResponse<TEntity>
                {
                    IsSuccess = false,
                    Message = exc.Message,
                };
            }
        }
    }
}
