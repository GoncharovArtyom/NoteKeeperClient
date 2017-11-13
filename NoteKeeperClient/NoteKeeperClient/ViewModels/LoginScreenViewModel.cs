using NoteKeeperClient.RestClient;
using NoteKeeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;
using Xamarin.Forms;
using System.Net.Http;

namespace NoteKeeperClient.ViewModels
{
    class LoginScreenViewModel : BaseViewModel
    {
        public LoginScreenViewModel(IRestClient restClient, Func<User, Task> navigateToMainScreenAsync) : base(restClient)
        {
            _navigateToMainScreenAsync = navigateToMainScreenAsync;
            LogInCommand = new Command(LogIn);
            CreateUserCommand = new Command(CreateUser);
            _logInEmail = "";
            _createEmail = "";
            _createName = "";
            _isBusy = false;
        }

        private Func<User, Task> _navigateToMainScreenAsync;

        #region commands
        public ICommand LogInCommand { get; private set; }
        public ICommand CreateUserCommand { get; private set; }
        #endregion

        #region bindable properties
        private string _logInEmail;
        public string LogInEmail
        {
            get
            {
                return _logInEmail;
            }
            set
            {
                _logInEmail = value;
                OnPropertyChanged();
            }
        }

        private string _createEmail;
        public string CreateEmail
        {
            get
            {
                return _createEmail;
            }
            set
            {
                _createEmail = value;
                OnPropertyChanged();
            }
        }

        private string _createName;
        public string CreateName
        {
            get
            {
                return _createName;
            }
            set
            {
                _createName = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region methods
        private async void LogIn()
        {
            IsBusy = true;

            if (LogInEmail == "")
            {
                Error = "Поле Email не может быть пустым";

                IsBusy = false;
                return;
            }

            try
            {
                var user = await _restClient.GetUserByEmailAsync(LogInEmail);
                if (user == null)
                {
                    Error = "Такого пользователя не существует";

                    IsBusy = false;
                    return;
                }

                await _navigateToMainScreenAsync(user);
            }
            catch (RestClientException ex)
            {
                Error = ex.Message;

                IsBusy = false;
                return;
            }
        }
        private async void CreateUser()
        {
            IsBusy = true;

            if (CreateEmail == "" || CreateName == "")
            {
                Error = "Все поля должны быть заполнены";

                IsBusy = false;
                return;
            }

            try
            {
                var user = await _restClient.CreateUserAsync(new User() { Name = CreateName, Email = CreateEmail });
                await _navigateToMainScreenAsync(user);
            } catch (RestClientException ex)
            {
                Error = ex.Message;

                IsBusy = false;
                return;
            }
        }
        #endregion
    }
}
