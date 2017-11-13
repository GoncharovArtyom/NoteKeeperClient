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
	public partial class EditTagScreen : ContentPage
	{
        public EditTagScreen(IRestClient client, User user, Tag tag, List<Note> notes, Action applyLocalChanges)
        {
            InitializeComponent();
            BindingContext = new EditTagScreenViewModel(client, user, tag, notes, applyLocalChanges, async () =>
            {
                await Navigation.PopModalAsync();
            });
        }
    }
}