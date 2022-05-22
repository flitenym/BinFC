﻿using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Entities.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Module.Repositories.Interfaces
{
    public interface ISpotDataRepository
    {
        public IEnumerable<SpotData> Get();
        public void Create(Data obj);
        public Task<string> DeleteAsync();
        public Task<string> SaveChangesAsync();
    }
}