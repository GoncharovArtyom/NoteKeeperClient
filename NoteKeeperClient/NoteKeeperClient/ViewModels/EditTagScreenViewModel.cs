using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NoteKeeperClient.ViewModels
{
    class EditTagScreenViewModel : BaseViewModel
    {
        Action _applyLocalChanges;
        Action _goBack;
        User _user;
        Tag _tag;
        List<Note> _notes;

        public EditTagScreenViewModel(IRestClient client, User user, Tag tag, List<Note> notes, Action applyLocalChanges, Action goBack) : base(client)
        {
            _notes = notes;
            _applyLocalChanges = applyLocalChanges;
            _goBack = goBack;
            _user = user;
            _tag = tag;
            Name = _tag.Name;

            EditTagCommand = new Command(async () =>
            {
                IsBusy = true;

                if (Name == "" || Name == null)
                {
                    Error = "Название тега не может быть пустым";

                    IsBusy = false;
                    return;
                }
                if (Name == _tag.Name)
                {
                    return;
                }

                try
                {
                    await _restClient.ChangeTagNameAsync(_tag, Name);
                    
                    foreach(var note in _notes)
                    {
                        var tempList = note.TagNames.ToList();
                        var oldTagNameIndex = tempList.FindIndex((string item)=> { return item == tag.Name; });
                        if (oldTagNameIndex != -1)
                        {
                            tempList[oldTagNameIndex] = Name;
                            note.TagNames = tempList;
                        }

                    }
                    tag.Name = Name;

                    IsBusy = false;
                    _applyLocalChanges();
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

        public ICommand EditTagCommand { get; set; }
    }
}
