using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T, TDto, TCreateDto, TUpdateDto> : BaseApiController
    where T : EntityBase
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
    {
        private readonly IValidator<TCreateDto> _createValidator;
        private readonly IValidator<TUpdateDto> _updateValidator;
        private readonly IGenericService<T> _service;
        private readonly IMapper _mapper;
        public GenericController(
        IValidator<TCreateDto> createValidator,
        IValidator<TUpdateDto> updateValidator,
        IGenericService<T> service,
        IMapper mapper
        )
        {
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _service = service;
            _mapper = mapper;
        }
        [HttpGet("GetAll")]
        public virtual async Task<IActionResult> GetAllAsync([FromQuery] QueryParameters param, CancellationToken ct = default)
        {
            var result = await _service.GetAllAsync(ct);

            var errorResponse = HandleServiceResult(result);

            if (errorResponse != null)
                return errorResponse;

            var entities = result.Data;

            var dtoList = _mapper.Map<IEnumerable<TDto>>(entities);

            return Ok(dtoList);
        }
        [HttpPost("Add")]
        public virtual async Task<IActionResult> AddAsync([FromBody] TCreateDto dto, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                return HandleValidationErrors(validationResult.Errors);

            var entity = _mapper.Map<T>(dto);

            var addingResult = await _service.AddAsync(entity, ct);

            var errorResponse = HandleServiceResult(addingResult);

            if (errorResponse != null)
                return errorResponse;

            return Ok(addingResult);
        }
        [HttpGet("{id:int}/getbyid")]
        public virtual async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromQuery] QueryParameters parma, CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id, ct);

            var errorResponse = HandleServiceResult(result);

            if (errorResponse != null)
                return errorResponse;

            var entity = result.Data;

            var dto = _mapper.Map<TDto>(entity);

            return Ok(dto);
        }
        [HttpPut("{id:int}/update")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] TUpdateDto dto, CancellationToken ct)
        {
            var existingEntityResponse = await _service.GetByIdAsync(id, ct);

            var existingEntityErrorResponse = HandleServiceResult(existingEntityResponse);

            if (existingEntityErrorResponse != null)
                return existingEntityErrorResponse;

            var validationResult = await _updateValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                return HandleValidationErrors(validationResult.Errors);

            var existingEntity = existingEntityResponse.Data;

            _mapper.Map(dto, existingEntity);

            var updateEntityResult = await _service.UpdateAsync(existingEntity, ct);

            var updateEntityErrorResult = HandleServiceResult(updateEntityResult);

            if (updateEntityErrorResult != null)
                return updateEntityErrorResult;

            return Ok(updateEntityResult.Message);
        }
        [HttpDelete("{id:int}/delete")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken ct)
        {
            var existingEntityResult = await _service.GetByIdAsync(id, ct);

            var existingEntityErrorResult = HandleServiceResult(existingEntityResult);

            if (existingEntityErrorResult != null)
                return existingEntityErrorResult;

            var existingEntity = existingEntityResult.Data;

            var deleteResult = await _service.DeleteAsync(existingEntity);

            var deleteErrorResult = HandleServiceResult(deleteResult);

            if (deleteErrorResult != null)
                return deleteErrorResult;

            return Ok(deleteResult.Message);
        }
        [HttpPut("{id:int}/restore")]
        public async Task<IActionResult> RestoreAsync([FromRoute] int id, CancellationToken ct)
        {
            var deletedEntityResult = await _service.GetByIdAsync(id, ct);

            var deletedEntityErrorResult = HandleServiceResult(deletedEntityResult);

            if (deletedEntityErrorResult != null)
                return deletedEntityErrorResult;

            var entity = deletedEntityResult.Data;

            var entityRestoreResult = await _service.RestoreAsync(entity, ct);

            var entityRestoreErrorResult = HandleServiceResult(entityRestoreResult);

            if (entityRestoreErrorResult != null)
                return entityRestoreErrorResult;

            return Ok(entityRestoreResult.Message);
        }
        [HttpDelete("{id:int}/soft-delete")]
        public async Task<IActionResult> SoftDeleteAsync([FromRoute]int id, CancellationToken ct)
        {
            var existingEntityResult = await _service.GetActiveByIdAsync(id, ct);

            var existingEntityErrorResult = HandleServiceResult(existingEntityResult);

            if (existingEntityErrorResult != null)
                return existingEntityErrorResult;

            var entity = existingEntityResult.Data;

            var entitySoftDeleteResult = await _service.SoftDeleteAsync(entity, ct);

            var entitySoftDeleteErrorResult = HandleServiceResult(entitySoftDeleteResult);

            if (entitySoftDeleteErrorResult != null)
                return entitySoftDeleteErrorResult;

            return Ok(entitySoftDeleteResult.Message);
        }
    }
}

