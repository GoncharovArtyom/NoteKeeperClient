using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteKeeper.Model;

namespace NoteKeeperClient.RestClient
{
    public interface IRestClient
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User newUser);
        Task<List<Note>> GetNotesByOwnerAsync(User owner);
        Task<List<Tag>> GetTagsByOwnerAsync(User owner);
        Task DeleteNoteAsync(Note note);
        Task<List<SharedNote>> GetSharedNotesAsync(User user);
        Task DeleteSharedNoteAsync(SharedNote note, User user);
        Task DeleteTagAsync(Tag tag);
        Task<Tag> CreateTagAsync(Tag tag);
        Task ChangeTagNameAsync(Tag tag, string newName);
        Task<Note> CreateNoteAsync(Note note);
        Task AddTagToNoteAsync(Note note, Tag tag);
        Task RemoveTagFromNoteAsync(Note note, Tag tag);
        Task ChangeNoteHeadingAsync(Note note, string newHeading);
        Task ChangeNoteTextAsync(Note note, string newText);
        Task AddAccessToUser(Note note, User user);
        Task RemoveAccessFromUser(Note note, User user);
    }
}
