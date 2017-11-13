using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using System.Windows.Input;
using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NoteKeeperClient.ViewModels
{
    class EditNoteScreenViewModel : BaseViewModel
    {
        public class BindableTag : INotifyPropertyChanged
        {
            private Tag _tag;
            public Tag Tag
            {
                get
                {
                    return _tag;
                }
                set
                {
                    _tag = value;
                    OnPropertyChanged();
                }
            }

            private bool _isSelected;
            public bool IsSelected
            {
                get
                {
                    return _isSelected;
                }
                set
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }

            private void OnPropertyChanged([CallerMemberName]string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        User _user;
        Note _note;
        List<Tag> _tags;
        Action _applyLocalChanges;

        public EditNoteScreenViewModel(IRestClient client, User user, Note note, List<Tag> tags, Action applyLocalChanges) : base(client)
        {
            _user = user;
            _note = note;
            _tags = new List<Tag>();
            for(int i=1; i<tags.Count()-1; ++i)
            {
                _tags.Add(tags[i]);
            }
            _applyLocalChanges = applyLocalChanges;
            NewHeading = _note.Heading;
            NewText = _note.Text;

            var listTags = new List<BindableTag>(_tags.Count());
            foreach(var tag in _tags)
            {
                var bindableTag = new BindableTag() { Tag = tag };
                if (_note.TagNames.ToList().FindIndex((string item)=>{ return item == tag.Name; }) != -1)
                {
                    bindableTag.IsSelected = true;
                }
                else
                {
                    bindableTag.IsSelected = false;
                }

                listTags.Add(bindableTag);
            }
            Tags = listTags;

            SaveNoteCommand = new Command(async () =>
            {
                if (NewHeading == _note.Heading && NewText == note.Text)
                {
                    return;
                }
                if (NewHeading == "" || NewText == "")
                {
                    Error = "Поля не могут быть пустыми";
                    return;
                }

                IsBusy = true;
                try
                {
                    if (NewHeading != _note.Heading)
                    {
                        await _restClient.ChangeNoteHeadingAsync(_note, NewHeading);
                        _note.Heading = NewHeading;
                    }
                    if (NewText != _note.Text)
                    {
                        await _restClient.ChangeNoteTextAsync(_note, NewText);
                        _note.Text = NewText;
                    }

                    _note.LastUpdateDate = DateTime.Now;
                    OnPropertyChanged("SelectedNote");
                    _applyLocalChanges();
                    IsBusy = false;
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;
                    IsBusy = false;
                }
            });
            AddPartnerCommand = new Command(async () =>
            {
                var partnerEmail = NewPartnerName;
                
                if(partnerEmail == "")
                {
                    Error = "Поле Email не может быть пустым";
                    return;
                }

                IsBusy = true;
                try
                {
                    var partner = await _restClient.GetUserByEmailAsync(partnerEmail);
                    if (partner == null)
                    {
                        Error = "Такого пользователя не существует";
                        IsBusy = false;
                        return;
                    }

                    await _restClient.AddAccessToUser(_note, partner);

                    (_note.Partners as List<User>).Add(partner);
                    _note.Partners = new List<User>(_note.Partners);
                    OnPropertyChanged("SelectedNote");
                    IsBusy = false;
                }
                catch(RestClientException ex)
                {
                    Error = ex.Message;
                    IsBusy = false;
                }
            });
            RemovePartnerCommand = new Command(async (object parameter) =>
            {
                var partner = parameter as User;

                IsBusy = true;
                try
                {
                    await _restClient.RemoveAccessFromUser(_note, partner);

                    (_note.Partners as List<User>).Remove(partner);
                    _note.Partners = new List<User>(_note.Partners);
                    OnPropertyChanged("SelectedNote");
                    IsBusy = false;
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;
                    IsBusy = false;
                }
            });
            RemoveTagCommand = new Command(async (object parameter) =>
            {
                var bindableTag = parameter as BindableTag;
                if (!bindableTag.IsSelected)
                {
                    return;
                }

                IsBusy = true;
                try
                {
                    await _restClient.RemoveTagFromNoteAsync(_note, bindableTag.Tag);

                    bindableTag.IsSelected = false;
                    (_note.TagNames as List<string>).Remove(bindableTag.Tag.Name);
                    _applyLocalChanges();
                    IsBusy = false;
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;
                    IsBusy = false;
                }
            });
            AddTagCommand = new Command(async (object parameter) =>
            {
                var bindableTag = parameter as BindableTag;
                if (bindableTag.IsSelected)
                {
                    return;
                }

                IsBusy = true;
                try
                {
                    await _restClient.AddTagToNoteAsync(_note, bindableTag.Tag);

                    bindableTag.IsSelected = true;
                    (_note.TagNames as List<string>).Add(bindableTag.Tag.Name);
                    _applyLocalChanges();
                    IsBusy = false;
                }
                catch (RestClientException ex)
                {
                    Error = ex.Message;
                    IsBusy = false;
                }
            });

        }

        public User Owner
        {
            get
            {
                return _user;
            }
        }
        public Note SelectedNote
        {
            get
            {
                return _note;
            }
        }
        public string NewHeading { get; set; }
        public string NewText { get; set; }
        public List<BindableTag> Tags { get; set; }
        public string NewPartnerName { get; set; }

        public ICommand AddPartnerCommand { get; set; }
        public ICommand SaveNoteCommand { get; set; }
        public ICommand AddTagCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand RemovePartnerCommand { get; set; }
    }
}
