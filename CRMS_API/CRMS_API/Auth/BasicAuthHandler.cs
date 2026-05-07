using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using CRMS_API.Data;
using CRMS_API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CRMS_API.Auth
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppDbContext _context;

        public BasicAuthHandler(AppDbContext context, IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the Authorization header exists
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization header");

            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();

                // Basic auth header looks like: "Basic dXNlcm5hbWU6cGFzc3dvcmQ="
                var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));

                // Decoded looks like "username:password"
                var parts = decoded.Split(':', 2);
                var username = parts[0];
                var password = parts[1];

                // Look up the user in the database
                var user = _context.Users.SingleOrDefault(u => u.Username == username);

                if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
                    return AuthenticateResult.Fail("Invalid username or password");

                // Create claims — these carry the user's identity and role through the app
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization header");
            }
        }
    }
}