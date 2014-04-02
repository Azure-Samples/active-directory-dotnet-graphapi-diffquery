//-------------------------------------------------------------------------------------------------
// <copyright file="DirectoryChangeManager.cs" company="Microsoft">
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
    using System.IO;
    using System.Threading;
    using System.Web.Script.Serialization;
    using DifferentialQueryClient;

    /// <summary>
    /// Defines methods to call the Differential Query service and process the results.
    /// </summary>
    public class DirectoryChangeManager : IDirectoryChangeManager
    {
        /// <summary>
        /// Cookie manager.
        /// </summary>
        private static readonly CookieManager cookieManager = new CookieManager();

        /// <summary>
        /// Differential Query client.
        /// </summary>
        private static readonly Client client = new Client(
                ConfigurationManager.AppSettings["TenantDomainName"],
                ConfigurationManager.AppSettings["AppPrincipalId"],
                ConfigurationManager.AppSettings["AppPrincipalPassword"],
                Logger.DefaultLogger);

        /// <summary>
        /// JavaScript serializer.
        /// </summary>
        private static readonly JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

        /// <summary>
        /// Directory object handler.
        /// </summary>
        private static readonly IDirectoryObjectHandler directoryObjectHandler = new DirectoryObjectHandler();

        /// <summary>
        /// Directory link handler.
        /// </summary>
        private static readonly IDirectoryLinkHandler directoryLinkHandler = new DirectoryLinkHandler();

        /// <summary>
        /// Output file for Differential Query result.
        /// </summary>
        private static StreamWriter outputFile;

        /// <summary>
        /// Calls the Differential Query service and processes the result.
        /// </summary>
        public void DifferentialQuery()
        {
            Logger.DefaultLogger.LogDebug(
                "Differential Query initialized for tenant {0}, with appPrincipalId {1}.",
                ConfigurationManager.AppSettings["TenantDomainName"],
                ConfigurationManager.AppSettings["AppPrincipalId"]);
            int pullIntervalSec = int.Parse(ConfigurationManager.AppSettings["PullIntervalSec"]);
            int retryAfterFailureIntervalSec = pullIntervalSec;
            string outputFilePath = Path.Combine(
                Environment.CurrentDirectory,
                "DifferentialQueryResult" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            outputFile = new StreamWriter(outputFilePath);
            Logger.DefaultLogger.Log("Differential Query sample application initialized");
            Logger.DefaultLogger.Log("Detected changes will be written to {0}.", outputFilePath);
            string skipToken = cookieManager.Read();

            while (true)
            {
                DifferentialQueryResult result;

                try
                {
                    result = client.DifferentialQuery(skipToken);
                }
                catch (Exception e)
                {
                    Logger.DefaultLogger.LogDebug(
                        "Differential Query request failed. Error: {0}, retrying after {1} sec",
                        e.Message,
                        retryAfterFailureIntervalSec);

                    Thread.Sleep(retryAfterFailureIntervalSec * 1000);
                    continue;
                }

                foreach (Dictionary<string, object> change in result.Changes)
                {
                    try
                    {
                        this.HandleChange(change);
                    }
                    catch (ArgumentException e)
                    {
                        Logger.DefaultLogger.Log("Invalid directory change: {0}", e.Message);
                    }
                }

                skipToken = result.SkipToken;
                cookieManager.Save(skipToken);

                if (!result.More)
                {
                    Logger.DefaultLogger.Log(
                        "Processed change(s) successfully. Will check back in {0} sec.",
                        pullIntervalSec);
                    Thread.Sleep(pullIntervalSec * 1000);
                }
            }
        }

        /// <summary>
        /// Processes the specified AAD object or link.
        /// </summary>
        /// <param name="change">Directory change representing an AAD object or link.</param>
        /// <exception cref="ArgumentNullException"><paramref name="change"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException">Invalid directory change.</exception>
        public void HandleChange(Dictionary<string, object> change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }

            if (!change.ContainsKey("odata.type"))
            {
                throw new ArgumentException("Invalid directory change", "change");
            }

            string directoryChangeType = change["odata.type"].ToString();
            switch (directoryChangeType)
            {
                case "Microsoft.WindowsAzure.ActiveDirectory.User":
                case "Microsoft.WindowsAzure.ActiveDirectory.Contact":
                case "Microsoft.WindowsAzure.ActiveDirectory.Group":
                    outputFile.WriteLine(javaScriptSerializer.Serialize(change));
                    Logger.DefaultLogger.Log(
                        "Detected a change about an AAD {0} with objectId {1}",
                        change["objectType"].ToString(),
                        change["objectId"]);
                    this.HandleObject(change);
                    break;

                case "Microsoft.WindowsAzure.ActiveDirectory.DirectoryLinkChange":
                    outputFile.WriteLine(javaScriptSerializer.Serialize(change));
                    Logger.DefaultLogger.Log(
                        "Detected a change about an AAD link with sourceObjectId {0} and targetObjectId {1}",
                        change["sourceObjectId"],
                        change["targetObjectId"]);
                    this.HandleLink(change);
                    break;

                default:
                    Logger.DefaultLogger.Log(
                        "Detected a change about unknown type {0} in AAD",
                        change["odata.type"]);
                    break;
            }
        }

        /// <summary>
        /// Processes the specified directory change representing an AAD object.
        /// </summary>
        /// <param name="change">Directory change representing an AAD object.</param>
        private void HandleObject(Dictionary<string, object> change)
        {
            bool isDeleted = change.ContainsKey("aad.isDeleted") &&
                bool.Parse(change["aad.isDeleted"].ToString());
            bool isSoftDeleted = change.ContainsKey("aad.isSoftDeleted") &&
                bool.Parse(change["aad.isSoftDeleted"].ToString());
            if (isDeleted || isSoftDeleted)
            {
                directoryObjectHandler.Delete(change);
            }
            else if (directoryObjectHandler.Exists(change))
            {
                directoryObjectHandler.Update(change);
            }
            else
            {
                directoryObjectHandler.Create(change);
            }
        }

        /// <summary>
        /// Processes the specified directory change representing an AAD link.
        /// </summary>
        /// <param name="change">Directory change representing an AAD link.</param>
        private void HandleLink(Dictionary<string, object> change)
        {
            bool isDeleted = change.ContainsKey("aad.isDeleted") &&
                bool.Parse(change["aad.isDeleted"].ToString());
            if (isDeleted)
            {
                directoryLinkHandler.Delete(change);
            }
            else if (directoryLinkHandler.Exists(change))
            {
                directoryLinkHandler.Update(change);
            }
            else 
            {
                directoryLinkHandler.Create(change);
            }
        }
    }
}
