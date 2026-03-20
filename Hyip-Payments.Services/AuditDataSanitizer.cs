using System.Text.Json;
using System.Text.RegularExpressions;

namespace Hyip_Payments.Services
{
    /// <summary>
    /// Helper class to sanitize sensitive data before audit logging
    /// </summary>
    public static class AuditDataSanitizer
    {
        // Sensitive field names that should be masked/excluded
        private static readonly HashSet<string> SensitiveFields = new(StringComparer.OrdinalIgnoreCase)
        {
            "password",
            "passwd",
            "pwd",
            "secret",
            "token",
            "apikey",
            "api_key",
            "accesstoken",
            "access_token",
            "refreshtoken",
            "refresh_token",
            "privatekey",
            "private_key",
            "clientsecret",
            "client_secret",
            "authorization",
            "auth",
            "securitycode",
            "security_code",
            "pin",
            "ssn",
            "socialsecurity",
            "creditcard",
            "credit_card",
            "cardnumber",
            "card_number",
            "cvv",
            "cvc",
            "encryptionkey",
            "encryption_key"
        };

        /// <summary>
        /// Sanitize an object by removing or masking sensitive fields
        /// </summary>
        public static string? SanitizeObject(object? obj)
        {
            if (obj == null)
                return null;

            try
            {
                // Serialize to JSON
                var json = JsonSerializer.Serialize(obj);
                
                // Sanitize the JSON string
                return SanitizeJson(json);
            }
            catch
            {
                // If serialization fails, return a safe message
                return "[Serialization Failed - Data Not Logged]";
            }
        }

        /// <summary>
        /// Sanitize a JSON string by masking sensitive fields
        /// </summary>
        public static string? SanitizeJson(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            try
            {
                // Parse JSON
                var jsonDoc = JsonDocument.Parse(json);
                var sanitized = SanitizeJsonElement(jsonDoc.RootElement);
                
                // Serialize back to string
                return JsonSerializer.Serialize(sanitized);
            }
            catch
            {
                // If parsing fails, use regex to mask sensitive fields
                return SanitizeWithRegex(json);
            }
        }

        /// <summary>
        /// Recursively sanitize JSON elements
        /// </summary>
        private static object? SanitizeJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var obj = new Dictionary<string, object?>();
                    foreach (var property in element.EnumerateObject())
                    {
                        // Check if field name is sensitive
                        if (IsSensitiveField(property.Name))
                        {
                            obj[property.Name] = "***REDACTED***";
                        }
                        else
                        {
                            obj[property.Name] = SanitizeJsonElement(property.Value);
                        }
                    }
                    return obj;

                case JsonValueKind.Array:
                    var arr = new List<object?>();
                    foreach (var item in element.EnumerateArray())
                    {
                        arr.Add(SanitizeJsonElement(item));
                    }
                    return arr;

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var longValue))
                        return longValue;
                    return element.GetDouble();

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Null:
                    return null;

                default:
                    return element.ToString();
            }
        }

        /// <summary>
        /// Check if a field name is sensitive
        /// </summary>
        private static bool IsSensitiveField(string fieldName)
        {
            return SensitiveFields.Contains(fieldName);
        }

        /// <summary>
        /// Fallback: Use regex to mask sensitive fields if JSON parsing fails
        /// </summary>
        private static string SanitizeWithRegex(string json)
        {
            // Pattern to match sensitive fields in JSON
            // Matches: "password": "value" or "password":"value"
            foreach (var field in SensitiveFields)
            {
                // Match both string and non-string values
                var pattern = $@"""{field}""\s*:\s*""[^""]*""";
                json = Regex.Replace(json, pattern, $@"""{field}"": ""***REDACTED***""", RegexOptions.IgnoreCase);
                
                // Match non-quoted values (numbers, booleans, etc.)
                pattern = $@"""{field}""\s*:\s*[^,}}]+";
                json = Regex.Replace(json, pattern, $@"""{field}"": ""***REDACTED***""", RegexOptions.IgnoreCase);
            }

            return json;
        }

        /// <summary>
        /// Sanitize a dictionary of key-value pairs
        /// </summary>
        public static Dictionary<string, string> SanitizeDictionary(Dictionary<string, string> data)
        {
            var sanitized = new Dictionary<string, string>();

            foreach (var kvp in data)
            {
                if (IsSensitiveField(kvp.Key))
                {
                    sanitized[kvp.Key] = "***REDACTED***";
                }
                else
                {
                    sanitized[kvp.Key] = kvp.Value;
                }
            }

            return sanitized;
        }

        /// <summary>
        /// Check if a string contains sensitive data (for quick checks)
        /// </summary>
        public static bool ContainsSensitiveData(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return SensitiveFields.Any(field => 
                text.Contains(field, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Mask a password string completely
        /// </summary>
        public static string MaskPassword(string? password)
        {
            if (string.IsNullOrEmpty(password))
                return "***EMPTY***";

            // Return fixed length mask to avoid revealing password length
            return "***REDACTED***";
        }
    }
}
