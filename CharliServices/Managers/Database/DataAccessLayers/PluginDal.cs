using McpNetwork.Charli.Managers.DatabaseManager.DataModel;
using McpNetwork.Charli.Managers.DatabaseManager.Validators;
using McpNetwork.Charli.Server.Exceptions;
using McpNetwork.Charli.Server.Models.Entities;
using McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers;

namespace McpNetwork.Charli.Managers.DatabaseManager.DataAccessLayers
{
    public class PluginDal : IPluginDal
    {

        private readonly string dbPath;

        internal PluginDal(string dbPath)
        {
            this.dbPath = dbPath;
        }


        public IEnumerable<Plugin> GetPlugins()
        {
            var result = new List<Plugin>();
            try
            {
                using (var dbContext = new CharliEntities(this.dbPath))
                {

                    result = dbContext.Plugins.ToList();
                }
            }
            catch (Exception e)
            {
                throw new DalException("DAL Error", e);
            }
            return result;
        }

        public bool RegisterPlugin(Plugin plugin)
        {
            var result = true;
            try
            {
                var validator = new PluginValidator();
                var validationResult = validator.Validate(plugin);
                if (validationResult.IsValid)
                {
                    using (var dbContext = new CharliEntities(this.dbPath))
                    {

                        dbContext.Plugins.Add(plugin);
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    throw new DalValidationException(validationResult.Errors.First().ErrorMessage);
                }
            }
            catch (DalValidationException)
            {
                throw;
            }
            catch (Exception e)
            {
                result = false;
                throw new DalException("DAL Error", e);
            }
            return result;

        }

        public bool PluginIsActive(string pluginName, Version version)
        {
            var result = false;
            try
            {
                using (var dbContext = new CharliEntities(this.dbPath))
                {
                    var pluginVersion = version.ToString(3);
                    var plugin = dbContext.Plugins
                        .Where(x => x.Version.StartsWith(pluginVersion))
                        .FirstOrDefault(p => p.Name == pluginName);
                    if (plugin != null)
                    {
                        result = plugin.Active;
                    }
                }
            }
            catch (Exception e)
            {
                throw new DalException("DAL Error", e);
            }

            return result;
        }

    }
}
