using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using NoteKeeperClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NoteKeeperClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TagsScreen : ContentPage
    {
        public TagsScreen(IRestClient client, User user, List<Tag> tags, List<Note> notes, Action applyLocalChanges)
        {
            InitializeComponent();
            BindingContext = new TagsScreenViewModel(client, user, tags, notes, applyLocalChanges,
                async (User _user, Action<Tag> _applyLocalChanges) =>
                {
                    await Navigation.PushModalAsync(new CreateTagScreen(client, _user, _applyLocalChanges));
                },
                async (User _user, Tag tag, List<Note> _notes, Action _applyLocalChanges) =>
                {
                    await Navigation.PushModalAsync(new EditTagScreen(client, _user, tag, _notes, _applyLocalChanges));
                });

            Tags.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
        }
    }
}