using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NoteKeeperClient.ViewModels
{
    class MainScreenViewModel : BaseViewModel
    {
        class AllNotesTag : Tag
        {
            public AllNotesTag()
            {
                Name = "Все заметки";
            }
        }
        class SharedNotesTag : Tag
        {
            public SharedNotesTag()
            {
                Name = "Чужие заметки";
            }
        }

        List<Note> _notes;
        List<Tag> _tags;
        User _user;
        bool _isSortByDate;

        private void SortByDate()
        {
            if (_isSortByDate)
            {
                return;
            }

            IsBusy = true;

            if (!AreSharedNotesShown)
            {
                var tmpList = new List<Note>(NotesToShow);
                tmpList.Sort(delegate (Note a, Note b)
                {
                    return (-1) * a.LastUpdateDate.CompareTo(b.LastUpdateDate);
                });
                _isSortByDate = true;

                NotesToShow = new List<Note>(tmpList);
            }
            else
            {
                var tmpList = new List<SharedNote>(SharedNotes);
                tmpList.Sort(delegate (SharedNote a, SharedNote b)
                {
                    return (-1) * a.LastUpdateDate.CompareTo(b.LastUpdateDate);
                });
                _isSortByDate = true;

                SharedNotes = new List<SharedNote>(tmpList);
            }

            IsBusy = false;
        }
        private void SortByHeading()
        {
            if (!_isSortByDate)
            {
                return;
            }

            IsBusy = true;

            if (!AreSharedNotesShown)
            {
                var tmpList = new List<Note>(NotesToShow);
                tmpList.Sort(delegate (Note a, Note b)
                {
                    return a.Heading.CompareTo(b.Heading);
                });
                _isSortByDate = false;

                NotesToShow = new List<Note>(tmpList);
            }
            else
            {
                var tmpList = new List<SharedNote>(SharedNotes);
                tmpList.Sort(delegate (SharedNote a, SharedNote b)
                {
                    return a.Heading.CompareTo(b.Heading);
                });
                _isSortByDate = false;

                SharedNotes = new List<SharedNote>(tmpList);
            }

            IsBusy = false;
        }
        private async void InitializeData()
        {
            try
            {
                IsBusy = true;
                IsRefreshing = true;

                _notes = await _restClient.GetNotesByOwnerAsync(_user);
                _tags = await _restClient.GetTagsByOwnerAsync(_user);
                SharedNotes = await _restClient.GetSharedNotesAsync(_user);

                _notesToShow = _notes;

                _isSortByDate = true;
                _tags.Insert(0, new AllNotesTag());
                _tags.Add(new SharedNotesTag());
                SelectedTag = _tags[0];

                TagsToShow = _tags;

                IsBusy = false;
                IsRefreshing = false;
            }
            catch (RestClientException ex)
            {
                Error = ex.Message;

                IsBusy = false;
                IsRefreshing = false;
            }
        }
        private void ApplyLocalChanges()
        {
            TagsToShow = new List<Tag>(_tags);
            if (SelectedTag.GetType() == typeof(AllNotesTag))
            {
                var tempList = new List<Note>(_notes);

                if (_isSortByDate)
                {
                    tempList.Sort(delegate (Note a, Note b)
                    {
                        return (-1) * a.LastUpdateDate.CompareTo(b.LastUpdateDate);
                    });
                }
                else
                {
                    tempList.Sort(delegate (Note a, Note b)
                    {
                        return a.Heading.CompareTo(b.Heading);
                    });
                }

                NotesToShow = tempList;
                AreSharedNotesShown = false;
            }
            else if (SelectedTag.GetType() == typeof(SharedNotesTag))
            {
                var tempList = new List<SharedNote>(_sharedNotes);
                if (_isSortByDate)
                {
                    tempList.Sort(delegate (SharedNote a, SharedNote b)
                    {
                        return (-1) * a.LastUpdateDate.CompareTo(b.LastUpdateDate);
                    });
                }
                else
                {
                    tempList.Sort(delegate (SharedNote a, SharedNote b)
                    {
                        return a.Heading.CompareTo(b.Heading);
                    });
                }
                SharedNotes = tempList;
                AreSharedNotesShown = true;
            }
            else
            {
                var filterFunc = new Func<Note, bool>((Note note) =>
                {
                    foreach (var tagName in note.TagNames)
                    {
                        if (tagName == SelectedTag.Name)
                        {
                            return true;
                        }
                    }
                    return false;
                });
                var tempList = _notes.Where<Note>(filterFunc).Select(item => item).ToList();

                if (_isSortByDate)
                {
                    tempList.Sort(delegate (Note a, Note b)
                    {
                        return (-1) * a.LastUpdateDate.CompareTo(b.LastUpdateDate);
                    });
                }
                else
                {
                    tempList.Sort(delegate (Note a, Note b)
                    {
                        return a.Heading.CompareTo(b.Heading);
                    });
                }

                NotesToShow = tempList;
                AreSharedNotesShown = false;
            }
        }

        public MainScreenViewModel(IRestClient client, User user, Action logOut, Action<User, Note, List<Tag>, Action> editNote, Action<User, Tag, List<Note>, Action> createNote, Action<IRestClient, User, List<Tag>, List<Note>, Action> editTags, Action<SharedNote> exploreSharedNote) : base(client)
        {
            _user = user;
            _areSharedNotesShown = false;
            _isSortByDate = true;
            SortByDateCommand = new Command(SortByDate);
            SortByHeadingCommand = new Command(SortByHeading);
            LogOutCommand = new Command(logOut);
            EditNoteCommand = new Command((object parameter) =>
            {
                var note = parameter as Note;
                editNote(_user, note, _tags, ApplyLocalChanges);
            });
            CreateNoteCommand = new Command(() =>
            {
                Tag tag = null;
                if (_selectedTag.GetType() != typeof(AllNotesTag) && _selectedTag.GetType() != typeof(SharedNotesTag))
                {
                    tag = _selectedTag;
                }
                createNote(_user, tag, _notes, ApplyLocalChanges);
            });
            EditTagsCommand = new Command(() =>
            {
                editTags(_restClient, _user, _tags, _notes, ApplyLocalChanges);
            });
            DeleteNoteCommand = new Command(async (object parameter) =>
            {
                IsBusy = true;
                var note = parameter as Note;
                try
                {
                    await _restClient.DeleteNoteAsync(note);
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;
                    return;
                }

                _notesToShow.Remove(note);
                NotesToShow = new List<Note>(_notesToShow);
                IsBusy = false;
            });
            ExploreSharedNoteCommand = new Command((object parameter) =>
            {
                var sharedNote = parameter as SharedNote;
                exploreSharedNote(sharedNote);
            });
            DeleteSharedNoteCommand = new Command(async (object parameter) =>
            {
                IsBusy = true;
                try
                {
                    var sharedNote = parameter as SharedNote;
                    await _restClient.DeleteSharedNoteAsync(sharedNote, _user);
                    _sharedNotes.Remove(sharedNote);
                    SharedNotes = new List<SharedNote>(_sharedNotes);
                    IsBusy = false;
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;
                    IsBusy = false;
                }
            });
            RefreshCommand = new Command(async () =>
            {
                IsBusy = true;
                IsRefreshing = true;

                try
                {
                    _notes = await _restClient.GetNotesByOwnerAsync(_user);

                    _tags = await _restClient.GetTagsByOwnerAsync(_user);
                    _tags.Insert(0, new AllNotesTag());
                    _tags.Add(new SharedNotesTag());
                    SelectedTag = _tags[0];

                    SharedNotes = await _restClient.GetSharedNotesAsync(_user);

                    TagsToShow = _tags;
                    SelectedTag = _selectedTag;

                    IsBusy = false;
                    IsRefreshing = false;
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;

                    IsBusy = false;
                    IsRefreshing = false;
                }
            });

            InitializeData();
        }

        public string Name { get { return _user.Name; } }
        public string Email { get { return _user.Email; } }

        private List<Tag> _tagsToShow;
        public List<Tag> TagsToShow
        {
            get
            {
                return _tagsToShow;
            }
            set
            {
                _tagsToShow = value;
                OnPropertyChanged();
            }
        }

        private List<Note> _notesToShow;
        public List<Note> NotesToShow
        {
            get
            {
                return _notesToShow;
            }
            set
            {
                _notesToShow = value;
                OnPropertyChanged();
            }
        }

        private List<SharedNote> _sharedNotes;
        public List<SharedNote> SharedNotes
        {
            get
            {
                return _sharedNotes;
            }
            set
            {
                _sharedNotes = value;
                OnPropertyChanged();
            }
        }

        private Tag _selectedTag;
        public Tag SelectedTag
        {
            get
            {
                return _selectedTag;

            }
            set
            {
                if (value.GetType() == typeof(AllNotesTag))
                {
                    var tempList = _notes;

                    if (_isSortByDate)
                    {
                        tempList.Sort(delegate (Note a, Note b)
                        {
                            return (-1) * a.LastUpdateDate.CompareTo(b.LastUpdateDate);
                        });
                    }
                    else
                    {
                        tempList.Sort(delegate (Note a, Note b)
                        {
                            return a.Heading.CompareTo(b.Heading);
                        });
                    }

                    NotesToShow = tempList;
                    AreSharedNotesShown = false;
                }
                else if (value.GetType() == typeof(SharedNotesTag))
                {
                    var tempList = _sharedNotes;
                    if (_isSortByDate)
                    {
                        tempList.Sort(delegate (SharedNote a, SharedNote b)
                        {
                            return (-1) * a.LastUpdateDate.CompareTo(b.LastUpdateDate);
                        });
                    }
                    else
                    {
                        tempList.Sort(delegate (SharedNote a, SharedNote b)
                        {
                            return a.Heading.CompareTo(b.Heading);
                        });
                    }
                    SharedNotes = tempList;
                    AreSharedNotesShown = true;
                }
                else
                {
                    var filterFunc = new Func<Note, bool>((Note note) =>
                    {
                        foreach (var tagName in note.TagNames)
                        {
                            if (tagName == value.Name)
                            {
                                return true;
                            }
                        }
                        return false;
                    });
                    var tempList = _notes.Where<Note>(filterFunc).Select(item => item).ToList();

                    if (_isSortByDate)
                    {
                        tempList.Sort(delegate (Note a, Note b)
                        {
                            return (-1) * a.LastUpdateDate.CompareTo(b.LastUpdateDate);
                        });
                    }
                    else
                    {
                        tempList.Sort(delegate (Note a, Note b)
                        {
                            return a.Heading.CompareTo(b.Heading);
                        });
                    }

                    NotesToShow = tempList;
                    AreSharedNotesShown = false;
                }

                _selectedTag = value;
            }
        }
        public Note SelectedNote
        {
            get
            {
                return null;
            }
            set
            {
                if (value != null)
                {
                    EditNoteCommand.Execute(value);
                    OnPropertyChanged();
                }
            }
        }
        public SharedNote SelectedSharedNote
        {
            get
            {
                return null;
            }
            set
            {
                if (value != null)
                {
                    ExploreSharedNoteCommand.Execute(value);
                    OnPropertyChanged();
                }
            }
        }

        private bool _areSharedNotesShown;
        public bool AreSharedNotesShown
        {
            get
            {
                return _areSharedNotesShown;
            }
            set
            {
                _areSharedNotesShown = value;
                OnPropertyChanged();
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand SortByDateCommand { get; set; }
        public ICommand SortByHeadingCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public ICommand EditNoteCommand { get; set; }
        public ICommand CreateNoteCommand { get; set; }
        public ICommand EditTagsCommand { get; set; }
        public ICommand DeleteNoteCommand { get; set; }
        public ICommand ExploreSharedNoteCommand { get; set; }
        public ICommand DeleteSharedNoteCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

    }
}
