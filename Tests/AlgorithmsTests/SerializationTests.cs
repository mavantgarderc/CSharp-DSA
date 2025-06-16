using System.Text.Json;
using Modules.Algorithms;

namespace Tests
{
    public class SerializationTests
    {
        public class Person
        {
            public string Name { get; set; } = "OusMava";
            public int Age { get; set; } = 22;
            public string? Nickname { get; set; }
        }

        [Fact]
        public void SerializeAndDeserialize_ReturnsEqualObject() {
            var person = new Person { Name = "Jermaine", Age = 40 };
            var json = SerializationUtils.SerializeToJson(person);
            var deserialized = SerializationUtils.DeserializeFromJson<Person>(json);

            Assert.Equal(person.Name, deserialized.Name);
            Assert.Equal(person.Age, deserialized.Age);
        }

        [Fact]
        public void SerializeToBase64_And_DeserializeFromBase64() {
            var person = new Person { Name = "Kendrick", Age = 37 };
            var base64 = SerializationUtils.SerializeToBase64(person);
            var deserialized = SerializationUtils.DeserializeFromBase64<Person>(base64);

            Assert.Equal("Kendrick", deserialized.Name);
            Assert.Equal(37, deserialized.Age);
        }

        [Fact]
        public void SerializeIgnoringNull() {
            var person = new Person { Name = "Aubrey", Age = 38 };
            var json = SerializationUtils.SerializeIgnoringNull(person);

            Assert.DoesNotContain("Nickname", json);
        }

        [Fact]
        public void SerializeWithNamingPolicy_CamelCase() {
            var person = new Person { Name = "Webster", Age = 34 };
            var json = SerializationUtils.SerializeWithNamingPolicy(person, JsonNamingPolicy.CamelCase);

            Assert.Contains("name", json);
            Assert.Contains("age", json);
        }

        [Fact]
        public void TryDeserializeOrDefault_ReturnsFallback() {
            var invalidJson = "{ invalid json }";
            var fallback = new Person { Name = "Fallback", Age = 0 };
            var result = SerializationUtils.TryDeserializeOrDefault(invalidJson, fallback);

            Assert.Equal("Fallback", result?.Name);
        }

        [Fact]
        public void SerializeToAndFromXml() {
            var person = new Person { Name = "Curtis", Age = 49};
            var xml = SerializationUtils.SerializeToXml(person);
            var deserialized = SerializationUtils.DeserializeFromXml<Person>(xml);

            Assert.Equal("Curtis", deserialized?.Name);
        }

        [Fact]
        public void SerializeToAndfromfile() {
            var person = new Person { Name = "Shawn", Age = 55 };
            var path = "temp-test.json";
            SerializationUtils.SerializeToFile(person, path);
            var deserialized = SerializationUtils.DeserializeFromFile<Person>(path);

            File.Delete(path);
            Assert.Equal("Shawn", deserialized?.Name);
        }

        [Fact]
        public void SerializeToAndFromStream() {
            var person = new Person { Name = "Dwayne", Age = 42 };
            using var stream = new MemoryStream();
            SerializationUtils.SerializeToStream(person, stream);
            stream.Position = 0;
            var deserialized = SerializationUtils.DeserializeFromStream<Person>(stream);

            Assert.Equal("Dwayne", deserialized?.Name);
        }

        [Fact]
        public void SerializeToAndFromBytes() {
            var person = new Person { Name = "Ye", Age = 48 };
            var bytes = SerializationUtils.SerializeToBytes(person);
            var deserialized = SerializationUtils.DeserializeFromBytes<Person>(bytes);

            Assert.Equal("Ye", deserialized?.Name);
        }

        [Fact]
        public void ValidateJson() {
            var person = new Person { Name = "Nayvadius", Age = 41 };
            var json = SerializationUtils.SerializeToJson(person);

            Assert.True(SerializationUtils.IsValidJson(json));
            Assert.False(SerializationUtils.IsValidJson("Invalid JSON"));
        }

        [Fact]
        public void DeepClone_CreateNewObject() {
            var person = new Person { Name = "Lelland", Age = 31 };
            var clone = SerializationUtils.DeepClone(person);

            Assert.NotSame(person, clone);
            Assert.Equal("Lelland", clone?.Name);
        }

    }
}
