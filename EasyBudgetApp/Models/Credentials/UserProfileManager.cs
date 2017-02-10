using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DataProvider;
using DataProvider.Interfaces;
using EasyBudgetApp.ViewModels;
using Newtonsoft.Json;

namespace EasyBudgetApp.Models.Credentials
{
    public class UserProfileManager
    {
        private readonly string _userProfileFilePath;
        private readonly string _companyProfileFilePath;

        public IList<UserProfile> UserProfiles { get; private set; }
        public IList<CompanyProfile> CompanyProfiles { get; private set; }
        public IList<IProviderDescriptor> DataProviders { get; private set; }

        public UserProfileManager()
        {
            _userProfileFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Models\Credentials\Data", @"UserProfile.json");
            _companyProfileFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Models\Credentials\Data", @"CompanyProfile.json");

            CompanyProfiles = LoadCompanyProfiles(_companyProfileFilePath);
            UserProfiles = LoadUserProfiles(_userProfileFilePath);
            DataProviders = LoadUserProviders(UserProfiles);

            NewAccountViewModel.WhenNewProfileAdded.Subscribe((newProfile) =>
            {
                if (UserProfiles.Contains(newProfile))
                {
                    var profile = UserProfiles.FirstOrDefault((p) => p.Equals(newProfile));
                    profile.Accounts = profile.Accounts.Union(newProfile.Accounts).ToList();
                }
                else
                {
                    UserProfiles.Add(newProfile);
                }

                SaveUserProfile();
            });
        }

        private IList<CompanyProfile> LoadCompanyProfiles(string profileFilePath)
        {
            if (string.IsNullOrEmpty(profileFilePath) || !File.Exists(profileFilePath))
            {
                throw new ArgumentException(@"Incorrect value of companyProfileFilePath - {0}", profileFilePath);
            }

            IList<CompanyProfile> profiles;
            using (StreamReader file = File.OpenText(profileFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                profiles = (List<CompanyProfile>)serializer.Deserialize(file, typeof(List<CompanyProfile>));
            }

            return profiles ?? new List<CompanyProfile>();
        }

        private IList<UserProfile> LoadUserProfiles(string profileFilePath)
        {
            if (string.IsNullOrEmpty(profileFilePath) || !File.Exists(profileFilePath))
            {
                throw new ArgumentException(@"Incorrect value of userProfileFilePath - {0}", profileFilePath);
            }

            IList<UserProfile> profiles;
            using (StreamReader file = File.OpenText(profileFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                profiles = (List<UserProfile>)serializer.Deserialize(file, typeof(List<UserProfile>));
            }

            if (profiles == null)
            {
                return new List<UserProfile>();
            }

            foreach (var profile in profiles)
            {
                foreach (var account in profile.Accounts)
                {
                    account.InstitutionName = profile.InstitutionName;
                    account.UserId = profile.UserId;
                }
            }
            return profiles;
        }

        private IList<IProviderDescriptor> LoadUserProviders(IList<UserProfile> userProfiles)
        {
            var userProviders = new List<IProviderDescriptor>();

            foreach (var userProfile in userProfiles)
            {
                var accounts = userProfile.Accounts.Select(accountProfile => accountProfile.ToAccount()).ToList();
                userProviders.Add(new ProviderDescriptor(userProfile.InstitutionName, userProfile.Credentials, accounts));
            }

            return userProviders;
        }

        private void SaveUserProfile()
        {
            using (StreamWriter file = File.CreateText(_userProfileFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, UserProfiles);
            }
        }

        public void RemoveUserAccountProfile(UserAccountProfile accountProfile)
        {
            var profile = UserProfiles.FirstOrDefault((p) => p.InstitutionName == accountProfile.InstitutionName);
            if (profile == null)
            {
                return;
            }

            profile.Accounts.Remove(accountProfile);
            SaveUserProfile();
        }
    }
}
