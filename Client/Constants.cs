//-------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <summary>
//     Differential Query sample client.
// 
//     This source is subject to the Sample Client End User License Agreement
//     included in this project.
// </summary>
//
// <remarks />
//
// <disclaimer>
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//     EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
//     WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </disclaimer>
//-------------------------------------------------------------------------------------------------

namespace DifferentialQueryClient
{
    /// <summary>
    /// This class contains the constant fields used in this sample.
    /// </summary>
    internal static class Constants
    {
        #region Resource Id

        /// <summary>
        /// Message Id for unauthorized request.
        /// </summary>
        public const string MessageIdUnauthorized = "Authentication_Unauthorized";

        /// <summary>
        /// Message id for expired tokens.
        /// </summary>
        public const string MessageIdExpired = "Authentication_ExpiredToken";

        /// <summary>
        /// Message id for unknown authentication failures.
        /// </summary>
        public const string MessageIdUnknown = "Authentication_Unknown";

        /// <summary>
        /// Message id for unsupported token type.
        /// </summary>
        public const string MessageIdUnsupportedToken = "Authentication_UnsupportedTokenType";

        /// <summary>
        /// Message id for the data contract missing error message
        /// </summary>
        public const string MessageIdContractVersionHeaderMissing = "Headers_DataContractVersionMissing";

        /// <summary>
        /// Message id for an invalid data contract version.
        /// </summary>
        public const string MessageIdInvalidDataContractVersion = "Headers_InvalidDataContractVersion";

        /// <summary>
        /// Message id for the data contract missing error message
        /// </summary>
        public const string MessageIdHeaderNotSupported = "Headers_HeaderNotSupported";

        /// <summary>
        /// The most generic message id, when the fault is due to a server error.
        /// </summary>
        public const string MessageIdInternalServerError = "Service_InternalServerError";

        /// <summary>
        /// The replica session key provided in the request is invalid.
        /// </summary>
        public const string MessageIdInvalidReplicaSessionKey = "Request_InvalidReplicaSessionKey";

        /// <summary>
        /// The replica session key provided in the request is invalid.
        /// </summary>
        public const string MessageIdBadRequest = "Request_BadRequest";

        /// <summary>
        /// User's data is not in the current datacenter.
        /// </summary>
        public const string MessageIdBindingRedirection = "Directory_BindingRedirection";

        /// <summary>
        /// Calling principal's information could not be read from the directory.
        /// </summary>
        public const string MessageIdAuthorizationIdentityNotFound = "Authorization_IdentityNotFound";

        /// <summary>
        /// Calling principal is disabled.
        /// </summary>
        public const string MessageIdAuthorizationIdentityDisabled = "Authorization_IdentityDisabled";

        /// <summary>
        /// Request is denied due to insufficient privileges.
        /// </summary>
        public const string MessageIdAuthorizationRequestDenied = "Authorization_RequestDenied";

        /// <summary>
        /// The request is throttled temporarily
        /// </summary>
        public const string MessageIdThrottledTemporarily = "Request_ThrottledTemporarily";

        /// <summary>
        /// The request is throttled permanently
        /// </summary>
        public const string MessageIdThrottledPermanently = "Request_ThrottledPermanently";

        /// <summary>
        /// The request query was rejected because it was either invalid or unsupported.
        /// </summary>
        public const string MessageIdUnsupportedQuery = "Request_UnsupportedQuery";

        /// <summary>
        /// Request is denied due to insufficient privileges.
        /// </summary>
        public const string MessageIdInvalidRequestUrl = "Request_InvalidRequestUrl";

        /// <summary>
        /// The requested media type is not supported - the $format parameter value is not supported.
        /// </summary>
        public const string MessageIdMediaTypeNotSupported = "Request_MediaTypeNotSupported";

        #endregion

        /// <summary>
        /// Maximum number of retry attempts for a given request.
        /// </summary>
        public const int MaxRetryAttempts = 3;

        /// <summary>
        /// Authorization header.
        /// </summary>
        public const string HeaderNameAuthorization = "Authorization";

        /// <summary>
        /// Client request ID header.
        /// </summary>
        public const string HeaderNameClientRequestId = "client-request-id";

        /// <summary>
        /// deltaLink query parameter.
        /// </summary>
        public const string DeltaLinkQueryParameter = "deltaLink";

        /// <summary>
        /// Instance annotation that represents if the entry is deleted.
        /// </summary>
        public const string IsDeletedInstanceAnnotation = "aad.isDeleted";

        /// <summary>
        /// Instance annotation that represents if the entry is soft-deleted.
        /// </summary>
        public const string IsSoftDeletedInstanceAnnotation = "aad.isSoftDeleted";

        /// <summary>
        /// Feed annotation that represents a URI to be called immediately for more changes.
        /// </summary>        
        public const string DeltaLinkFeedAnnotation = "aad.deltaLink";

        /// <summary>
        /// Feed annotation that represents a URI to be called later after the polling interval has passed.
        /// </summary>
        public const string NextLinkFeedAnnotation = "aad.nextLink";
    }
}
