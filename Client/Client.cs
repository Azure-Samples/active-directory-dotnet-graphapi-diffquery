//-------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="Microsoft">
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
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Sample implementation of obtaining changes from graph using Differential Query.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// JavaScript Serializer.
        /// </summary>
        private static readonly JavaScriptSerializer javascriptSerializer = new JavaScriptSerializer();

        /// <summary>
        /// Logger to be used for logging output/debug.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="tenantDomainName">Windows Azure AD tenant domain name.</param>
        /// <param name="appPrincipalId">Service principal ID.</param>
        /// <param name="appPrincipalPassword">Service principal password.</param>
        /// <param name="logger">Logger to be used for logging output/debug.</param>
        public Client(
            string tenantDomainName,
            string appPrincipalId,
            string appPrincipalPassword,
            ILogger logger)
        {
            this.ReadConfiguration();

            this.TenantDomainName = tenantDomainName;
            this.AppPrincipalId = appPrincipalId;
            this.AppPrincipalPassword = appPrincipalPassword;
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the Graph service endpoint.
        /// </summary>
        protected string AzureADServiceHost { get; set; }

        /// <summary>
        /// Gets or sets the OAuth2 bearer token.
        /// </summary>
        protected string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the Graph API version.
        /// </summary>
        protected string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets the Windows Azure AD Access Control Service endpoint.
        /// </summary>
        private static string StsUrl { get; set; }

        /// <summary>
        /// Gets or sets the well known service principal ID for Windows Azure AD Access Control.
        /// </summary>
        private static string ProtectedResourcePrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the Windows Azure AD tenant domain name.
        /// </summary>
        private string TenantDomainName { get; set; }

        /// <summary>
        /// Gets or sets the service principal ID for your application.
        /// </summary>
        private string AppPrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the service principal password for your application.
        /// </summary>
        private string AppPrincipalPassword { get; set; }

        /// <summary>
        /// Calls the Differential Query service and returns the result.
        /// </summary>
        /// <param name="skipToken">
        /// Skip token returned by a previous call to the service or <see langref="null"/>.
        /// </param>
        /// <returns>Result from the Differential Query service.</returns>
        public DifferentialQueryResult DifferentialQuery(string skipToken)
        {
            return this.DifferentialQuery(
                "directoryObjects",
                skipToken,
                new string[0], 
                new string[0]);
        }

        /// <summary>
        /// Calls the Differential Query service and returns the result.
        /// </summary>
        /// <param name="resourceSet">Name of the resource set to query.</param>
        /// <param name="skipToken">
        /// Skip token returned by a previous call to the service or <see langref="null"/>.
        /// </param>
        /// <param name="objectClassList">List of directory object classes to retrieve.</param>
        /// <param name="propertyList">List of directory properties to retrieve.</param>
        /// <returns>Result from the Differential Query service.</returns>
        public DifferentialQueryResult DifferentialQuery(
            string resourceSet,
            string skipToken,
            ICollection<string> objectClassList,
            ICollection<string> propertyList)
        {
            WebClient webClient = new WebClient();
            webClient.QueryString.Add("api-version", this.ApiVersion);
            webClient.QueryString.Add("deltaLink", skipToken ?? String.Empty);

            if (propertyList.Any())
            {
                webClient.QueryString.Add("$select", String.Join(",", propertyList));
            }

            if (objectClassList.Any())
            {
                webClient.QueryString.Add(
                    "$filter",
                    String.Join(" or ", objectClassList.Select(x => String.Format("isof('{0}')", x))));
            }

            byte[] responseBytes = null;

            this.InvokeOperationWithRetry(
                () => { responseBytes = DownloadData(webClient, resourceSet); });

            if (responseBytes != null)
            {
                return new DifferentialQueryResult(
                    javascriptSerializer.DeserializeObject(
                        Encoding.UTF8.GetString(responseBytes)) as Dictionary<string, object>);
            }

            return null;
        }

        /// <summary>
        /// Calls the Graph service and returns the directory object with the specified object ID.
        /// </summary>
        /// <param name="objectId">Object ID of the object to retrieve.</param>
        /// <returns>Directory object with the specified object ID.</returns>
        public Dictionary<string, object> GetDirectoryObject(string objectId)
        {
            WebClient webClient = new WebClient();
            
            webClient.QueryString.Add("api-version", this.ApiVersion);

            string suffix = string.Format(
                CultureInfo.InvariantCulture,
                "directoryObjects('{0}')",
                objectId);

            byte[] responseBytes = null;

            this.InvokeOperationWithRetry(
                () => { responseBytes = DownloadData(webClient, suffix); });

            if (responseBytes != null)
            {
                return javascriptSerializer.DeserializeObject(
                    Encoding.UTF8.GetString(responseBytes)) as Dictionary<string, object>;
            }

            return null;
        }

        #region helpers

        /// <summary>
        /// Gets the Oauth2 Authorization header from Windows Azure AD Access Control.
        /// </summary>
        /// <returns>OAuth2 bearer token</returns>
        protected virtual string GetAuthorizationHeader()
        {
            if (this.AccessToken != null)
            {
                return this.AccessToken;
            }

            string postData = String.Format(
                CultureInfo.InvariantCulture,
                "grant_type=client_credentials&resource={0}&client_id={1}&client_secret={2}",
                HttpUtility.UrlEncode(ProtectedResourcePrincipalId),
                HttpUtility.UrlEncode(this.AppPrincipalId),
                HttpUtility.UrlEncode(this.AppPrincipalPassword));
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                String.Format(StsUrl, this.TenantDomainName));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AccessTokenFormat));
                    AccessTokenFormat token = (AccessTokenFormat)ser.ReadObject(stream);
                    this.AccessToken = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} {1}",
                        token.token_type,
                        token.access_token);
                    return this.AccessToken;
                }
            }
        }

        /// <summary>
        /// Returns a string that can logged given a <see cref="NameValueCollection"/>.
        /// </summary>
        /// <param name="queryParameters">Query parameters to be logged.</param>
        /// <returns>String to be logged.</returns>
        private static string LogQueryParameters(NameValueCollection queryParameters)
        {
            string logString = string.Empty;
            foreach (string key in queryParameters.AllKeys)
            {
                logString = String.Join("&", logString, String.Join("=", key, queryParameters[key]));
            }

            return logString;
        }

        /// <summary>
        /// Reads the client configuration.
        /// </summary>
        private void ReadConfiguration()
        {
            this.AzureADServiceHost = Configuration.GetElementValue("AzureADServiceHost");
            this.ApiVersion = Configuration.GetElementValue("ApiVersion");
            StsUrl = Configuration.GetElementValue("StsUrl");
            ProtectedResourcePrincipalId = Configuration.GetElementValue("ProtectedResourcePrincipalId");
        }

        /// <summary>
        /// Adds the required headers to the specified web client.
        /// </summary>
        /// <param name="webClient">Web client to add the required headers to.</param>
        private void AddHeaders(WebClient webClient)
        {
            webClient.Headers.Add(Constants.HeaderNameAuthorization, this.GetAuthorizationHeader());
            webClient.Headers.Add(Constants.HeaderNameClientRequestId, Guid.NewGuid().ToString());
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=minimalmetadata");
        }

        /// <summary>
        /// Constructs the URI with the specified suffix and downloads it with the specified web client.
        /// </summary>
        /// <param name="webClient">Web client to be used to download the URI.</param>
        /// <param name="suffix">Suffix to be used to construct the URI.</param>
        /// <returns>Byte array containing the downloaded URI.</returns>
        private byte[] DownloadData(WebClient webClient, string suffix)
        {
            this.AddHeaders(webClient);
            string serviceEndPoint = string.Format(
                @"https://{0}/{1}/{2}",
                this.AzureADServiceHost,
                this.TenantDomainName,
                suffix);

            // Log the query string and endpoint.
            if (this.logger != null)
            {
                this.logger.LogDebug("Making call to endpoint : {0}", serviceEndPoint);
                this.logger.LogDebug("Query Parameters : {0}", LogQueryParameters(webClient.QueryString));
            }

            return webClient.DownloadData(serviceEndPoint);
        }

        /// <summary>
        /// Delegate to invoke the specified operation, and retry if necessary.
        /// </summary>
        /// <param name="operation">Operation to invoke.</param>
        private void InvokeOperationWithRetry(Action operation)
        {
            int retryCount = Constants.MaxRetryAttempts;
            while (retryCount > 0)
            {
                try
                {
                    operation();

                    // Operation was successful
                    retryCount = 0;
                }
                catch (InvalidOperationException ex)
                {
                    // Operation not successful

                    // De-serialize error message to check the error code from AzureAD Service
                    ParsedException parsedException = ParsedException.Parse(ex);
                    if (parsedException == null)
                    {
                        // Could not parse the exception so it wasn't in the format of DataServiceException
                        throw;
                    }

                    // Look at the error code to determine if we want to retry on this exception 
                    switch (parsedException.Code)
                    {
                        // These are the errors we don't want to retry on
                        // Please look at the descriptions for details about each of these
                        case Constants.MessageIdAuthorizationIdentityDisabled:
                        case Constants.MessageIdAuthorizationIdentityNotFound:
                        case Constants.MessageIdAuthorizationRequestDenied:
                        case Constants.MessageIdBadRequest:
                        case Constants.MessageIdContractVersionHeaderMissing:
                        case Constants.MessageIdHeaderNotSupported:
                        case Constants.MessageIdInternalServerError:
                        case Constants.MessageIdInvalidDataContractVersion:
                        case Constants.MessageIdInvalidReplicaSessionKey:
                        case Constants.MessageIdInvalidRequestUrl:
                        case Constants.MessageIdMediaTypeNotSupported:
                        case Constants.MessageIdThrottledPermanently:
                        case Constants.MessageIdThrottledTemporarily:
                        case Constants.MessageIdUnauthorized:
                        case Constants.MessageIdUnknown:
                        case Constants.MessageIdUnsupportedQuery:
                        case Constants.MessageIdUnsupportedToken:
                        {
                            // We just create a new exception with the message
                            // and throw it so that the 'OnException' handler handles it
                            throw new InvalidOperationException(parsedException.Message.Value, ex);
                        }

                        // This means that the token has expired. 
                        case Constants.MessageIdExpired:
                        {
                            // Renew the token and retry the operation. This is done as a 
                            // part of the operation itself (AddHeaders)
                            this.AccessToken = null;
                            retryCount--;
                            break;
                        }

                        default:
                        {
                            // Not sure what happened, don't want to retry
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Access token format.
        /// </summary>
        [DataContract]
        private class AccessTokenFormat
        {
            /// <summary>
            /// Gets or sets the token type.
            /// </summary>
            [SuppressMessage(
                "Microsoft.StyleCop.CSharp.NamingRules",
                "SA1300:ElementMustBeginWithUpperCaseLetter",
                Justification = "JSON key name")]
            [DataMember]
            internal string token_type { get; set; }

            /// <summary>
            /// Gets or sets the access token.
            /// </summary>
            [SuppressMessage(
                "Microsoft.StyleCop.CSharp.NamingRules",
                "SA1300:ElementMustBeginWithUpperCaseLetter",
                Justification = "JSON key name")]
            [DataMember]
            internal string access_token { get; set; }
        }

        #endregion
    }
}
