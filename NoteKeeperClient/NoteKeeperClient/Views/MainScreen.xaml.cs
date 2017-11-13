using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using NoteKeeperClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NoteKeeperClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainScreen : MasterDetailPage
    {
        public MainScreen(IRestClient client, User user)
        {
            InitializeComponent();

            BindingContext = new MainScreenViewModel(client, user,
                async () =>
                {
                    var page = new LoginScreen(client);
                    App.Current.MainPage = page;
                },
                async (User _user, Note note, List<Tag> tags, Action applyLocalChanges) =>
                {
                    await Detail.Navigation.PushModalAsync(new EditNoteScreen(client, _user, note, tags, applyLocalChanges));
                },
                async (User _user, Tag selectedTag, List<Note> notes, Action applyLocalChanges) =>
                {
                    await Detail.Navigation.PushModalAsync(new CreateNoteScreen(client, _user, selectedTag, notes, applyLocalChanges));
                },
                async (IRestClient _client, User _user, List<Tag> tags, List<Note> notes, Action applyLocalChanges) =>
                {

                    await Detail.Navigation.PushModalAsync(new NavigationPage(new TagsScreen(_client, _user, tags, notes, applyLocalChanges)));

                },
                async (SharedNote sharedNote) =>
                {
                    await Detail.Navigation.PushModalAsync(new SharedNoteScreen(client, sharedNote));
                });

        }
    }
}