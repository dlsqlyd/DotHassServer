using DotHass.Middleware.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotHass.Middleware.Mvc.Controller
{
    public class ControllerProvider : IControllerProvider
    {
        private IList<ControllerDescriptor> _collection;
        private ApplicationPartManager _partManager;
        private ControllerOptions options;

        public ControllerProvider(ApplicationPartManager partManager, IOptions<ControllerOptions> options)
        {
            this.options = options.Value;
            _partManager = partManager;
        }

        public void Initialize()
        {
            if (this._collection == null)
            {
                this._collection = new List<ControllerDescriptor>();
                foreach (var item in GetControllerTypes())
                {
                    this._collection.Add(new ControllerDescriptor(item, options));
                }
            }
        }

        public ControllerDescriptor GetControllerDescriptor(int id)
        {
            this.Initialize();
            return this._collection.Where(d => d.ContractID == id).FirstOrDefault();
        }

        private IEnumerable<TypeInfo> GetControllerTypes()
        {
            var feature = new ControllerFeature();
            _partManager.PopulateFeature(feature);
            return feature.Controllers;
        }
    }
}