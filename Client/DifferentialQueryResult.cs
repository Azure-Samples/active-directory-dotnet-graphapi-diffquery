//-------------------------------------------------------------------------------------------------
// <copyright file="DifferentialQueryResult.cs" company="Microsoft">
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
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Response from Differential Query service.
    /// </summary>
    public class DifferentialQueryResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DifferentialQueryResult"/> class.
        /// </summary>
        /// <param name="response">Response from Differential Query service.</param>
        /// <exception cref="ArgumentNullException"><paramref name="response"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid response.</exception>
        public DifferentialQueryResult(Dictionary<string, object> response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            Regex deltaLinkRegex =
                new Regex(
                    String.Format(CultureInfo.InvariantCulture, "{0}=(.+)", Constants.DeltaLinkQueryParameter),
                    RegexOptions.Compiled);            
            Match nextTokenMatch = null;

            if (response.ContainsKey(Constants.NextLinkFeedAnnotation))
            {
                nextTokenMatch = deltaLinkRegex.Match((string)response[Constants.NextLinkFeedAnnotation]);
                this.More = true;
            }
            else if (response.ContainsKey(Constants.DeltaLinkFeedAnnotation))
            {
                nextTokenMatch = deltaLinkRegex.Match((string)response[Constants.DeltaLinkFeedAnnotation]);
                this.More = false;
            }
            else
            {
                // missing nextLink/deltaLink
                throw new ArgumentException("missing nextLink/deltaLink", "response");
            }

            this.SkipToken = nextTokenMatch.Groups[1].Value;

            if (response.ContainsKey("value"))
            {
                this.Changes = Array.ConvertAll((object[])response["value"], o => (Dictionary<string, object>)o);
            }
            else
            {
                // missing changes
                throw new ArgumentException("missing changes", "response");
            }
        }

        /// <summary>
        /// Gets the collection of changes.
        /// </summary>
        public ICollection<Dictionary<string, object>> Changes { get; private set; }

        /// <summary>
        /// Gets the next synchronization cookie.
        /// </summary>
        public string SkipToken { get; private set; }

        /// <summary>
        /// Gets a value indicating whether more changes are available for synchronization.
        /// </summary>
        public bool More { get; private set; }
    }
}
