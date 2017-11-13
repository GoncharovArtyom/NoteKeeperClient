using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteKeeperClient.RestClient;
using NoteKeeper.Model;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace NoteKeeperClient.ViewModels
{
    class CreateTagScreenViewModel : BaseViewModel
    {
        Action<Tag> _applyLocalChanges;
        Action _goBack;
        User _user;

        public CreateTagScreenViewModel(IRestClient client, User user, Action<Tag> applyLocalChanges, Action goBack) : base(client)
        {
            _applyLocalChanges = applyLocalChanges;
            _goBack = goBack;
            _user = user;

            CreateTagCommand = new Command(async () =>
            {
                IsBusy = true;

                if (Name == "" || Name == null)
                {
                    Error = "Название тега не может быть пустым";

                    IsBusy = false;
                    return;
                }
                try
                {
                    var tag = new Tag()
                    {
                        OwnerId = _user.Id,
                        Name = _name
                    };
                    tag = await _restClient.CreateTagAsync(tag);
                    IsBusy = false;
                    _applyLocalChanges(tag);
                    _goBack();
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;
                    IsBusy = false;
                    return;
                }
            });
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateTagCommand { get; set; }
    }
}
