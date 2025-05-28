using McpNetwork.Charli.Managers.DatabaseManager.DataModel;
using McpNetwork.Charli.Managers.DatabaseManager.Validators;
using McpNetwork.Charli.Server.Exceptions;
using McpNetwork.Charli.Server.Models.Entities;
using McpNetwork.Charli.Server.Models.Interfaces.DataAccessLayers;

namespace McpNetwork.Charli.Managers.DatabaseManager.DataAccessLayers
{
    public class DeviceDal : IDeviceDal
    {

        private readonly string dbPath;

        internal DeviceDal(string dbPath)
        {
            this.dbPath = dbPath;
        }


        public IEnumerable<Device> FindDeviceForUser(int userId)
        {
            var result = new List<Device>();
            try
            {
                using (var dbContext = new CharliEntities(this.dbPath))
                {
                    result = dbContext.Devices.Where(d => d.UserId == userId).ToList();
                }
            }
            catch (Exception e)
            {
                throw new DalException("DAL Error", e);
            }
            return result;
        }

        public bool DeleteDeviceByNotificationToken(string notificationToken)
        {
            var fctResult = true;
            try
            {
                using (var dbContext = new CharliEntities(this.dbPath))
                {
                    Device device = dbContext.Devices.FirstOrDefault(s => s.NotificationToken == notificationToken);
                    if (device != null)
                    {
                        dbContext.Devices.Remove(device);
                    }
                }
            }
            catch (Exception e)
            {
                fctResult = false;
                throw new DalException("DAL Error", e);
            }
            return fctResult;

        }

        public bool UpdateNotificationToken(string oldNotificationToken, string newNotificationToken)
        {
            var fctResult = true;
            try
            {
                using (var dbContext = new CharliEntities(this.dbPath))
                {
                    var device = dbContext.Devices.FirstOrDefault(s => s.NotificationToken == oldNotificationToken);
                    if (device != null)
                    {
                        device.NotificationToken = newNotificationToken;
                    }
                }
            }
            catch (Exception e)
            {
                fctResult = false;
                throw new DalException("DAL Error", e);
            }
            return fctResult;

        }

        public bool RegisterDevice(Device device)
        {
            var validator = new DeviceValidator();
            var validation = validator.Validate(device);
            if (!validation.IsValid)
            {
                throw new DalValidationException(validation.Errors.First().ErrorMessage);
            }

            using (var dbContext = new CharliEntities(this.dbPath))
            {
                var dbDevice = dbContext.Devices.FirstOrDefault(d => d.DeviceKey == device.DeviceKey);
                if (dbDevice == null)
                {
                    dbDevice = new Device
                    {
                        CreationDate = DateTime.Now,
                    };
                    dbContext.Devices.Add(dbDevice);
                }

                dbDevice.Active = device.Active;
                dbDevice.DeviceKey = device.DeviceKey;
                dbDevice.NotificationToken = device.NotificationToken;
                dbDevice.OsVersion = device.OsVersion;
                dbDevice.Platform = device.Platform;
                dbDevice.UpdateDate = DateTime.Now;
                dbDevice.UserId = device.UserId;
                dbContext.SaveChanges();
            }
            return true;
        }

        public IEnumerable<Device> GetUserDevices(int userId)
        {
            using (var dbContext = new CharliEntities(this.dbPath))
            {
                return dbContext.Devices
                    .Where(d => d.UserId == userId)
                    .Where(d => d.Active)
                    .ToList();
            }

        }

    }
}
