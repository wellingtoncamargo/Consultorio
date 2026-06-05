using System;
using NUnit.Framework;

using Consultorio.Application.Services;

namespace Consultorio.Tests.Unit.Services
{
    [TestFixture]
    public class PasswordHasherServiceTests
    {
        private PasswordHasherService _hasher;

       [SetUp]
        public void Setup() => _hasher = new PasswordHasherService();

       [Test]
        public void Hash_Then_Verify_Should_Return_True_For_Correct_Password()
        {
            var pwd = "P@ssw0rd!";
            var hashed = _hasher.HashPassword(pwd);
            Assert.That(_hasher.Verify(hashed, pwd), Is.True);
        }

       [Test]
        public void Verify_Should_Return_False_For_Wrong_Password()
        {
            var hashed = _hasher.HashPassword("abc123");
            Assert.That(_hasher.Verify(hashed, "wrong"), Is.False);
        }
    }
}
