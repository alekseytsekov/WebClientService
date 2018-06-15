using HttpRequestWrapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JsonConvertWrapper;

namespace WebClientService
{
    public interface IService
    {
        string StatusCodeToMessage(int statusCode);

        Task<(bool, string, TEntity)> CreateEntityAsync<TEntity, TDto>(IHttpRequestWrapper http, IJsonConverter converter, string url, TDto dto);
        Task<(bool, string)> EditEntityAsync<TEntity, TDto>(IHttpRequestWrapper http, IJsonConverter converter, string url, TDto dto);
        Task<(bool, string)> DeleteEntityAsync(IHttpRequestWrapper http, string url);

        Task<(bool, string, TDto)> GetEntityAsync<TEntity, TDto>(IHttpRequestWrapper http, IJsonConverter converter, string url);
        Task<(bool, string, IEnumerable<TDto>)> GetCollectionAsync<TEntity, TDto>(IHttpRequestWrapper http, IJsonConverter converter, string url);
        Task<(bool, string, IEnumerable<T>)> GetCollectionAsync<T>(IHttpRequestWrapper http, IJsonConverter converter, string url);

        TDto MapEntityToDto<TEntity, TDto>(TEntity entity);
        IEnumerable<TDto> MapEntityToDto<TEntity, TDto>(IEnumerable<TEntity> entities);

        TEntity MapDtoToEntity<TDto, TEntity>(TDto dto);
        IEnumerable<TEntity> MapDtoToEntity<TDto, TEntity>(IEnumerable<TDto> dtos);
    }
}
