using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteKeeper.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;

namespace NoteKeeperClient.RestClient
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _client;

        public RestClient(string connectionString)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(connectionString);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task AddAccessToUser(Note note, User user)
        {
            HttpResponseMessage response;
            string responseString;
            try
            {
                var uri = String.Format("notes/{0}/shared-to-users/{1}", note.Id, user.Id);

                response = await _client.PostAsync(uri, null);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task AddTagToNoteAsync(Note note, Tag tag)
        {
            HttpResponseMessage response;
            string responseString;
            try
            {
                var uri = String.Format("notes/{0}/tags/{1}", note.Id, tag.Id);

                response = await _client.PostAsync(uri, null);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task ChangeNoteHeadingAsync(Note note, string newHeading)
        {
            HttpResponseMessage response;
            string responseString;
            try
            {
                var uri = String.Format("notes/{0}/heading", note.Id);
                var content = new StringContent(String.Format("\"{0}\"", newHeading), Encoding.UTF8, "application/json");
                response = await _client.PutAsync(uri, content);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task ChangeNoteTextAsync(Note note, string newText)
        {
            HttpResponseMessage response;
            string responseString;
            try
            {
                var uri = String.Format("notes/{0}/text", note.Id);
                var content = new StringContent(String.Format("\"{0}\"", newText), Encoding.UTF8, "application/json");
                response = await _client.PutAsync(uri, content);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task ChangeTagNameAsync(Tag tag, string newName)
        {
            HttpResponseMessage response;
            string responseString;
            try
            {
                var uri = String.Format("tags/{0}/name", tag.Id);
                var content = new StringContent(String.Format("\"{0}\"", newName), Encoding.UTF8, "application/json");
                response = await _client.PutAsync(uri, content);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task<Note> CreateNoteAsync(Note note)
        {
            var noteJson = JsonConvert.SerializeObject(note);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.PostAsync("notes", new StringContent(noteJson, Encoding.UTF8, "application/json"));
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }

            try
            {
                var createdNote = JsonConvert.DeserializeObject<Note>(responseString);
                return createdNote;
            }
            catch (JsonException ex)
            {
                throw new RestClientException("Неизвестная проблема", ex);
            }
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            var tagJson = JsonConvert.SerializeObject(tag);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.PostAsync("tags", new StringContent(tagJson, Encoding.UTF8, "application/json"));
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }

            try
            {
                var createdTag = JsonConvert.DeserializeObject<Tag>(responseString);
                return createdTag;
            }
            catch (JsonException ex)
            {
                throw new RestClientException("Неизвестная проблема", ex);
            }
        }

        public async Task<User> CreateUserAsync(User newUser)
        {
            var userJson = JsonConvert.SerializeObject(newUser);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.PostAsync("users", new StringContent(userJson, Encoding.UTF8, "application/json"));
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }

            try
            {
                var createdUser = JsonConvert.DeserializeObject<User>(responseString);
                return createdUser;
            }
            catch (JsonException ex)
            {
                throw new RestClientException("Неизвестная проблема", ex);
            }
        }

        public async Task DeleteNoteAsync(Note note)
        {
            var uri = String.Format("notes/{0}", note.Id);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.DeleteAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task DeleteSharedNoteAsync(SharedNote note, User user)
        {
            var uri = String.Format("users/{0}/shared-notes/{1}", user.Id, note.Id);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.DeleteAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task DeleteTagAsync(Tag tag)
        {
            var uri = String.Format("tags/{0}", tag.Id);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.DeleteAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task<List<Note>> GetNotesByOwnerAsync(User owner)
        {
            var uri = String.Format("users/{0}/notes", owner.Id);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.GetAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }

            try
            {
                var notes = JsonConvert.DeserializeObject<List<Note>>(responseString);
                return notes;
            }
            catch (JsonException ex)
            {
                throw new RestClientException("Неизвестная проблема", ex);
            }
        }

        public async Task<List<SharedNote>> GetSharedNotesAsync(User user)
        {
            var uri = String.Format("users/{0}/shared-notes", user.Id);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.GetAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }

            try
            {
                var sharedNotes = JsonConvert.DeserializeObject<List<SharedNote>>(responseString);
                return sharedNotes;
            }
            catch (JsonException ex)
            {
                throw new RestClientException("Неизвестная проблема", ex);
            }
        }

        public async Task<List<Tag>> GetTagsByOwnerAsync(User owner)
        {
            var uri = String.Format("users/{0}/tags", owner.Id);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.GetAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }

            try
            {
                var tags = JsonConvert.DeserializeObject<List<Tag>>(responseString);
                return tags;
            }
            catch (JsonException ex)
            {
                throw new RestClientException("Неизвестная проблема", ex);
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var uri = String.Format("users/have-email?email={0}", email);

            HttpResponseMessage response;
            string responseString;
            try
            {
                response = await _client.GetAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная проблема", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }

            try
            {
                var user = JsonConvert.DeserializeObject<User>(responseString);
                return user;
            }
            catch (JsonException ex)
            {
                throw new RestClientException("Неизвестная проблема", ex);
            }
        }

        public async Task RemoveAccessFromUser(Note note, User user)
        {
            HttpResponseMessage response;
            string responseString;
            try
            {
                var uri = String.Format("notes/{0}/shared-to-users/{1}", note.Id, user.Id);

                response = await _client.DeleteAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }

        public async Task RemoveTagFromNoteAsync(Note note, Tag tag)
        {
            HttpResponseMessage response;
            string responseString;
            try
            {
                var uri = String.Format("notes/{0}/tags/{1}", note.Id, tag.Id);

                response = await _client.DeleteAsync(uri);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new RestClientException("Неизвестная ошибка", ex);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(responseString);
                throw new RestClientException((string)errorObj["Message"]);
            }
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new RestClientException("Проблемы с сервером");
            }
        }
    }
}
