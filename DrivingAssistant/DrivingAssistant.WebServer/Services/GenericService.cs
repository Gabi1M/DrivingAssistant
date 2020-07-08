﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrivingAssistant.WebServer.Services
{
    public abstract class GenericService<T> : IDisposable
    {
        //============================================================
        public abstract Task<ICollection<T>> GetAsync();

        //============================================================
        public abstract Task<long> SetAsync(T data);

        //============================================================
        public abstract Task UpdateAsync(T data);

        //============================================================
        public abstract Task DeleteAsync(long id);

        //============================================================
        public abstract void Dispose();
    }
}
