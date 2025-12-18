using System.Text.Json.Serialization;

namespace OpenID4VPTest.Models;

/// <summary>
/// Represents an attribute to be requested in the DC API request.
/// </summary>
public class AttributeDefinition
{
    /// <summary>
    /// The namespace of the attribute (e.g., "org.iso.18013.5.1").
    /// </summary>
    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// The name of the attribute (e.g., "family_name").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the attribute.
    /// </summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this attribute is checked/selected for the request.
    /// </summary>
    [JsonPropertyName("checked")]
    public bool Checked { get; set; }
}

/// <summary>
/// Represents the input for creating a DC API request.
/// </summary>
public class GetRequestInput
{
    /// <summary>
    /// The protocol version (e.g., "openid4vp-v1").
    /// </summary>
    [JsonPropertyName("protocol")]
    public string Protocol { get; set; } = "openid4vp-v1";

    /// <summary>
    /// The document type being requested (e.g., "org.iso.18013.5.1.mDL").
    /// </summary>
    [JsonPropertyName("doctype")]
    public string Doctype { get; set; } = string.Empty;

    /// <summary>
    /// Dictionary of attributes to request, keyed by their identifier.
    /// </summary>
    [JsonPropertyName("attrs")]
    public Dictionary<string, AttributeDefinition> Attrs { get; set; } = new();

    /// <summary>
    /// Whether to encrypt the response.
    /// </summary>
    [JsonPropertyName("encrypt_response")]
    public bool EncryptResponse { get; set; }

    /// <summary>
    /// Whether to sign the request.
    /// </summary>
    [JsonPropertyName("sign_request")]
    public bool SignRequest { get; set; }

    /// <summary>
    /// Gets the list of checked attributes.
    /// </summary>
    public IEnumerable<AttributeDefinition> GetCheckedAttributes()
        => Attrs.Values.Where(attr => attr.Checked);
}
