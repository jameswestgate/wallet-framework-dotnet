using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenID4VPTest.Models;

/// <summary>
/// Represents the VP token data containing credential responses.
/// </summary>
public class VpTokenData
{
    /// <summary>
    /// Dictionary mapping credential IDs to arrays of base64-encoded credential responses.
    /// For mso_mdoc, each entry contains a CBOR-encoded DeviceResponse.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Credentials { get; set; }
}

/// <summary>
/// Represents the data portion of the validate request.
/// </summary>
public class ValidateRequestData
{
    /// <summary>
    /// The VP token containing credential responses.
    /// </summary>
    [JsonPropertyName("vp_token")]
    public Dictionary<string, List<string>> VpToken { get; set; } = new();
}

/// <summary>
/// Represents the state from the original request for validation.
/// </summary>
public class ValidateRequestState
{
    /// <summary>
    /// The credential type (e.g., "mso_mdoc").
    /// </summary>
    [JsonPropertyName("credential_type")]
    public string CredentialType { get; set; } = string.Empty;

    /// <summary>
    /// The nonce used in the original request.
    /// </summary>
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = string.Empty;

    /// <summary>
    /// The base64-encoded private key from the original request.
    /// </summary>
    [JsonPropertyName("private_key")]
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// The base64-encoded public key from the original request.
    /// </summary>
    [JsonPropertyName("public_key")]
    public string PublicKey { get; set; } = string.Empty;
}

/// <summary>
/// Represents the input for validating a DC API response.
/// </summary>
public class ValidateRequestInput
{
    /// <summary>
    /// The protocol used (e.g., "openid4vp-v1-unsigned").
    /// </summary>
    [JsonPropertyName("protocol")]
    public string Protocol { get; set; } = string.Empty;

    /// <summary>
    /// The data containing the VP token with credential responses.
    /// </summary>
    [JsonPropertyName("data")]
    public ValidateRequestData Data { get; set; } = new();

    /// <summary>
    /// The state from the original request for validation.
    /// </summary>
    [JsonPropertyName("state")]
    public ValidateRequestState State { get; set; } = new();

    /// <summary>
    /// The origin of the request.
    /// </summary>
    [JsonPropertyName("origin")]
    public string? Origin { get; set; }
}
