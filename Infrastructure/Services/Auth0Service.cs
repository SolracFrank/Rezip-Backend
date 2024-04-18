using Microsoft.Extensions.Options;

namespace Infrastructure.Services;
using Domain.Custom;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


public class Auth0Service : IAuth0Service
{
    public Auth0Service(IOptions<Auth0Config> auth0Config, ILogger<Auth0Service> logger, IHttpClientFactory clientFactory)
    {
        _auth0Config = auth0Config.Value;
        _logger = logger;
        _clientFactory = clientFactory;
    }

    private readonly Auth0Config _auth0Config;
    private readonly ILogger<Auth0Service> _logger;
    private readonly IHttpClientFactory _clientFactory;


    public async Task<string> GetAuth0Token()
    {
        _logger.LogInformation("Sending request for getting Auth0 Token");
        var client = _clientFactory.CreateClient();
        var tokenUrl = $"{_auth0Config.Authority}oauth/token";
        var requestBody = new
        {
            client_id = _auth0Config.ClientId,
            client_secret = _auth0Config.ClientSecrets,
            audience = _auth0Config.Audience,
            grant_type = _auth0Config.GrantType,
            scope = "update:users delete:users"
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(tokenUrl, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error requesting Auth0 Token");
            _logger.LogError($"Request to Auth0 failed with Content {response.Content}");


            throw new HttpRequestException($"Request to Auth0 failed with status code {response.StatusCode}");
        }
        _logger.LogInformation("response successfully got");

        _logger.LogInformation("Reading response for getting auth0 Token");

        var responseContent = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(responseContent);
        var accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogError("Auth0 Token not found");

            throw new InvalidOperationException("Auth0 Token not found");
        }
        _logger.LogInformation("Auth0 Token found");
        _logger.LogInformation($"token: {accessToken}");
        return accessToken;
    }
    public async Task<Result<string>> GiveUserPermissions(string userId)
    {
        try
        {
            var encodedUserId = Uri.EscapeDataString(userId);
            _logger.LogInformation($"Request to change permissions for {userId}");

            var accessToken = await GetAuth0Token();
            var client = _clientFactory.CreateClient();
            var permissionUrl = $"{_auth0Config.Authority}api/v2/users/{encodedUserId}/permissions";

            var permissionsData = new
            {
                permissions = new[]
                {
                        new { resource_server_identifier = _auth0Config.ApiIdentifier, permission_name = "read:active" }
                    }
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(permissionsData), Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Trim());
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };

            var response = await client.PostAsync(permissionUrl, requestContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User permissions has been given");
                return new Result<string>("User permission has been added");
            }

            _logger.LogInformation($"Response Code: {response.StatusCode}");
            _logger.LogInformation($"Response Body: {responseBody}");

            return new Result<string>(new HttpRequestException("Error adding permissions to user"));

        }
        catch (Exception ex)
        {
            string error = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Error desconocido";

            _logger.LogInformation($"{error}");

            return new Result<string>(new BadRequestException(ex.Message));
        }
    }
    public async Task<Result<string>> DeleteUser(string userId)
    {
        try
        {
            var encodedUserId = Uri.EscapeDataString(userId);
            _logger.LogInformation($"Request to delete {userId}");

            var accessToken = await GetAuth0Token();
            var client = _clientFactory.CreateClient();
            var deleteUrl = $"{_auth0Config.Authority}api/v2/users/{encodedUserId}";


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Trim());

            _logger.LogInformation($"{client.DefaultRequestHeaders}");


            var response = await client.DeleteAsync(deleteUrl);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User has been deleted");
                return new Result<string>("User has been deleted");
            }

            _logger.LogInformation($"Response: {response}");
            _logger.LogInformation($"Response Code: {response.StatusCode}");
            _logger.LogInformation($"Response Body: {responseBody}");

            return new Result<string>(new HttpRequestException("Error deleting user"));

        }
        catch (Exception ex)
        {
            string error = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Error desconocido";

            _logger.LogInformation($"{error}");

            return new Result<string>(new BadRequestException(ex.Message));
        }
    }
}

