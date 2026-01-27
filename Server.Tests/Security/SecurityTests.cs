using Xunit;
using Server.DTOs;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Server.Tests.Security
{
    public class SecurityTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SecurityTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task NoAuthenticationBypass_WithoutToken_AccessDenied()
        {
            // Act
            var response = await _client.GetAsync("/api/people");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task NoAuthenticationBypass_WithInvalidToken_AccessDenied()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer invalid.token.here");

            // Act
            var response = await _client.GetAsync("/api/people");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SqlInjection_NoVulnerabilityInLogin()
        {
            // Arrange - Try SQL injection payload
            var loginDto = new LoginDTO 
            { 
                Username = "' OR '1'='1", 
                Password = "' OR '1'='1" 
            };
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task NoSensitiveDataInLoginResponse()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "usuario1", Password = "senha123" };
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.DoesNotContain("senha", responseContent, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("password", responseContent, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task NoPasswordInErrorMessages_InvalidCredentials()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "invaliduser", Password = "wrongpass" };
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.DoesNotContain("senha", responseContent, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("usuario", responseContent, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InputValidation_EmptyLoginRequest_Rejected()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "", Password = "" };
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
