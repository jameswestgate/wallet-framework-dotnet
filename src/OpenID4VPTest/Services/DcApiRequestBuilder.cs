using System.Security.Cryptography;
using OpenID4VPTest.Models;

namespace OpenID4VPTest.Services;

/// <summary>
/// Service for building DC API requests from input parameters.
/// </summary>
public static class DcApiRequestBuilder
{
    private const string MsoMdocFormat = "mso_mdoc";
    private const string DcApiResponseMode = "dc_api";
    private const string DcApiJwtResponseMode = "dc_api.jwt";
    private const string UnsignedProtocol = "openid4vp-v1-unsigned";
    private const string SignedProtocol = "openid4vp-v1-signed";
    
    // ES256 algorithm identifier (COSE)
    private const int Es256AlgorithmId = -7;

    /// <summary>
    /// Builds a GetRequestResponse from the input parameters.
    /// </summary>
    /// <param name="input">The input parameters for the request.</param>
    /// <returns>A complete GetRequestResponse ready to be serialized.</returns>
    public static GetRequestResponse Build(GetRequestInput input)
    {
        var nonce = GenerateNonce();
        var (privateKey, publicKey) = GenerateEcKeyPair();
        
        var checkedAttrs = input.GetCheckedAttributes().ToList();
        
        var claims = checkedAttrs.Select(attr => new ClaimPathEntry
        {
            Path = new[] { attr.Namespace, attr.Name }
        }).ToArray();

        var credentialQuery = new CredentialQueryResponse
        {
            Id = "cred1",
            Format = MsoMdocFormat,
            Meta = new CredentialMeta
            {
                DoctypeValue = input.Doctype
            },
            Claims = claims
        };

        var dcqlQuery = new DcqlQueryResponse
        {
            Credentials = new[] { credentialQuery }
        };

        var clientMetadata = new ClientMetadataResponse
        {
            VpFormatsSupported = new VpFormatsSupported
            {
                MsoMdoc = new MsoMdocFormat
                {
                    DeviceAuthAlgValues = new[] { Es256AlgorithmId },
                    IssuerAuthAlgValues = new[] { Es256AlgorithmId }
                }
            }
        };

        var responseMode = input.SignRequest ? DcApiJwtResponseMode : DcApiResponseMode;
        var protocol = input.SignRequest ? SignedProtocol : UnsignedProtocol;

        var authorizationRequest = new AuthorizationRequestResponse
        {
            ClientMetadata = clientMetadata,
            DcqlQuery = dcqlQuery,
            Nonce = nonce,
            ResponseMode = responseMode,
            ResponseType = "vp_token"
        };

        var state = new RequestState
        {
            CredentialType = MsoMdocFormat,
            Nonce = nonce,
            PrivateKey = privateKey,
            PublicKey = publicKey
        };

        return new GetRequestResponse
        {
            Protocol = protocol,
            Request = authorizationRequest,
            State = state
        };
    }

    /// <summary>
    /// Generates a cryptographically secure nonce.
    /// </summary>
    private static string GenerateNonce()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    /// Generates an EC P-256 key pair and returns base64-encoded private and public keys.
    /// </summary>
    private static (string PrivateKey, string PublicKey) GenerateEcKeyPair()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        
        var parameters = ecdsa.ExportParameters(true);
        
        // Export private key (D parameter)
        var privateKey = Convert.ToBase64String(parameters.D!);
        
        // Export public key in uncompressed format (0x04 || X || Y)
        var publicKeyBytes = new byte[65];
        publicKeyBytes[0] = 0x04;
        Array.Copy(parameters.Q.X!, 0, publicKeyBytes, 1, 32);
        Array.Copy(parameters.Q.Y!, 0, publicKeyBytes, 33, 32);
        var publicKey = Convert.ToBase64String(publicKeyBytes);
        
        return (privateKey, publicKey);
    }
}
