using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrivingAssistant.WebServer.Services.Generic
{
    public interface IGenericService<T> : IDisposable
    {
        //============================================================
        public Task<ICollection<T>> GetAsync();

        //============================================================
        public Task<long> SetAsync(T data);

        //============================================================
        public Task DeleteAsync(T data);
    }
}
