using Milton.Database.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Configuration
{
    public interface ISettingsService
    {
        IDictionary<String, Setting> GetAllSettings();
        Setting GetSettingByKey(String key);
        Boolean SettingExists(String key);

        void InsertSetting(Setting setting);
        void UpdateSetting(Setting setting);
        void DeleteSetting(Setting setting);

        //TODO: Remove the overloads to load by person id and country. No longer part of our roadmap.
        T LoadSetting<T>(String extraIdentifier = "") where T : ISettings, new();
        void SaveSetting<T>(T settings, String extraIdentifier = "") where T : ISettings, new();
        void DeleteSetting<T>(String extraIdentifier = "") where T : ISettings, new();
        void SetSetting<T>(String key, T value);
    }
}
