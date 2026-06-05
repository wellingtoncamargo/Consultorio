Unit Test Template (NUnit)

Filename: {ClassName}Tests.cs

using Moq;
using System;
using System.Threading.Tasks;
using NUnit.Framework;

// Domain/Application usings are injected by the generator based on class being tested
// e.g. using Consultorio.Domain.Entities;
//       using Consultorio.Services;
//       using Consultorio.Domain.Repositories;

// Namespace will be: Consultorio.Tests.Unit.{TestFolder}
// {TestFolder} examples: Services, Repositories, Controllers, Application
namespace Consultorio.Tests.Unit.{TestFolder}
{
    [TestFixture]
    public class {ClassName}Tests
    {
        // Mocks (generator replaces placeholders):
        {Mocks}

        // System under test declaration (generator replaces):
        {SystemUnderTestDeclaration}

        [SetUp]
        public void Setup()
        {
            {SetupMocks}
            {SutInitialization}
        }

        [Test]
        public async Task {ScenarioName}()
        {
            // Arrange
            {Arrange}

            // Act
            var resultado = await {SutInstance}.{MethodUnderTest}({MethodArgs});

            // Assert
            {Asserts}
        }
    }
}
