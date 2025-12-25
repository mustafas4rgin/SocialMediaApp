using System.Data;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
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
            var activeEntities = await _repository.GetAllAsync(false, ct);

            if (!activeEntities.Any())
                return new ErrorResultWithData<IEnumerable<T>>("There is no active data.", 404);

            return new SuccessResultWithData<IEnumerable<T>>("Active data found.", activeEntities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResultWithData<IEnumerable<T>>("An error occured while getting entities.");
        }
    }
    public async Task<IServiceResultWithData<IEnumerable<T>>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var entities = await _repository.GetAllAsync(true, ct);

            if (!entities.Any())
                return new ErrorResultWithData<IEnumerable<T>>("There is no data.", 404);

            return new SuccessResultWithData<IEnumerable<T>>("Data found.", entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResultWithData<IEnumerable<T>>("An error occured while getting entities.");
        }
    }
    public virtual async Task<IServiceResultWithData<T>> AddAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(entity, ct);

            if (!validationResult.IsValid)
                return new ErrorResultWithData<T>(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));

            var created = await _repository.AddAsync(entity, ct);
            if (created is null)
                return new ErrorResultWithData<T>("Entity not found or create failed.", 404);

            await _repository.SaveChangesAsync(ct);

            return new SuccessResultWithData<T>($"Entity added successfully.", entity, 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResultWithData<T>("An error occured while adding entity.");
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

            var updated = await _repository.UpdateAsync(entity, ct);
            if (updated is null)
                return new ErrorResult("Entity not found or update failed.", 404);

            await _repository.SaveChangesAsync(ct);

            return new SuccessResult("Entity updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResult("An error occured while updating entity.");
        }
    }
    public async Task<IServiceResult> DeleteByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id, includeDeleted: false, ct: ct);

            if (entity is null)
                return new ErrorResult($"There is no entity with ID : {id}.", 404);

            var validationResult = await _validator.ValidateAsync(entity, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));

            _repository.Delete(entity);
            await _repository.SaveChangesAsync(ct);

            return new SuccessResult("Entity deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResult("An error occured while deleting entity.");
        }
    }

    public async Task<IServiceResult> RestoreAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id, includeDeleted: true, ct: ct);

            if (entity is null)
                return new ErrorResult($"There is no entity with ID : {id}.", 404);

            var validationResult = await _validator.ValidateAsync(entity, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));

            if (!entity.IsDeleted)
                return new ErrorResult("Entity is not currently deleted.");

            _repository.Restore(entity);
            await _repository.SaveChangesAsync(ct);

            return new SuccessResult("Entity restored successfully.");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResult("An error occured while restoring entity.");
        }
    }

    public async Task<IServiceResultWithData<T>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id, includeDeleted: true, ct: ct);

            if (entity is null)
                return new ErrorResultWithData<T>($"There is no entity with ID : {id}", 404);

            return new SuccessResultWithData<T>("Entity found.", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResultWithData<T>($"An error occured while getting entity with ID : {id}.");
        }
    }
    public async Task<IServiceResultWithData<T>> GetActiveByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity is null)
                return new ErrorResultWithData<T>($"There is no active entity with ID : {id}", 404);

            return new SuccessResultWithData<T>("Active entity found.", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occured.");
            return new ErrorResultWithData<T>($"An error occured while getting entity with ID : {id}.");
        }
    }
}