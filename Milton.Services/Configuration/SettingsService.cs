using Milton.Database;
using Milton.Database.Models.Configuration;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Configuration
{
    public class SettingsService : ISettingsService
    {
        #region Constants
        protected const String KEY = "Milton.Cache.Settings";
        protected const String PATTERN = "Milton.Cache.Settings";
        #endregion

        #region Fields
        protected ICacheManager _cacheManager;
        protected IDataRepository<Setting> _repository;
        #endregion

        #region Constructor
        public SettingsService(Services.Cache.ICacheManager cacheManager, IDataRepository<Setting> repository)
        {
            _cacheManager = cacheManager;
            _repository = repository;
        }
        #endregion

        /// <summary>
        /// Get all settings from the repository
        /// </summary>
        /// <returns>A Dicstionary of Setting objects</returns>
        public virtual IDictionary<String, Setting> GetAllSettings()
        {
            return _cacheManager.Get<IDictionary<String, Setting>>(KEY, 10, () =>
            {
                var result = _repository.Table;
                return result.ToDictionary<Setting, String>(s => GenerateDictionaryKey(s.Key));
            });
        }

        /// <summary>
        /// Get a setting by it key
        /// </summary>
        /// <param name="key">The key to match</param>
        /// <returns>A Setting object or null</returns>
        public virtual Setting GetSettingByKey(String key)
        {
            //Get all settings from the cache
            IDictionary<String, Setting> settings = GetAllSettings();

            //Find the correct setting
            if (settings.ContainsKey(GenerateDictionaryKey(key))) return settings[GenerateDictionaryKey(key)];
            else return null;

        }

        /// <summary>
        /// Checks if a setting exists
        /// </summary>
        /// <param name="key">The key to match</param>
        /// <returns>A Setting object or null</returns>
        public virtual bool SettingExists(string key)
        {
            return GetSettingByKey(key) != null;
        }

        /// <summary>
        /// Inserts a new setting in the database
        /// </summary>
        /// <param name="setting">A Setting object to insert</param>
        public virtual void InsertSetting(Setting setting)
        {
            _repository.Insert(setting);
            _cacheManager.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing setting
        /// </summary>
        /// <param name="setting"></param>
        public virtual void UpdateSetting(Setting setting)
        {
            _repository.Update(setting);
            _cacheManager.RemoveByPattern(PATTERN);

        }

        /// <summary>
        /// Deletes a setting from the database
        /// </summary>
        /// <param name="setting"></param>
        public virtual void DeleteSetting(Setting setting)
        {
            _repository.Delete(setting);
            _cacheManager.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Loads settings of the specified type from the repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T LoadSetting<T>(String extraIdentifier = "") where T : ISettings, new()
        {
            //Create the setting object
            var settings = Activator.CreateInstance<T>();
            //TODO:check if T is dictionary

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite) continue;

                //The key to the property
                String key = typeof(T).Name + "." + prop.Name;
                if (!String.IsNullOrEmpty(extraIdentifier)) key += "." + extraIdentifier;

                //Get the setting
                Setting setting = GetSettingByKey(key);
                if (setting == null) continue;

                //Get the type converter
                TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);

                //Check if the type can be converted from a string
                if (!converter.CanConvertFrom(typeof(String))) continue;

                //Check if the setting value can be converted into the target type
                if (!converter.IsValid(setting.Value)) continue;

                //Get the value of the property
                Object value = converter.ConvertFromInvariantString(setting.Value);

                //Set the property
                prop.SetValue(settings, value);
            }

            //Return the settings object
            return settings;
        }

        /// <summary>
        /// Save a settings object to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        public virtual void SaveSetting<T>(T settings, String extraIdentifier = "") where T : ISettings, new()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite) continue;

                //The key to the property
                String key = typeof(T).Name + "." + prop.Name;
                if (!String.IsNullOrEmpty(extraIdentifier)) key += "." + extraIdentifier;

                //Get the value
                dynamic value = prop.GetValue(settings, null);

                //Set the setting
                SetSetting(key, (value != null ? value : ""));
            }
        }

        /// <summary>
        /// Deletes all settings from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual void DeleteSetting<T>(String extraIdentifier = "") where T : ISettings, new()
        {
            var toDelete = new List<Setting>();
            foreach (var prop in typeof(T).GetProperties())
            {
                string key = typeof(T).Name + "." + prop.Name;
                if (!String.IsNullOrEmpty(extraIdentifier)) key += "." + extraIdentifier;
                toDelete.Add(GetSettingByKey(key));
            }

            foreach (var setting in toDelete)
                DeleteSetting(setting);
        }

        /// <summary>
        /// Insert ot update a setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void SetSetting<T>(string key, T value)
        {
            if (key == null) throw new ArgumentNullException("key");

            //Always store lowercase keys
            key = key.Trim().ToLowerInvariant();

            //Update an existing setting
            if (SettingExists(key))
            {
                Setting setting = GetSettingByKey(key);
                setting.Value = value.ToString();
                UpdateSetting(setting);
            }

            //Insert a new setting
            else
            {
                InsertSetting(new Setting()
                {
                    Key = key,
                    Value = value.ToString()
                });

            }
        }

        private String GenerateDictionaryKey(String baseKey)
        {
            //The key to the property
            String key = "{0}";

            //Always use lowercase keys
            baseKey = baseKey.ToLower();

            //Get person/region/country specific setting key
            return String.Format(key,
                baseKey);
        }
    }
}
