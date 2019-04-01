using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace BurnForMoney.Functions.UnitTests.Serialization
{
    public class ExceptionsSerializationTests
    {
        private readonly ISpecimenContext _fixtureContext = new SpecimenContext(
            new Fixture().Customize(new AutoNSubstituteCustomization())
        );

        public static IEnumerable<object[]> ExceptionTypes =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.FullName.StartsWith("BurnForMoney"))
                .Where(type => typeof(Exception).IsAssignableFrom(type))
                .Select(type => new object[] {type});

        [Theory]
        [MemberData(nameof(ExceptionTypes))]
        public void Assert_AssemblyExceptions_CanBeSerializedAndDeserialized(Type exceptionType)
        {
            object exception = _fixtureContext.Resolve(exceptionType);

            string serialized = JsonConvert.SerializeObject(exception);
            serialized.Should().NotBeNullOrEmpty();

            object deserialized = JsonConvert.DeserializeObject(serialized, exceptionType);
            deserialized.Should().NotBeNull();
        }
    }
}