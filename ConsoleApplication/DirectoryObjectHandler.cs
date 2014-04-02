//-------------------------------------------------------------------------------------------------
// <copyright file="DirectoryObjectHandler.cs" company="Microsoft">
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

    /// <summary>
    /// Defines methods to process the AAD objects returned by Differential Query.
    /// </summary>
    public class DirectoryObjectHandler : IDirectoryObjectHandler
    {
        /// <summary>
        /// Local object store.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, object>> objectStore =
            new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Creates the specified AAD object in the local store.
        /// </summary>
        /// <param name="change">Directory change representing a new object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory object.</exception>
        public void Create(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (change.ContainsKey("objectId"))
            {
                string objectId = (string)change["objectId"];
                objectStore.Add(objectId, change);
                Logger.DefaultLogger.Log(
                    "Object {0} added to local store",
                    objectId);
            }
            else
            {
                throw new ArgumentException("Invalid directory object", "change");
            }
        }

        /// <summary>
        /// Updates the specified AAD object in the local store.
        /// </summary>
        /// <param name="change">Directory change representing an update to an existing object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory object.</exception>
        public void Update(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (change.ContainsKey("objectId"))
            {
                string objectId = (string)change["objectId"];
                objectStore[objectId] = change;
                Logger.DefaultLogger.Log(
                    "Object {0} updated in local store",
                    objectId);
            }
            else
            {
                throw new ArgumentException("Invalid directory object", "change");
            }
        }

        /// <summary>
        /// Determines whether the local store contains the specified AAD object.
        /// </summary>
        /// <param name="change">Directory change representing an AAD object.</param>
        /// <returns>
        /// <see langword="true"/> if the local store contains the specified AAD object;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory object.</exception>
        public bool Exists(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (change.ContainsKey("objectId"))
            {
                string objectId = (string)change["objectId"];
                return objectStore.ContainsKey(objectId);
            }

            throw new ArgumentException("Invalid directory object", "change");
        }

        /// <summary>
        /// Determines whether the local store contains an AAD object with the specified object ID.
        /// </summary>
        /// <param name="objectId">An AAD object ID.</param>
        /// <returns>
        /// <see langword="true"/> if the local store contains an AAD object with the specified object ID;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Exists(string objectId)
        {
            return objectStore.ContainsKey(objectId);
        }

        /// <summary>
        /// Deletes the specified AAD object from the local store.
        /// </summary>
        /// <param name="change">Directory change representing a deleted object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory object.</exception>
        public void Delete(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (change.ContainsKey("objectId"))
            {
                string objectId = (string)change["objectId"];
                objectStore.Remove(objectId);
                Logger.DefaultLogger.Log(
                    "Object {0} removed from local store",
                    objectId);
            }
            else
            {
                throw new ArgumentException("Invalid directory object", "change");
            }
        }
    }
}
