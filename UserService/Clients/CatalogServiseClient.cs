using System;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace UserService.Clients
{
    public class CatalogServiceClient
    {
        private readonly HttpClient _client;

        public CatalogServiceClient()
        {
            _client = new HttpClient();
        }

        


        public async Task<int> CreateUserRequest(CreateUserRequest request)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://userservice-uat.azurewebsites.net/Register/RegisterNewUser"),
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
            string responseBody = await response.Content.ReadAsStringAsync();
            int userId = JsonConvert.DeserializeObject<int>(responseBody);

            return userId;
        }

        public async Task<bool> GetUserStatus(int userId)
        {
            string url = $"https://userservice-uat.azurewebsites.net/UserManagement/GetUserStatus?userId={userId}";
            HttpResponseMessage response = await _client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            bool userStatus = JsonConvert.DeserializeObject<bool>(responseBody);

            return userStatus;

        }

        public async Task<bool> SetUserStatus(int userId, bool newStatus)
        {
            string url = $"https://userservice-uat.azurewebsites.net/UserManagement/SetUserStatus?userId={userId}&newStatus={newStatus}";
            HttpResponseMessage response = await _client.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }



        public async Task<HttpStatusCode> DeleteUser(int userId)
        {
            string url = $"https://userservice-uat.azurewebsites.net/Register/DeleteUser?userId={userId}";
            HttpResponseMessage response = await _client.DeleteAsync(url);

            return response.StatusCode;
        }


        public async Task<HttpStatusCode> GetUserStatusCode(int userId)
        {
            string url = $"https://userservice-uat.azurewebsites.net/UserManagement/GetUserStatusCode?userId={userId}";
            HttpResponseMessage response = await _client.GetAsync(url);

            return response.StatusCode;
        }
    }
}
