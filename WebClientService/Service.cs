using HttpRequestWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonConvertWrapper;

namespace WebClientService
{
    public abstract class Service : IService
    {
        private const int DefaultStatusCodeForInCorrectResponse = 300;
        public string StatusCodeToMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 200 : 
                case 204 : return "...";
                case 201 : return "...";
                case 400 : return "...";
                case 404 : return "...";
                case 500 :
                default :
                    return "...";
            }
        }


        public async Task<(bool, string, TEntity)> CreateEntityAsync<TEntity, TDto>(IHttpRequestWrapper http, IJsonConverter converter, string url, TDto dto)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return (false, "Invalid url!", default(TEntity));
            }

            if (dto.Equals(default(TDto)))
            {
                return (false, "Invalid dto!", default(TEntity));
            }

            TEntity entity = this.MapDtoToEntity<TDto, TEntity>(dto);
            
            string entityAsJson = converter.SerializeTo(entity);
            
            (int statusCode, string  response) = await http.PostAsync(url, entityAsJson);

            if (statusCode > DefaultStatusCodeForInCorrectResponse)
            {
                return (false, this.StatusCodeToMessage(statusCode), default(TEntity));
            }

            if (String.IsNullOrWhiteSpace(response))
            {
                return (true, this.StatusCodeToMessage(statusCode), default(TEntity));
            }

            TEntity result = converter.DeserializeTo<TEntity>(response);

            return (true, string.Empty, result);
        }

        public async Task<(bool, string)> EditEntityAsync<TEntity, TDto>(IHttpRequestWrapper http, IJsonConverter converter, string url, TDto dto)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return (false, "Invalid url!");
            }

            if (dto.Equals(default(TDto)))
            {
                return (false, "Invalid dto!");
            }

            TEntity entity = this.MapDtoToEntity<TDto, TEntity>(dto);
            string data = converter.SerializeTo(entity);

            (int statusCode, string response) = await http.PutAsync(url, data);

            if (statusCode > DefaultStatusCodeForInCorrectResponse)
            {
                return (false, this.StatusCodeToMessage(statusCode));
            }

            return (true, this.StatusCodeToMessage(statusCode));
        }

        public async Task<(bool, string)> DeleteEntityAsync(IHttpRequestWrapper http, string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return (false, "Invalid url!");
            }

            (int statusCode, string response) = await http.DeleteAsync(url);

            if (statusCode > DefaultStatusCodeForInCorrectResponse)
            {
                return (false, this.StatusCodeToMessage(statusCode));
            }

            return (true, this.StatusCodeToMessage(statusCode));
        }

        
        public async Task<(bool, string, TDto)> GetEntityAsync<TEntity,TDto>(IHttpRequestWrapper http, IJsonConverter converter, string url)
        {
            (int statusCode, string response) = await http.GetAsync(url);

            if (statusCode > DefaultStatusCodeForInCorrectResponse)
            {
                return (false, this.StatusCodeToMessage(statusCode), default(TDto));
            }

            if (String.IsNullOrWhiteSpace(response))
            {
                return (false, "Entity not found", default(TDto));
            }

            TEntity entity = converter.DeserializeTo<TEntity>(response);
            TDto dto = this.MapEntityToDto<TEntity, TDto>(entity);

            return (true, string.Empty, dto);
        }

        public async Task<(bool, string, IEnumerable<TDto>)> GetCollectionAsync<TEntity, TDto>(IHttpRequestWrapper http, IJsonConverter converter, string url)
        {
            (int statusCode, string response) = await http.GetAsync(url);

            if (statusCode > DefaultStatusCodeForInCorrectResponse)
            {
                return (false, this.StatusCodeToMessage(statusCode), null);
            }

            if (String.IsNullOrWhiteSpace(response))
            {
                return (true, this.StatusCodeToMessage(statusCode), Enumerable.Empty<TDto>());
            }

            IEnumerable<TEntity> entities = converter.DeserializeToEnumerable<TEntity>(response);
            IEnumerable<TDto> dtos = this.MapEntityToDto<TEntity, TDto>(entities);

            return (true, string.Empty, dtos);
        }

        public async Task<(bool, string, IEnumerable<T>)> GetCollectionAsync<T>(IHttpRequestWrapper http, IJsonConverter converter, string url)
        {
            (int statusCode, string response) = await http.GetAsync(url);

            if (statusCode > DefaultStatusCodeForInCorrectResponse)
            {
                return (false, this.StatusCodeToMessage(statusCode), null);
            }

            if (String.IsNullOrWhiteSpace(response))
            {
                return (true, this.StatusCodeToMessage(statusCode), Enumerable.Empty<T>());
            }

            IEnumerable<T> entities = converter.DeserializeToEnumerable<T>(response);

            return (true, string.Empty, entities);
        }


        public abstract TDto MapEntityToDto<TEntity, TDto>(TEntity entity);

        public abstract IEnumerable<TDto> MapEntityToDto<TEntity, TDto>(IEnumerable<TEntity> entities);

        public abstract TEntity MapDtoToEntity<TDto, TEntity>(TDto dto);

        public abstract IEnumerable<TEntity> MapDtoToEntity<TDto,TEntity>(IEnumerable<TDto> dtos);
    }
}
