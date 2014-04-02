//-------------------------------------------------------------------------------------------------
// <copyright file="CookieManager.cs" company="Microsoft">
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
    /// <summary>
    /// Defines methods to manage the sync cookie obtained from Differential Query.
    /// </summary>
    public class CookieManager : ICookieManager
    {
        /// <summary>
        /// Saves the cookie into a persistent store.
        /// </summary>
        /// <param name="cookie">Cookie to save.</param>
        public void Save(string cookie)
        {
            // Implement a way to store the cookie in a file or any other persistent store.
        }

        /// <summary>
        /// Reads the cookie from the persistent store.
        /// </summary>
        /// <returns>Cookie read from the persistent store or <see langword="null"/> if none exists.</returns>
        public string Read()
        {
            // Implement a way to retrieve the cookie from a file or any other persistent store.
            return null;
        }
    }
}
