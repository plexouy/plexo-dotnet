# Plexo.Net

The official Plexo .NET library, supporting Standard 2.0+, .NET Core 2.0+, and .NET Framework 4.6.1+.

## Installation

### Using the Package Manager

```bash
Install-Package Plexo.Net
```

### Using the .NET Core command-line interface (CLI) tools

```bash
dotnet add package Plexo.Net
```

### From Visual Studio

Open the <code>Solution Explorer</code>.
Right-click on a project within your solution.
Click on <code>Manage NuGet Packages....</code>
Click on the <code>Browse</code> tab and search for "Plexo.Net".
Click on the <code>Plexo.Net</code> package, select the appropriate version and click Install.

## Usage

### Configuration

We provide an extension that you can easily add to your startup where your appsettings configuration will be automaticaly bind with the specified section.

```bash
services.AddPlexoClient(options => Configuration.GetSection("PlexoSettings").Bind(options));
```

<code>PlexoSettings</code> can be replaced with any custom name.

Optionally you can also set each property value manually.

```bash
services.AddPlexoClient(options =>
          {
              options.ClientName = "MyCommerce";
              options.CertificateName = "mycommerce_production";
              options.CertificatePassword = "R4ndomStr0ngPa$$word";
              options.GatewayUrl = "https://pagos.plexo.com.uy:4043/SecurePaymentGateway.svc";
          });
```

#### Required fields

- <code>ClientName</code>
  Name of the client to configure
- <code>CertificateName</code>
  Name of the certificate on the certificate store
- <code>CertificatePassword</code>
  Password of the certicate
- <code>GatewayUrl</code>
  Gateway endpoint for the selected environment _(refer to available gateway endpoints)_

#### Optional fields

- <code>CertificatePath</code>
  Custom path to locate the certificate, must be relative to the project's root

#### Available gateway endpoints

- <code>Testing</code>
  https://testing.plexo.com.uy:4043/SecurePaymentGateway.svc
- <code>Production</code>
  https://pagos.plexo.com.uy:4043/SecurePaymentGateway.svc

#### Certificates

Plexo requires the use certificate signing on requests and responses, with is handled by the library for you.

```
Contact our support team and they will provide you with a PKCS#1 certificate (PFX) for the requested environment and assistance if required.
```

#### Automatic certificate provision

The current user personal store must contain a certificate with the value of <code>CertificateName</code>.
As fallback the library will attempt to locate the certificate on a custom file path relative to the application's base path.

Windows

```
CurrentUser\My
```

Linux

```
/{currentUser}/.dotnet/corefx/cryptography/x509stores/my
/{currentUser}/.dotnet/corefx/cryptography/x509stores/root
```

Fallback (relative path)

```
Certificate/
```

## Current limitations

- No built-in support for automatic retries.
- There's no support for multiple clients certificates on the same application.
