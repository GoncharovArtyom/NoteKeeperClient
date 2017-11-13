using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteKeeperClient.ViewModels
{
    class SharedNoteScreenViewModel:BaseViewModel
    {
        private SharedNote _sharedNote;
        public SharedNoteScreenViewModel(IRestClient client, SharedNote sharedNote) : base(client)
        {
            _sharedNote = sharedNote;

            OwnerName = _sharedNote.Owner.Name;
            OwnerEmail = _sharedNote.Owner.Email;
            CreationDate = _sharedNote.CreationDate;
            LastUpdateDate = _sharedNote.LastUpdateDate;
            Heading = _sharedNote.Heading;
            Text = _sharedNote.Text;
        }

        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string Heading { get; set; }
        public string Text { get; set; }
    }
}
