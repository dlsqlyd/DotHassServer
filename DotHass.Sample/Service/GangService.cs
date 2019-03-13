using System;
using DotHass.Repository;
using DotHass.Sample.Config;

namespace DotHass.Sample.Service
{
    public class GangService
    {
        private JsonConfigRepository<GangConfig> dataRepository;

        public GangService(JsonConfigRepository<GangConfig> repository)
        {
            this.dataRepository = repository;
        }

        internal GangConfig GetById(int v)
        {
            return this.dataRepository.Find(v);
        }
    }
}