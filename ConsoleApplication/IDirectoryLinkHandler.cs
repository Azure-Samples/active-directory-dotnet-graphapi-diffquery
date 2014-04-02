//-------------------------------------------------------------------------------------------------
// <copyright file="IDirectoryLinkHandler.cs" company="Microsoft">
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
    /// Defines methods to process the AAD links returned by Differential Query.
    /// </summary>
    public interface IDirectoryLinkHandler
    {
        /// <summary>
        /// Creates the specified AAD link in the local store.
        /// </summary>
        /// <param name="change">Directory change representing a new link.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        void Create(Dictionary<string, object> change);

        /// <summary>
        /// Updates the specified AAD link in the local store.
        /// </summary>
        /// <param name="change">Directory change representing an update to an existing link.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        void Update(Dictionary<string, object> change);

        /// <summary>
        /// Determines whether the local store contains the specified AAD link.
        /// </summary>
        /// <param name="change">Directory change representing an AAD link.</param>
        /// <returns>
        /// <see langword="true"/> if the local store contains the specified AAD link;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        bool Exists(Dictionary<string, object> change);

        /// <summary>
        /// Deletes the specified AAD link from the local store.
        /// </summary>
        /// <param name="change">Directory change representing a deleted link.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        void Delete(Dictionary<string, object> change);
    }
}
