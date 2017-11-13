using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using System.Windows.Input;
using Xamarin.Forms;

namespace NoteKeeperClient.ViewModels
{
    class CreateNoteScreenViewModel : BaseViewModel
    {
        private User _user;
        private Tag _selectedTag;
        private List<Note> _notes;
        private Action _goBack;
        private Action _applyLocalChanges;

        public CreateNoteScreenViewModel(IRestClient client, User user, Tag selectedTag, List<Note> notes, Action goBack, Action applyLocalChanges) : base(client)
        {
            _user = user;
            _selectedTag = selectedTag;
            HasTag = _selectedTag != null;
            _notes = notes;
            _goBack = goBack;
            _applyLocalChanges = applyLocalChanges;
            Heading = "";
            Text = "";

            CreateNoteCommand = new Command(async () =>
            {
                IsBusy = true;
                if (Text == "" || Heading == "")
                {
                    Error = "Поля не могут быть пустыми";
                    IsBusy = false;
                    return;
                }

                try
                {
                    var note = new Note()
                    {
                        OwnerId = _user.Id,
                        Heading = Heading,
                        Text = Text
                    };
                    note = await _restClient.CreateNoteAsync(note);
                    note.TagNames = new List<string>();
                    if (HasTag)
                    {
                        await _restClient.AddTagToNoteAsync(note, _selectedTag);

                        (note.TagNames as List<string>).Add(_selectedTag.Name);
                    }

                    _notes.Add(note);
                    _applyLocalChanges();
                    _goBack();
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;
                    IsBusy = false;
                }
            });
        }

        public string Heading { get; set; }
        public string Text { get; set; }
        public bool HasTag { get; set; }
        public Tag SelectedTag
        {
            get
            {
                return _selectedTag;
            }
        }

        public ICommand CreateNoteCommand { get; set; }
    }
}
