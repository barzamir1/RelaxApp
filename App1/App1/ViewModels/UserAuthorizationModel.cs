using App1.DataObjects;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App1.ViewModels
{
    class UserAuthorizationModel
    {
        private ObservableCollection<Users> _allowedUsers;
        private List<Users> allUsers;
        private AzureDataService _azureDataService = AzureDataService.Instance;

        public event PropertyChangedEventHandler PropertyChanged;
        private static UserAuthorizationModel _instance;

        public UserAuthorizationModel()
        {
            if (_instance == null)
            {
                _allowedUsers = new ObservableCollection<Users>();
            }
        }
        public static async Task<UserAuthorizationModel> GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UserAuthorizationModel();
                await _instance.InitializeAuthorizedUsers();
            }
            return _instance;

        }
        public async Task InitializeAuthorizedUsers()
        {
            allUsers = await _azureDataService.GetAllUsers();
            var authUsers = await _azureDataService.GetAllAuthUsers();

            foreach (var currUserID in authUsers)
            {
                var currUser = allUsers.Where(item => item.id == currUserID.AuthorizedToViewUserID).ToList();
                if (currUser != null && currUser.Count > 0)
                    AllowedUsers.Add(currUser[0]);
            }
        }

        public ObservableCollection<Users> AllowedUsers
        {
            get { return _allowedUsers; }
            set
            {
                _allowedUsers = value;
                OnPropertyChanged("AllowedUsers");
            }
        }
        public bool AddAuthUser(String shortUserID)
        {
            var uList = allUsers.Where(item => item.shortID == shortUserID).ToList();
            if (uList != null && uList.Count > 0)
            {
                var userToAdd = uList[0];
                if (AllowedUsers.Contains(userToAdd))
                    return true; //already allowed to view
                UserAuthorizations userAuthorizations = new UserAuthorizations();
                userAuthorizations.TherapistID = Login.Default.CurrentUser.id;
                userAuthorizations.AuthorizedToViewUserID = userToAdd.id;
                _azureDataService.AddAuthUser(userAuthorizations); //doesn't wait
                AllowedUsers.Add(userToAdd);
                return true;
            }
            else
                return false;
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
