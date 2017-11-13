using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using NoteKeeperClient.ViewModels;

namespace NoteKeeperClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditNoteScreen : TabbedPage
    {
        public EditNoteScreen (IRestClient client, User user, Note note, List<Tag> tags, Action applyLocalChanges)
        {
            InitializeComponent();
            BindingContext = new EditNoteScreenViewModel(client, user, note, tags, applyLocalChanges);

            Tags.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
        }
    }
}