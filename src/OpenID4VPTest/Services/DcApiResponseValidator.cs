using Microsoft.IdentityModel.Tokens;
using OpenID4VPTest.Models;
using PeterO.Cbor;

namespace OpenID4VPTest.Services;

/// <summary>
/// Service for validating DC API responses and extracting credential data.
/// </summary>
public static class DcApiResponseValidator
{
    private const string DocumentsLabel = "documents";
    private const string IssuerSignedLabel = "issuerSigned";
    private const string NameSpacesLabel = "nameSpaces";
    private const string DocTypeLabel = "docType";
    private const string DeviceSignedLabel = "deviceSigned";

    /// <summary>
    /// Validates a DC API response and extracts credential attributes.
    /// </summary>
    /// <param name="input">The validation request input.</param>
    /// <returns>A ValidateResponseOutput containing debug info and extracted attributes.</returns>
    public static ValidateResponseOutput Validate(ValidateRequestInput input)
    {
        var response = new ValidateResponseOutput();
        var debugEntries = new List<DebugEntry>();
        var responseData = new List<ResponseDataEntry>();

        // Check if response is encrypted
        debugEntries.Add(new DebugEntry
        {
            Name = "Response Encrypted",
            Value = false // For unsigned protocol, response is not encrypted
        });

        // Get VP token size
        var vpTokenSize = input.Data.VpToken.Count;
        debugEntries.Add(new DebugEntry
        {
            Name = "Vp_token Size",
            Value = vpTokenSize.ToString()
        });

        // Process each credential in the VP token
        foreach (var (credentialId, encodedResponses) in input.Data.VpToken)
        {
            var verificationResult = "Success";
            var documentCount = 0;

            foreach (var encodedResponse in encodedResponses)
            {
                try
                {
                    // Decode the base64-encoded CBOR DeviceResponse
                    var bytes = Base64UrlEncoder.DecodeBytes(encodedResponse);
                    var cbor = CBORObject.DecodeFromBytes(bytes);

                    // Extract documents from the device response
                    var documents = ExtractDocuments(cbor);
                    documentCount = documents.Count;

                    // Extract claims from each document
                    foreach (var document in documents)
                    {
                        var claims = ExtractClaims(document);
                        responseData.AddRange(claims);
                    }
                }
                catch (Exception ex)
                {
                    verificationResult = $"Failed: {ex.Message}";
                }
            }

            debugEntries.Add(new DebugEntry
            {
                Name = $"Device Response Verification for {credentialId}",
                Value = verificationResult
            });

            debugEntries.Add(new DebugEntry
            {
                Name = $"Document Size for {credentialId}",
                Value = documentCount
            });
        }

        response.Debug = debugEntries;
        response.ResponseData = responseData;

        return response;
    }

    /// <summary>
    /// Extracts documents from a CBOR-encoded DeviceResponse.
    /// </summary>
    private static List<CBORObject> ExtractDocuments(CBORObject deviceResponse)
    {
        var documents = new List<CBORObject>();

        // Check if this is a DeviceResponse with documents array
        if (deviceResponse.ContainsKey(DocumentsLabel))
        {
            var documentsArray = deviceResponse[DocumentsLabel];
            foreach (var document in documentsArray.Values)
            {
                documents.Add(document);
            }
        }
        // Check if this is a single document (mdoc format)
        else if (deviceResponse.ContainsKey(IssuerSignedLabel))
        {
            documents.Add(deviceResponse);
        }

        return documents;
    }

    /// <summary>
    /// Extracts claims from a CBOR document.
    /// </summary>
    private static List<ResponseDataEntry> ExtractClaims(CBORObject document)
    {
        var claims = new List<ResponseDataEntry>();

        try
        {
            // Get issuerSigned.nameSpaces
            if (!document.ContainsKey(IssuerSignedLabel))
                return claims;

            var issuerSigned = document[IssuerSignedLabel];
            if (!issuerSigned.ContainsKey(NameSpacesLabel))
                return claims;

            var nameSpaces = issuerSigned[NameSpacesLabel];

            // Iterate through all namespaces
            foreach (var nameSpaceKey in nameSpaces.Keys)
            {
                var nameSpaceItems = nameSpaces[nameSpaceKey];

                // Each item in the namespace is a CBOR-tagged byte string containing an IssuerSignedItem
                foreach (var item in nameSpaceItems.Values)
                {
                    var claim = ExtractClaimFromIssuerSignedItem(item);
                    if (claim != null)
                    {
                        claims.Add(claim);
                    }
                }
            }
        }
        catch
        {
            // If we can't parse, return empty list
        }

        return claims;
    }

    /// <summary>
    /// Extracts a claim from an IssuerSignedItem.
    /// </summary>
    private static ResponseDataEntry? ExtractClaimFromIssuerSignedItem(CBORObject item)
    {
        try
        {
            // IssuerSignedItem is typically a tagged byte string (tag 24)
            CBORObject issuerSignedItem;

            if (item.HasMostOuterTag(24))
            {
                // Decode the tagged byte string
                var bytes = item.GetByteString();
                issuerSignedItem = CBORObject.DecodeFromBytes(bytes);
            }
            else if (item.Type == CBORType.ByteString)
            {
                var bytes = item.GetByteString();
                issuerSignedItem = CBORObject.DecodeFromBytes(bytes);
            }
            else
            {
                issuerSignedItem = item;
            }

            // Extract elementIdentifier and elementValue
            var elementIdentifier = issuerSignedItem["elementIdentifier"];
            var elementValue = issuerSignedItem["elementValue"];

            if (elementIdentifier == null || elementValue == null)
                return null;

            var name = elementIdentifier.AsString();
            var value = ConvertCborToObject(elementValue);

            return new ResponseDataEntry
            {
                Name = name,
                Value = value
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a CBOR value to a .NET object for JSON serialization.
    /// </summary>
    private static object ConvertCborToObject(CBORObject cbor)
    {
        return cbor.Type switch
        {
            CBORType.Boolean => cbor.AsBoolean(),
            CBORType.Integer => cbor.AsInt32Value(),
            CBORType.FloatingPoint => cbor.AsDouble(),
            CBORType.TextString => cbor.AsString(),
            CBORType.ByteString => Convert.ToBase64String(cbor.GetByteString()),
            CBORType.Array => cbor.Values.Select(ConvertCborToObject).ToList(),
            CBORType.Map => cbor.Keys.ToDictionary(
                k => k.AsString(),
                k => ConvertCborToObject(cbor[k])),
            _ => cbor.ToString()
        };
    }
}
