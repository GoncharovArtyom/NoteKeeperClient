using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NoteKeeper.Model;
using NoteKeeperClient.ViewModels;
using NoteKeeperClient.RestClient;

namespace NoteKeeperClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateNoteScreen : ContentPage
    {
        public CreateNoteScreen(IRestClient client, User user, Tag selectedTag, List<Note> notes, Action applyLocalChanges)
        {
            InitializeComponent();
            BindingContext = new CreateNoteScreenViewModel(client, user, selectedTag, notes, async () =>
            {
                await Navigation.PopModalAsync();
            }, applyLocalChanges);

        }
    }
}