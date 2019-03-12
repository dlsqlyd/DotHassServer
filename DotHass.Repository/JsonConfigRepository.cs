using DotHass.Repository.Entity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotHass.Repository
{
    public class JsonConfigOptions
    {
        public string Path { get; set; }
    }

    public class JsonConfigRepository<TEntity> : ReadonlyCacheRepository<TEntity> where TEntity : class
    {
        protected string _storagePath;

        /// <summary>
        ///
        /// </summary>
        /// <param name="storagePath">Path to the directory.  The XML filename is determined by the TypeName</param>
        /// <param name="cachingStrategy"></param>
        public JsonConfigRepository(IHostingEnvironment environment, IOptions<JsonConfigOptions> options, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _storagePath = Path.Combine(environment.ContentRootPath, options.Value.Path);

            if (!_storagePath.EndsWith(@"\"))
            {
                _storagePath += @"\";
            }
            _storagePath = String.Format("{0}{1}.json", _storagePath, TypeName);
        }

        protected override bool LoadDataFactory(bool isReload)
        {
            if (!File.Exists(_storagePath)) return false;

            using (var stream = new FileStream(_storagePath, FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                List<TEntity> _items = JsonConvert.DeserializeObject<List<TEntity>>(json);

                if (_items.Count == 0) return true;

                return InitCache(_items, isReload);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="periodTime"></param>
        /// <param name="isReplace"></param>
        /// <returns></returns>
        protected bool InitCache(List<TEntity> dataList, bool isReplace)
        {
            Collection.Clear();
            foreach (var data in dataList)
            {
                Collection.Add(EntityKeyInfo.GetPrimaryKey(data), data);
            }
            return true;
        }

        protected override bool LoadItemFactory(object[] keys, bool isReplace)
        {
            return true;
        }
    }
}