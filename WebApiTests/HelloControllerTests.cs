using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using WebApiProject;


namespace WebApiTests;

public class HelloControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HelloControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Test1()
    {
        // Arrange
        var request = new HttpRequestMessage(new HttpMethod("GET"), "/hello");

        // Act
        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Be("Hello, World!");
    }
}
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Usuń oryginalny schemat autentykacji
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AuthenticationHandler<AuthenticationSchemeOptions>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Dodaj TestAuthHandler jako schemat autentykacji
            services.AddAuthentication("TestAuthentication")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuthentication", options => { });
        });
    }
}


public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, "user"),
                    new Claim(ClaimTypes.Name, "user"),
                };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);

    }
}