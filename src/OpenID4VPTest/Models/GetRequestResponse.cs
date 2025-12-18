using System.Text.Json.Serialization;

namespace OpenID4VPTest.Models;

/// <summary>
/// Represents a claim path in the DCQL query.
/// </summary>
public class ClaimPathEntry
{
    /// <summary>
    /// The path components for the claim.
    /// </summary>
    [JsonPropertyName("path")]
    public string[] Path { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Represents credential metadata in the DCQL query.
/// </summary>
public class CredentialMeta
{
    /// <summary>
    /// The document type value for mso_mdoc credentials.
    /// </summary>
    [JsonPropertyName("doctype_value")]
    public string? DoctypeValue { get; set; }

    /// <summary>
    /// The VCT values for SD-JWT credentials.
    /// </summary>
    [JsonPropertyName("vct_values")]
    public string[]? VctValues { get; set; }
}

/// <summary>
/// Represents a credential query in the DCQL query.
/// </summary>
public class CredentialQueryResponse
{
    /// <summary>
    /// The list of claims being requested.
    /// </summary>
    [JsonPropertyName("claims")]
    public ClaimPathEntry[] Claims { get; set; } = Array.Empty<ClaimPathEntry>();

    /// <summary>
    /// The format of the credential (e.g., "mso_mdoc").
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// The identifier for this credential query.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The metadata for the credential.
    /// </summary>
    [JsonPropertyName("meta")]
    public CredentialMeta Meta { get; set; } = new();
}

/// <summary>
/// Represents the DCQL query structure.
/// </summary>
public class DcqlQueryResponse
{
    /// <summary>
    /// The list of credential queries.
    /// </summary>
    [JsonPropertyName("credentials")]
    public CredentialQueryResponse[] Credentials { get; set; } = Array.Empty<CredentialQueryResponse>();
}

/// <summary>
/// Represents the VP formats supported by the client.
/// </summary>
public class VpFormatsSupported
{
    /// <summary>
    /// MSO MDOC format support configuration.
    /// </summary>
    [JsonPropertyName("mso_mdoc")]
    public MsoMdocFormat? MsoMdoc { get; set; }
}

/// <summary>
/// Represents MSO MDOC format configuration.
/// </summary>
public class MsoMdocFormat
{
    /// <summary>
    /// Device authentication algorithm values.
    /// </summary>
    [JsonPropertyName("deviceauth_alg_values")]
    public int[] DeviceAuthAlgValues { get; set; } = Array.Empty<int>();

    /// <summary>
    /// Issuer authentication algorithm values.
    /// </summary>
    [JsonPropertyName("issuerauth_alg_values")]
    public int[] IssuerAuthAlgValues { get; set; } = Array.Empty<int>();
}

/// <summary>
/// Represents client metadata in the authorization request.
/// </summary>
public class ClientMetadataResponse
{
    /// <summary>
    /// The VP formats supported by the client.
    /// </summary>
    [JsonPropertyName("vp_formats_supported")]
    public VpFormatsSupported VpFormatsSupported { get; set; } = new();
}

/// <summary>
/// Represents the authorization request content.
/// </summary>
public class AuthorizationRequestResponse
{
    /// <summary>
    /// The client metadata.
    /// </summary>
    [JsonPropertyName("client_metadata")]
    public ClientMetadataResponse ClientMetadata { get; set; } = new();

    /// <summary>
    /// The DCQL query.
    /// </summary>
    [JsonPropertyName("dcql_query")]
    public DcqlQueryResponse DcqlQuery { get; set; } = new();

    /// <summary>
    /// The nonce for the request.
    /// </summary>
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = string.Empty;

    /// <summary>
    /// The response mode (e.g., "dc_api").
    /// </summary>
    [JsonPropertyName("response_mode")]
    public string ResponseMode { get; set; } = string.Empty;

    /// <summary>
    /// The response type (e.g., "vp_token").
    /// </summary>
    [JsonPropertyName("response_type")]
    public string ResponseType { get; set; } = "vp_token";
}

/// <summary>
/// Represents the session state for the request.
/// </summary>
public class RequestState
{
    /// <summary>
    /// The credential type being requested.
    /// </summary>
    [JsonPropertyName("credential_type")]
    public string CredentialType { get; set; } = string.Empty;

    /// <summary>
    /// The nonce for the request.
    /// </summary>
    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = string.Empty;

    /// <summary>
    /// The base64-encoded private key for the session.
    /// </summary>
    [JsonPropertyName("private_key")]
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// The base64-encoded public key for the session.
    /// </summary>
    [JsonPropertyName("public_key")]
    public string PublicKey { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response from the GetRequest endpoint.
/// </summary>
public class GetRequestResponse
{
    /// <summary>
    /// The protocol version (e.g., "openid4vp-v1-unsigned" or "openid4vp-v1-signed").
    /// </summary>
    [JsonPropertyName("protocol")]
    public string Protocol { get; set; } = string.Empty;

    /// <summary>
    /// The authorization request content.
    /// </summary>
    [JsonPropertyName("request")]
    public AuthorizationRequestResponse Request { get; set; } = new();

    /// <summary>
    /// The session state for validating responses.
    /// </summary>
    [JsonPropertyName("state")]
    public RequestState State { get; set; } = new();
}
