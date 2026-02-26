# Genesys Cloud - .NET OAuth WebView2 Control

[![NuGet Badge](https://buildstats.info/nuget/GenesysCloudOAuthWebView)](https://www.nuget.org/packages/GenesysCloudOAuthWebView/)

This is a library that provides a simple way to execute a Genesys Cloud OAuth 2 flow in a .NET application. This is accomplished by providing the OAuthWebView class, which uses the [Microsoft Edge WebView2](https://docs.microsoft.com/en-us/microsoft-edge/webview2/) control. When invoked, the control will navigate to the appropriate login URL, allow the user to authenticate securely, and will raise events for authentication or errors.

This library is intended as a modern replacement for the MSHTML-based [PureCloudOAuthControl](https://github.com/MyPureCloud/purecloud_api_dotnet_oauth_control).

# New version - v2.0-alpha (02/20/2026) - BREAKING CHANGES

**The new version (v2) of the OAuth WebView component introduces some breaking changes and additions:**
- The component now **supports PKCE Grant Flow and Implicit Grant Flow** (controlled using the config's `IsPKCEGrant` property). **PKCE Grant Flow is set as the default OAuth Grant Flow**.
- The **BeginImplicitGrant() method is deprecated and replaced with the new BeginOAuthGrant()** method.
- The OAuth WebView project generates the libraries for different framework versions: .Net Core 3.1, .Net Framework 4.7.2, .Net 8.0 (Windows), .Net 10.0 (Windows).

# Table of Contents
* [Example Applications](#example-applications)
* [Getting Started](#getting-started)
  * [Prerequisites](#prerequisites)
  * [Installation](#installation)
    * [New v2 version](#new-v2-version)
    * [Legacy v1 version](#legacy-v1-version)
  * [Usage](#usage)
    * [Creating an Instance](#creating-an-instance)
    * [Configuring the Control](#configuring-the-control)
    * [Starting the PKCE Grant Flow or the Implicit Grant Flow](#starting-the-pkce-grant-flow-or-the-implicit-grant-flow)
    * [Disposing the Control](#disposing-the-control)
  * [Using the Standalone Form](#using-the-standalone-form)
    * [Notes](#notes)
  * [Configuration](#configuration)

# Example Applications

There are three types of example solutions in the `examples` directory of this repository:
- _Standalone Form Example_ uses the provided `WinForms.OAuthWebViewForm` form in a console application.
- _WinForms Example_ uses the `WinForms.OAuthWebView` control in a Windows Forms app with a custom UI.
- _WPF Example_ uses the `Wpf.OAuthWebView` control in a WPF app with a custom UI.

The `examples` directory contains different subdirectories:
- _v1-net31_ uses the legacy V1 version, installed via a nuget package reference.
- _v2-net472_ uses the new v2 version, installed via an internal project reference, using .Net Framework 4.7.2 (i.e. net472) as Target Framework.
- _v2-net8_ uses the new v2 version, installed via an internal project reference, using .Net 8.0 Windows (i.e. net8.0-windows) as Target Framework.

# Getting Started

## Prerequisites

This library uses the Microsoft Edge WebView2 control. In order for this to function, the [Chromium-based version of Microsoft Edge](https://www.microsoft.com/en-us/edge) or the [WebView2 runtime](https://docs.microsoft.com/en-us/microsoft-edge/webview2/concepts/distribution) must already be installed on the user's machine.

## Installation

### New v2 version

***Note:*** The new version supports the OAuth PKCE Grant Flow and the OAuth Implicit Grant Flow. **Only source is available in v2**. **Use BeginOAuthGrant() to start the PKCE Grant Flow or the Implicit Grant Flow**.

If you're building from source, reference your version of `GenesysCloudOAuthWebView.Core.dll` in your project, as well as either `GenesysCloudOAuthWebView.WinForms.dll` or `GenesysCloudOAuthWebView.Wpf.dll`, depending on the UI framework you are using. Finally, reference the [Microsoft Edge WebView2 SDK](https://docs.microsoft.com/en-us/microsoft-edge/webview2/), `Microsoft.Web.WebView2`.

### Legacy v1 version

***Note:*** The legacy version only supports the OAuth Implicit Grant Flow. Source and nuget package are available in v1. Use BeginImplicitGrant() to start the Implicit Grant Flow.

Install using NuGet:
```
install-package GenesysCloudOAuthWebView
```

If you're building from source or otherwise not using nuget, reference your version of `GenesysCloudOAuthWebView.Core.dll` in your project, as well as either `GenesysCloudOAuthWebView.WinForms.dll` or `GenesysCloudOAuthWebView.Wpf.dll`, depending on the UI framework you are using. Finally, reference the [Microsoft Edge WebView2 SDK](https://docs.microsoft.com/en-us/microsoft-edge/webview2/), `Microsoft.Web.WebView2`.

The legacy V1 source code remains available in this repository under the `v1_legacy_src` directory.


## Usage

### Creating an Instance

There are two versions of the web view control, depending if you are using Windows Forms or WPF. For Windows Forms apps, use `GenesysCloudOAuthWebView.WinForms.OAuthWebView`. For WPF apps, use `GenesysCloudOAuthWebView.Wpf.OAuthWebView`.

To create just the web view control, use UI tools to add the control, or create it in code:

```csharp
using GenesysCloudOAuthWebView.Wpf;

// ... //

var oAuthWebView = new OAuthWebView();
```

### Configuring the Control

#### Environment

The browser control will use the mypurecloud.com environment by default. If the user should log in to an org in another region, simply specify that region in the config's `Environment` property prior to initiating the login flow. There is no validation on the values, but should be a valid Genesys Cloud base URL, such as "mypurecloud.com", "mypurecloud.com.au", "mypurecloud.ie", "mypurecloud.jp", "usw2.pure.cloud", "cac1.pure.cloud", etc. If additional regions are added, simply use the new URL in the same partial format.

```csharp
var oAuthWebView = new OAuthWebView();
oAuthWebView.Config.Environment = "usw2.pure.cloud";
```

#### Required Parameters

A client ID and redirect URI must be defined to use the control successfully. Before initiating the login flow, set the config's `ClientId` and `RedirectUri` properties. For more information on creating an OAuth client and authorizing your redirect URI, see [this Developer Center page](https://developer.genesys.cloud/api/rest/authorization/use-implicit-grant#get-the-oauth-client-information).

```csharp
var oAuthWebView = new OAuthWebView();
oAuthWebView.Config.ClientId = "babbc081-0761-4f16-8f56-071aa402ebcb";
oAuthWebView.Config.RedirectUriIsFake = true;
oAuthWebView.Config.RedirectUri = "http://localhost:8080";
```

Reference the [Configuration](#configuration) section below for a description of each parameter, as well as additional configuration parameters.

### Starting the PKCE Grant Flow or the Implicit Grant Flow

To start the oauth grant flow, invoke the `BeginOAuthGrant()` method. The `Authenticated` event will be raised when the flow has completed.  

The choice of oauth grant flow is controlled using the config's `IsPKCEGrant` property.  
If the config's `IsPKCEGrant` property is set to `true` (default value), a PKCE Grant Flow will be started.  
If the config's `IsPKCEGrant` property is set to `false`, an Implicit Grant Flow will be started.

```csharp
// PKCE Grant Flow
var oAuthWebView = new OAuthWebView();
oAuthWebView.Config.ClientId = "babbc081-0761-4f16-8f56-071aa402ebcb";
oAuthWebView.Config.RedirectUriIsFake = true;
oAuthWebView.Config.RedirectUri = "http://localhost:8080";
oAuthWebView.Config.IsPKCEGrant = true; // default value - starts an OAuth PKCE Grant Flow
```

```csharp
// Implicit Grant Flow
var oAuthWebView = new OAuthWebView();
oAuthWebView.Config.ClientId = "babbc081-0761-4f16-8f56-071aa402ebcb";
oAuthWebView.Config.RedirectUriIsFake = true;
oAuthWebView.Config.RedirectUri = "http://localhost:8080";
oAuthWebView.Config.IsPKCEGrant = false; //  starts an OAuth Implicit Grant Flow
```

### Disposing the Control

When finished with the control, simply dispose of it by calling its `Dispose()` method or by letting .NET garbage collect it.

## Using the Standalone Form

The easiest method of use for this control is to use the `OAuthWebViewForm` form to handle authentication with just a few lines of code. This example will instiantiate and configure the form, initiate the login process, and log the result:

```csharp
// Create form
var form = new OAuthWebViewForm();

// Set settings
form.oAuthWebView.Config.ClientId = "babbc081-0761-4f16-8f56-071aa402ebcb";
form.oAuthWebView.Config.RedirectUriIsFake = true;
form.oAuthWebView.Config.IsPKCEGrant = true;
form.oAuthWebView.Config.RedirectUri = "http://localhost:8080";

// Open it
var result = form.ShowDialog();

Console.WriteLine($"Result: {result}");
Console.WriteLine($"AccessToken: {form.oAuthWebView.AccessToken}");
```

### Notes

* The form has overridden the methods `Show()`, `Show(IWin32Window)`, `ShowDialog()`, and `ShowDialog(IWin32Window)` to validate the configuration (client ID and redirect URI properties must be set), begin the implicit grant flow, and set the dialog result based on if there is an access token (`return string.IsNullOrEmpty(oAuthWebView.AccessToken) ? DialogResult.Cancel : DialogResult.OK;`).
* The form does not proxy properties or events from the browser. The browser object can be directly accessed via `OAuthWebViewForm.oAuthWebView`.
* When using this in a console application, remember that the application must be decorated with [[STAThread]](https://msdn.microsoft.com/en-us/library/system.stathreadattribute(v=vs.110).aspx) to interact with UI components.
* The Show and ShowDialog methods will log all exceptions to the console and rethrow them. Handle the custom exception type `OAuthSettingsValidationException` to identify validation errors.

## Configuration

The following properties on the `OAuthWebView`'s `Config` object should be configured before invoking an OAuth flow:

* `ClientId` - The Client ID (aka Application ID) of the [OAuth client](https://help.mypurecloud.com/articles/create-an-oauth-client/)
* `RedirectUri` - The redirect URI configured for the OAuth client
* `RedirectUriIsFake` - If set to `true`, the control will hide itself upon successfully retrieving an auth token.  This exists due to the non-web nature of a .NET app -- there is not necessarially a webpage to redirect to. If a fake URL is used in the configuration (e.g. http://notarealserver/), setting this property to true prevents the user from seeing errors in the browser related to being unable to resolve the address. This setting defaults to `false`.
* `IsPKCEGrant` - Set to `true` (default value) for an OAuth PKCE Grant flow. Set to `false` for an OAuth Implicit Grant flow.
* `Proxy` - Optionally set a System.Net's WebProx, used in the second step of the OAuth PKCE Grant flow (i.e. HTTP Request, sent using System.Net.Http's HttpClient, to obtain a token from the retrieved code).
* `ForceLoginPrompt` - If set to `true`, the user will be prompted to enter credentials at the Genesys Cloud login screen and any remembered sessions (auth cookies) will be ignored.
* `State` - An abitrary string used to associate a login request with a login response.
* `Org` - The organization name that would normally be used when logging in.
* `Provider` - The authentication provider to log in with. For a list of valid values, [see this page on the Developer Center](https://developer.genesys.cloud/api/rest/authorization/additional-parameters#specify-login-provider).

***Note:*** Org must be set if Provider is set and likewise, Provider must be set if Org is set.

The following events may be useful for the consuming application:

* `ExceptionEncountered` - Raised when an exception is encountered during an OAuth flow
* `Authenticated` - Raised when an OAuth flow has completed and the auth token has been retrieved

In addition to the properties and events defined above, the properties, events, and methods of the underlying WebView2 control are available.