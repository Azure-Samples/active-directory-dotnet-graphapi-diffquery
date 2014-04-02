//-------------------------------------------------------------------------------------------------
// <copyright file="DirectoryLinkHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <summary>
//     Differential Query sample application.
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

namespace DifferentialQueryConsoleApplication
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using DifferentialQueryClient;

    /// <summary>
    /// Defines methods to process the AAD links returned by Differential Query.
    /// </summary>
    public class DirectoryLinkHandler : IDirectoryLinkHandler
    {
        /// <summary>
        /// Local link store.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, object>> linkStore =
            new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Differential query client.
        /// </summary>
        private static readonly Client client = new Client(
                ConfigurationManager.AppSettings["TenantDomainName"],
                ConfigurationManager.AppSettings["AppPrincipalId"],
                ConfigurationManager.AppSettings["AppPrincipalPassword"],
                Logger.DefaultLogger);

        /// <summary>
        /// Directory object handler, used to check if the source and target of a link exists in the local object store
        /// and create them if necessary.
        /// </summary>
        private readonly IDirectoryObjectHandler directoryObjectHandler = new DirectoryObjectHandler();

        /// <summary>
        /// Creates the specified AAD link in the local store.
        /// </summary>
        /// <param name="change">Directory change representing a new link.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory link.</exception>
        public void Create(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (change.ContainsKey("sourceObjectId") && change.ContainsKey("targetObjectId"))
            {
                string sourceObjectId = change["sourceObjectId"].ToString();
                string targetObjectId = change["targetObjectId"].ToString();

                this.Verify(sourceObjectId, targetObjectId);

                // Create the link
                linkStore.Add(sourceObjectId + targetObjectId, change);
                Logger.DefaultLogger.Log(
                    "Link {0} added to local store",
                    sourceObjectId + targetObjectId);
            }
            else
            {
                throw new ArgumentException("Invalid directory link", "change");
            }
        }

        /// <summary>
        /// Updates the specified AAD link in the local store.
        /// </summary>
        /// <param name="change">Directory change representing an update to an existing link.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory link.</exception>
        public void Update(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (change.ContainsKey("sourceObjectId") && change.ContainsKey("targetObjectId"))
            {
                string sourceObjectId = change["sourceObjectId"].ToString();
                string targetObjectId = change["targetObjectId"].ToString();

                this.Verify(sourceObjectId, targetObjectId);

                linkStore[sourceObjectId + targetObjectId] = change;
                Logger.DefaultLogger.Log(
                    "Link {0}{1} updated in local store",
                    sourceObjectId,
                    targetObjectId);
            }
            else
            {
                throw new ArgumentException("Invalid directory link", "change");
            }
        }

        /// <summary>
        /// Determines whether the local store contains the specified AAD link.
        /// </summary>
        /// <param name="change">Directory change representing an AAD link.</param>
        /// <returns>
        /// <see langword="true"/> if the local store contains the specified AAD link;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory link.</exception>
        public bool Exists(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (change.ContainsKey("sourceObjectId") && change.ContainsKey("targetObjectId"))
            {
                string sourceObjectId = change["sourceObjectId"].ToString();
                string targetObjectId = change["targetObjectId"].ToString();
                return linkStore.ContainsKey(sourceObjectId + targetObjectId);
            }

            throw new ArgumentException("Invalid directory link", "change");
        }

        /// <summary>
        /// Deletes the specified AAD link from the local store.
        /// </summary>
        /// <param name="change">Directory change representing a deleted link.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory link.</exception>
        public void Delete(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (change.ContainsKey("sourceObjectId") && change.ContainsKey("targetObjectId"))
            {
                string sourceObjectId = change["sourceObjectId"].ToString();
                string targetObjectId = change["targetObjectId"].ToString();

                linkStore.Remove(sourceObjectId + targetObjectId);
                Logger.DefaultLogger.Log(
                    "Link {0}{1} removed from local store",
                    sourceObjectId,
                    targetObjectId);
            }
            else
            {
                throw new ArgumentException("Invalid directory link", "change");
            }
        }

        /// <summary>
        /// Checks if the specified source and target exists in the local object store
        /// and retrieves them from AAD if necessary.
        /// </summary>
        /// <param name="sourceObjectId">source object ID.</param>
        /// <param name="targetObjectId">target object ID.</param>
        private void Verify(string sourceObjectId, string targetObjectId)
        {
            if (!this.directoryObjectHandler.Exists(sourceObjectId))
            {
                this.directoryObjectHandler.Create(client.GetDirectoryObject(sourceObjectId));
                Logger.DefaultLogger.Log(
                    "Manually retrieved object {0} from AAD.",
                    sourceObjectId);
            }

            if (!this.directoryObjectHandler.Exists(targetObjectId))
            {
                this.directoryObjectHandler.Create(client.GetDirectoryObject(targetObjectId));
                Logger.DefaultLogger.Log(
                    "Manually retrieved object {0} from AAD.",
                    targetObjectId);
            }
        }
    }
}
