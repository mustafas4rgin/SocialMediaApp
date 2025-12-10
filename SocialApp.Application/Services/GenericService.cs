using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class GenericService<T> : IGenericService<T> where T : EntityBase
{
    private readonly IValidator<T> _validator;
    private readonly ILogger<GenericService<T>> _logger;
    private readonly IGenericRepository<T> _repository;
    public GenericService(IValidator<T> validator, IGenericRepository<T> repository, ILogger<GenericService<T>> logger)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
    }
    public async Task<IServiceResultWithData<IEnumerable<T>>> GetAllActiveAsync(CancellationToken ct = default)
    {
        try
        {
            var activeEntities = await _repository.GetAllActive(ct)
                                    .ToListAsync(ct);

            if (!activeEntities.Any())
                return new ErrorResultWithData<IEnumerable<T>>("There is no active data.");

            return new SuccessResultWithData<IEnumerable<T>>("Active data found.", activeEntities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResultWithData<IEnumerable<T>>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<IEnumerable<T>>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var entities = await _repository.GetAll(ct)
                               .ToListAsync(ct);

            if (!entities.Any())
                return new ErrorResultWithData<IEnumerable<T>>("There is no data.");

            return new SuccessResultWithData<IEnumerable<T>>("Data found.", entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResultWithData<IEnumerable<T>>(ex.Message);
        }
    }
    public virtual async Task<IServiceResult> AddAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(entity, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));

            await _repository.AddAsync(entity, ct);
            await _repository.SaveChangesAsync(ct);

            return new SuccessResult("Entity added successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResult(ex.Message);
        }
    }

    public virtual async Task<IServiceResult> UpdateAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(entity, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));

            await _repository.UpdateAsync(entity, ct);
            await _repository.SaveChangesAsync(ct);

            return new SuccessResult("Entity updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResult(ex.Message);
        }
    }
    public async Task<IServiceResult> SoftDeleteAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(entity, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));

            _repository.SoftDelete(entity, ct);
            await _repository.SaveChangesAsync(ct);

            return new SuccessResult("Entity soft deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResult(ex.Message);
        }
    }
    public async Task<IServiceResult> DeleteAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(entity, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));

            _repository.Delete(entity, ct);
            await _repository.SaveChangesAsync(ct);

            return new SuccessResult("Entity deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResult(ex.Message);
        }
    }

    public async Task<IServiceResult> RestoreAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(entity, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));

            if (!entity.IsDeleted)
                return new ErrorResult("Entity is not currently deleted.");

            _repository.Restore(entity, ct);
            await _repository.SaveChangesAsync(ct);

            return new SuccessResult("Entity restored successfully.");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResult(ex.Message);
        }
    }

    public async Task<IServiceResultWithData<T>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id, ct);

            if (entity is null)
                return new ErrorResultWithData<T>($"There is no entity with ID : {id}");

            return new SuccessResultWithData<T>("Entity found.", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResultWithData<T>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<T>> GetActiveByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _repository.GetActiveByIdAsync(id, ct);

            if (entity is null)
                return new ErrorResultWithData<T>($"There is no active entity with ID : {id}");

            return new SuccessResultWithData<T>("Active entity found.", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResultWithData<T>(ex.Message);
        }
    }
}