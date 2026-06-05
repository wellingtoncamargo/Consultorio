using System;
using System.Security.Claims;
using NUnit.Framework;

using Consultorio.Application.Services;

namespace Consultorio.Tests.Unit.Services
{
    [TestFixture]
    public class TokenServiceTests
    {
        private TokenService _tokenService;
        private const string Secret = "supersecretkey_supersecretkey_123";

        [SetUp]
        public void Setup()
        {
            _tokenService = new TokenService(Secret, 60);
        }

       [Test]
        public void GenerateAccessToken_And_Validate_Should_Work()
        {
            var userId = Guid.NewGuid();
            var token = _tokenService.GenerateAccessToken(userId, "u@t.com", "Admin");
            Assert.That(string.IsNullOrWhiteSpace(token), Is.False);

           var principal = _tokenService.ValidateToken(token);
            Assert.That(principal, Is.Not.Null);
            Assert.That("u@t.com", Is.EqualTo(principal?.FindFirst(ClaimTypes.Email)?.Value));
            Assert.That("Admin", Is.EqualTo(principal?.FindFirst(ClaimTypes.Role)?.Value));
        }

       [Test]
        public void GenerateRefreshToken_ShouldReturn_NonEmpty()
        {
            var rt = _tokenService.GenerateRefreshToken();
            Assert.That(string.IsNullOrWhiteSpace(rt), Is.False);
        }
    }
}
