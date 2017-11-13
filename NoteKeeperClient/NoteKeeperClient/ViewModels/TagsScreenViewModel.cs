using NoteKeeperClient.RestClient;
using NoteKeeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace NoteKeeperClient.ViewModels
{
    class TagsScreenViewModel : BaseViewModel
    {
        List<Note> _notes;
        Action _applyLocalChanges;
        User _user;
        List<Tag> _allTags;

        public TagsScreenViewModel(IRestClient client, User user, List<Tag> tags, List<Note> notes, Action applyLocalChanges, Action<User, Action<Tag>> navigateToCreateScreen, Action<User, Tag,List<Note>, Action> navigateToEditScreen) : base(client)
        {
            _applyLocalChanges = applyLocalChanges;
            _user = user;
            _allTags = tags;

            _tags = new List<Tag>();
            for (int i = 1; i < tags.Count() - 1; ++i)
            {
                Tags.Add(tags[i]);
            }
            
            _notes = notes;

            DeleteTagCommand = new Command(async (object parameter) =>
            {
                var tag = parameter as Tag;
                IsBusy = true;
                try
                {
                    await _restClient.DeleteTagAsync(tag);

                    _allTags.Remove(tag);
                    _tags.Remove(tag);
                    Tags = new List<Tag>(_tags);

                    foreach (var note in _notes)
                    {
                        var tempList = note.TagNames.ToList();
                        tempList.Remove(tag.Name);
                        note.TagNames = tempList;
                    }

                    applyLocalChanges();
                    IsBusy = false;
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;

                    IsBusy = false;
                }
            });
            CreateTagCommand = new Command(() =>
            {
                Action<Tag> callBackAction = (Tag _tag) =>
                {
                    _allTags.Insert(tags.Count() - 1, _tag);
                    _tags.Add(_tag);
                    Tags = new List<Tag>(_tags);

                    applyLocalChanges();
                };

                navigateToCreateScreen(_user, callBackAction);
            });
            EditTagCommand = new Command((object parameter) =>
            {
                var tag = parameter as Tag;
                Action callBackAction = () =>
                {
                    Tags = new List<Tag>(_tags);

                    applyLocalChanges();
                };

                navigateToEditScreen(_user, tag, _notes, callBackAction);
            });
        }

        private List<Tag> _tags;
        public List<Tag> Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteTagCommand { get; set; }
        public ICommand CreateTagCommand { get; set; }
        public ICommand EditTagCommand { get; set; }
    }
}
