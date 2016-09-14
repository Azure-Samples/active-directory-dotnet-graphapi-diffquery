---
services: active-directory
platforms: dotnet
author: dstrockis
---

# ConsoleApp-GraphAPI-DiffQuery-DotNet

This console application demonstrates how to make Differential Query calls to the Graph API, allowing applications to request only changed data from Azure AD tenants.

For more information about how the protocols work in this scenario and other scenarios, see the REST API and Authentication Scenarios on http://msdn.microsoft.com/aad.

## How To Run This Sample

To run this sample you will need:
- Visual 2013
- An Internet connection
- An Azure subscription (a free trial is sufficient)

Every Azure subscription has an associated Azure Active Directory tenant.  If you don't already have an Azure subscription, you can get a free subscription by signing up at [https://azure.microsoft.com](https://azure.microsoft.com).  All of the Azure AD features used by this sample are available free of charge.

### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-dotnet-graphapi-diffquery.git`

### Step 2:  Run the sample in Visual Studio 2013

The sample app is preconfigured to read data from a Demonstration company (GraphDir1.onMicrosoft.com) in Azure AD. 

### Step 3:  Running this application with your Azure Active Directory tenant

#### Register the MVC Sample app for your own tenant

1. Sign in to the [Azure portal](https://portal.azure.com).
2. On the top bar, click on your account and under the **Directory** list, choose the Active Directory tenant where you wish to register your application.
3. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
4. Click on **App registrations** and choose **Add**.
5. Enter a friendly name for the application, for example 'WebApp for Azure AD' and select 'Web Application and/or Web API' as the Application Type. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:44322`. Click on **Create** to create the application.
6. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
7. Find the Application ID value and copy it to the clipboard.
8. From the settings page, click on 'Reply URLs' and add the reply URL address used to return the authorization code returned during Authorization code flow.  For example: "https://localhost:44322/".
9. Configure Permissions for your application - in the Settings menu, choose the 'Required permissions' section, click on **Add**, then **Select an API**, and select 'Microsoft Graph' (this is the Graph API). Then, click on  **Select Permissions** and select 'Read Directory Data'.

## About The Code

Coming soon.
