using DotHass.Repository;
using DotHass.Sample.Model.Data;
using DotHass.Sample.Model.Identity;
using IdGen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotHass.Sample.Service
{

    public class RoleService
    {
        public CacheRepository<GameRole> dataRepos;

        private IdGenerator IdGen;

        public long GeneratorId
        {
            get
            {
                return IdGen.CreateId();
            }
        }

        public RoleService(CacheRepository<GameRole> repository
            , IdGenerator idGen
            )
        {
            this.dataRepos = repository;
            IdGen = idGen;
        }

        public GameRole GetRole(long value)
        {
            return this.dataRepos.Find(value);
        }

        public bool IsNickName(string nickName)
        {
            return dataRepos.Exist(x => x.RoleName.ToLower() == nickName.ToLower());
        }

        public GameRole CreateRole(GameUser user)
        {
            var role = new GameRole()
            {
                Id = GeneratorId,
                RoleName = "",
                HeadId = "00000",
                Money = 11111,
                Gold = 11111,
                Experience = 0,
            };

            this.dataRepos.Add(role);

            return role;
        }




        public void UpdateRole(GameRole role)
        {
            dataRepos.Update(role);
        }
    }
}
