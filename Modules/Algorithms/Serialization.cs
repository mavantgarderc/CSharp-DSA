using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Modules.Algorithms
{
    /// <summary>
    /// Concepts:
    ///     - JSON, Base64, and XML Serialization
    ///     - Deep Cloning through Serialization
    ///     - Custom Converters and Naming Policies
    ///     - Ignoring Nulls and Fallback Strategies
    /// Key Practices:
    ///     - Cleaner code by centralizing (de)serialization</item>
    ///     - Forward-thinking design (network, files, streams, base64)</item>
    ///     - Robust fallback mechanisms (TryDeserializeOrDefault)</item>
    ///     - Support for diagnostics (SerializeIncludingPrivate)</item>
    /// </summary>

    public static class SerializationUtils
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            WriteIndented = true,
            IncludeFields = true
        };
        private static readonly JsonSerializerOptions IgnoreNullsOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private static readonly JsonSerializerOptions NamingPolicyCamel = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private static readonly Newtonsoft.Json.JsonSerializerSettings NewtonsoftPrivateSettings = new()
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
            {
                SerializeCompilerGeneratedMembers = true
            }
        };


        #region Primary Methods

        public static string SerializeToJson<T>(T data)
        {
            return System.Text.Json.JsonSerializer.Serialize(data, DefaultOptions)
                ?? throw new InvalidOperationException("Null result!!!");

        }

        public static T DeserializeFromJson<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, DefaultOptions)
                ?? throw new InvalidOperationException("Null result!!!");
        }

        public static void SerializeToFile<T>(T data, string filePath)
        {
            File.WriteAllText(filePath, SerializeToJson(data));
        }

        public static T DeserializeFromFile<T>(string filePath)
        {
            return DeserializeFromJson<T>(File.ReadAllText(filePath));
        }

        public static void SerializeToStream<T>(T data, Stream stream)
        {
            System.Text.Json.JsonSerializer.Serialize(stream, data, DefaultOptions);
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(stream, DefaultOptions)!
                ?? throw new InvalidOperationException("Null result!!!");
        }

        #endregion


        #region Advanced Methods

        public static byte[] SerializeToBytes<T>(T data)
        {
            return Encoding.UTF8.GetBytes(SerializeToJson(data));
        }

        public static T DeserializeFromBytes<T>(byte[] bytes)
        {
            return DeserializeFromJson<T>(Encoding.UTF8.GetString(bytes));
        }

        public static string SerializeWithConverters<T>(T data, IEnumerable<JsonConverter> converters)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            foreach (var converter in converters)
            {
                options.Converters.Add(converter);
            }

            return System.Text.Json.JsonSerializer.Serialize(data, options);
        }

        public static T DeserializeWithConverters<T>(string json, IEnumerable<JsonConverter> converters)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            foreach (var converter in converters)
            {
                options.Converters.Add(converter);
            }

            return System.Text.Json.JsonSerializer.Deserialize<T>(json, options)
                ?? throw new InvalidOperationException("Null result!!!");
        }

        public static string SerializePolymorphic<T>(T baseObject)
        {
            return SerializeToJson(baseObject);
        }

        public static T DeserializePolymorphic<T>(string json)
        {
            return DeserializeFromJson<T>(json);
        }

        public static T DeepClone<T>(T data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(data))!;
        }

        public static string SerializeToBase64<T>(T data)
        {
            return Convert.ToBase64String(SerializeToBytes(data));
        }

        public static T DeserializeFromBase64<T>(string base64)
        {
            return DeserializeFromBytes<T>(Convert.FromBase64String(base64));
        }

        public static void SerializeToNetwork<T>(T data, Stream networkStream)
        {
            SerializeToStream(data, networkStream);
        }

        public static T DeserializeFromNetwork<T>(Stream networkStream)
        {
            return DeserializeFromStream<T>(networkStream);
        }

        public static string SerializeToXml<T>(T data)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using var memory = new MemoryStream();
            serializer.Serialize(memory, data);
            return Encoding.UTF8.GetString(memory.ToArray());
        }

        public static T DeserializeFromXml<T>(string xml)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using var memory = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            return (T)serializer.Deserialize(memory)!;
        }

        public static string SerializeIgnoringNull<T>(T data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return System.Text.Json.JsonSerializer.Serialize(data, options);
        }

        public static bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static T TryDeserializeOrDefault<T>(string json, T fallback=default!)
        {
            try
            {
                return DeserializeFromJson<T>(json);
            }
            catch
            {
                return fallback;
            }
        }

        public static string SerializeWithNamingPolicy<T>(T data, JsonNamingPolicy namingPolicy)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = namingPolicy
            };

            return System.Text.Json.JsonSerializer.Serialize(data, options);
        }

        public static string SerializeIgnoreNullsToBase64<T>(T data)
        {
            var json = SerializeIgnoringNull(data);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        public static string SerializeIncludingPrivate<T>(T data)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    SerializeCompilerGeneratedMembers = true
                }
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(data, settings);
        }

        #endregion
    }
}
