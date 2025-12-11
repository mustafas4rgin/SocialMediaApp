using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Serilog;
using SocialApp.Application.Helpers;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;
using SocialApp.Domain.Results.Error;
using SocialApp.Domain.Results.Success;

namespace SocialApp.Application.Services;

public class RoleService : GenericService<Role>, IRoleService
{
    private readonly IValidator<Role> _validator;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
    IValidator<Role> validator,
    IRoleRepository roleRepository,
    ILogger<RoleService> logger
    ) : base(validator, roleRepository, logger)
    {
        _logger = logger;
        _validator = validator;
        _roleRepository = roleRepository;
    }
    public async Task<IServiceResultWithData<IEnumerable<Role>>> GetRolesWithIncludesAsync(QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var roles = await _roleRepository.GetAllRolesAsync(param.Include, ct);

            if (!roles.Any())
                return new ErrorResultWithData<IEnumerable<Role>>("Roles not found.");

            return new SuccessResultWithData<IEnumerable<Role>>("Roles found.", roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResultWithData<IEnumerable<Role>>("An unexpected error occured.");
        }
    }
    public async Task<IServiceResultWithData<Role>> GetRoleByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct = default)
    {
        try
        {
            var role = await _roleRepository.GetRoleByIdAsync(id, param.Include, ct);

            if (role is null)
                return new ErrorResultWithData<Role>($"There is no role with ID : {id}");

            return new SuccessResultWithData<Role>("Role found", role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResultWithData<Role>("An unexpected error occured.");
        }
    }
    public override async Task<IServiceResult> UpdateAsync(Role role, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(role, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));
                
            var roleExists = await _roleRepository.RoleNameCheckAsync(StringHelper.Normalize(role.Name), ct);

            if (roleExists)
                return new ErrorResult($"Role already exists with name : {role.Name}");
            
            await _roleRepository.UpdateAsync(role, ct);
            await _roleRepository.SaveChangesAsync();

            return new SuccessResult("Role updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResult("An unexpected error occured.");
        }
    }
    public override async Task<IServiceResult> AddAsync(Role role, CancellationToken ct = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(role, ct);

            if (!validationResult.IsValid)
                return new ErrorResult(string.Join(" | ",
                    validationResult.Errors.Select(e => e.ErrorMessage)));
            
            var roleExists = await _roleRepository.RoleNameCheckAsync(StringHelper.Normalize(role.Name), ct);

            if (roleExists)
                return new ErrorResult($"Role already exists with name : {role.Name}");
            
            await _roleRepository.AddAsync(role, ct);
            await _roleRepository.SaveChangesAsync();

            return new SuccessResult("Role added successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ErrorResult("An unexpected error occured.");
        }
    }
}