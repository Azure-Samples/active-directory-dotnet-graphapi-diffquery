//-------------------------------------------------------------------------------------------------
// <copyright file="IDirectoryObjectHandler.cs" company="Microsoft">
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
    using System.Collections.Generic;

    /// <summary>
    /// Defines methods to process the AAD objects returned by Differential Query.
    /// </summary>
    public interface IDirectoryObjectHandler
    {
        /// <summary>
        /// Creates the specified AAD object in the local store.
        /// </summary>
        /// <param name="change">Directory change representing a new object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        void Create(Dictionary<string, object> change);

        /// <summary>
        /// Updates the specified AAD object in the local store.
        /// </summary>
        /// <param name="change">Directory change representing an update to an existing object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        void Update(Dictionary<string, object> change);

        /// <summary>
        /// Determines whether the local store contains the specified AAD object.
        /// </summary>
        /// <param name="change">Directory change representing an AAD object.</param>
        /// <returns>
        /// <see langword="true"/> if the local store contains the specified AAD object;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        bool Exists(Dictionary<string, object> change);

        /// <summary>
        /// Determines whether the local store contains an AAD object with the specified object ID.
        /// </summary>
        /// <param name="objectId">An AAD object ID.</param>
        /// <returns>
        /// <see langword="true"/> if the local store contains an AAD object with the specified object ID;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        bool Exists(string objectId);

        /// <summary>
        /// Deletes the specified AAD object from the local store.
        /// </summary>
        /// <param name="change">Directory change representing a deleted object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        void Delete(Dictionary<string, object> change);
    }
}
