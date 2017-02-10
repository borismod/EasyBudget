using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EasyBudgetApp
{
    public class InactiveTransactionsManager : IDisposable
    {
        private static IDictionary<long, bool> _inactive;
        private readonly string _activityFilePath;

        public InactiveTransactionsManager()
        {
            _inactive = new Dictionary<long, bool>();

            _activityFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Inactive.json");
            if (string.IsNullOrEmpty(_activityFilePath) || !File.Exists(_activityFilePath))
            {
                throw new ArgumentException(@"Incorrect value of activityFilePath - {0}", _activityFilePath);
            }

            LoadActivities(_activityFilePath);
        }

        private static void LoadActivities(string activityFilePath)
        {
            using (StreamReader file = File.OpenText(activityFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                _inactive = (IDictionary<long, bool>) serializer.Deserialize(file, typeof (IDictionary<long, bool>));
            }
        }

        public static bool IsActive(long id)
        {
            return !_inactive.ContainsKey(id);
        }

        public static bool SetActivity(long id, bool isActive)
        {
            if (isActive && _inactive.ContainsKey(id))
            {
                _inactive.Remove(id);
                return false;
            }

            if (!isActive && !_inactive.ContainsKey(id))
            {
                _inactive.Add(id, false);
                return true;
            }

            return isActive;
        }

        public void Dispose()
        {
            SaveActivities();
        }

        private void SaveActivities()
        {
            using (StreamWriter file = File.CreateText(_activityFilePath))
            {
                JsonSerializer serializer = new JsonSerializer {Formatting = Formatting.Indented};
                serializer.Serialize(file, _inactive);
            }
        }
    }
}
