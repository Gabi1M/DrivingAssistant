using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IGenericService<T> : IDisposable
    {
        //============================================================
        public Task<IEnumerable<T>> GetAsync();

        //============================================================
        public Task<T> GetById(long id);

        //============================================================
        public Task<long> SetAsync(T data);

        //============================================================
        public Task DeleteAsync(T data);
    }
}
