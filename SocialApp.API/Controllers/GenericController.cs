using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Interfaces;
using SocialApp.Domain.Entities;

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
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _service.GetAllAsync();

            var errorResponse = HandleServiceResult(result);

            if (errorResponse != null)
                return errorResponse;

            var entities = result.Data;

            var dtoList = _mapper.Map<IEnumerable<TDto>>(entities);

            return Ok(dtoList);
        }
        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync([FromBody] TCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return HandleValidationErrors(validationResult.Errors);

            var entity = _mapper.Map<T>(dto);

            var addingResult = await _service.AddAsync(entity);

            var errorResponse = HandleServiceResult(addingResult);

            if (errorResponse != null)
                return errorResponse;

            return Ok(addingResult.Message);
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);

            var errorResponse = HandleServiceResult(result);

            if (errorResponse != null)
                return errorResponse;

            var entity = result.Data;

            var dto = _mapper.Map<TDto>(entity);

            return Ok(dto);
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] TUpdateDto dto)
        {
            var existingEntityResponse = await _service.GetByIdAsync(id);

            var existingEntityErrorResponse = HandleServiceResult(existingEntityResponse);

            if (existingEntityErrorResponse != null)
                return existingEntityErrorResponse;

            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return HandleValidationErrors(validationResult.Errors);

            var existingEntity = existingEntityResponse.Data;

            _mapper.Map(dto, existingEntity);

            var updateEntityResult = await _service.UpdateAsync(existingEntity);

            var updateEntityErrorResult = HandleServiceResult(updateEntityResult);

            if (updateEntityErrorResult != null)
                return updateEntityErrorResult;

            return Ok(updateEntityResult.Message);
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]int id)
        {
            var existingEntityResult = await _service.GetByIdAsync(id);

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
    }
}

