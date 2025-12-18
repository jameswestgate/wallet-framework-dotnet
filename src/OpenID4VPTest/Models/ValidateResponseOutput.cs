using System.Text.Json.Serialization;

namespace OpenID4VPTest.Models;

/// <summary>
/// Represents a debug entry in the validation response.
/// </summary>
public class DebugEntry
{
    /// <summary>
    /// The name of the debug entry.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The value of the debug entry (can be various types).
    /// </summary>
    [JsonPropertyName("value")]
    public object Value { get; set; } = null!;
}

/// <summary>
/// Represents a credential attribute/claim in the validation response.
/// </summary>
public class ResponseDataEntry
{
    /// <summary>
    /// The name of the attribute.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The value of the attribute.
    /// </summary>
    [JsonPropertyName("value")]
    public object Value { get; set; } = null!;
}

/// <summary>
/// Represents the response from validating a DC API response.
/// </summary>
public class ValidateResponseOutput
{
    /// <summary>
    /// Debug information about the validation process.
    /// </summary>
    [JsonPropertyName("debug")]
    public List<DebugEntry> Debug { get; set; } = new();

    /// <summary>
    /// The extracted credential attributes/claims.
    /// </summary>
    [JsonPropertyName("response_data")]
    public List<ResponseDataEntry> ResponseData { get; set; } = new();
}
