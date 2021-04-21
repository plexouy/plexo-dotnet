using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Plexo.Client.SDK.Helpers.OperatingSystem;
using Plexo.Client.SDK.Helpers.Signatures;
using Plexo.Client.SDK.Models;
using Plexo.Client.SDK.Models.Exceptions;

namespace Plexo.Client.SDK.Helpers.Certificates
{
    public class CertificateHelper
    {
        internal Dictionary<string, SignatureHelper> _verifyKeys = new Dictionary<string, SignatureHelper>();
        internal SemaphoreSlim _serverCertSemaphore = new SemaphoreSlim(1);

        public CertificateHelper()
        {
            if (string.IsNullOrWhiteSpace(Settings.ClientName))
            {
                throw new ConfigurationException(("en", "Invalid Client line in configuration"),
                    ("es", "La Linea del cliente en la configuracion es invalida"));
            }

            if (string.IsNullOrWhiteSpace(Settings.CertificateName))
            {
                throw new ConfigurationException(("en", "Invalid certificate name in configuration"),
                    ("es", "La Linea del nombre del certificado cliente en la configuracion es invalida"));
            }

            var x509Certificate = SearchCertificate(Settings.CertificateName, Settings.CertificatePassword, Settings.CertificatePath);
            if (x509Certificate == null)
            {
                throw new CertificateException(
                    ("en",
                        $"Unable to find Certificate '{Settings.CertificateName.Trim()}' in the X509 Store, please make sure that the user using this context has security access to the certificate"),
                    ("es",
                        $"No puedo encontar el certificado '{Settings.CertificateName.Trim()}' el el Store de Certficado, asegurese que el certificado este instalado, y que el usuario que corrar el contexto de la aplicacion tenga permisos para acceder a este"));
            }

            ClientSignKeys.Add(Settings.ClientName.Trim(), new SignatureHelper(x509Certificate, true));
        }

        private Dictionary<string, SignatureHelper> ClientSignKeys { get; } = new Dictionary<string, SignatureHelper>();

        public T SignClient<T, TS>(string clientname, TS obj) where T : SignedObject<TS>, new()
        {
            if (ClientSignKeys.ContainsKey(clientname))
            {
                return ClientSignKeys[clientname].Sign<T, TS>(obj);
            }

            throw new CertificateException(("en", $"Unable to find certificate for client '{clientname}'"),
                ("es", $"No puedo encontrar certificado para el cliente '{clientname}'"));
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private X509Certificate2 SearchCertificate(string certname, string password, string path)
        {
            StoreName[] stores;
            X509Certificate2 localCertificate = null;

            // Get the certificate from the operative system key store
            if (PlatformHelper.IsWindows())
            {
                stores = new StoreName[]
                {
                    StoreName.My, StoreName.TrustedPublisher, StoreName.TrustedPeople, StoreName.Root,
                    StoreName.CertificateAuthority, StoreName.AuthRoot, StoreName.AddressBook
                };
            }
            else if (PlatformHelper.IsLinux())
            {
                stores = new StoreName[]
                {
                    StoreName.Root,
                    StoreName.CertificateAuthority
                };
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            StoreLocation[] locations = { StoreLocation.CurrentUser, StoreLocation.LocalMachine };

            foreach (var location in locations)
            {
                foreach (var s in stores)
                {
                    var store = new X509Store(s, location);
                    store.Open(OpenFlags.ReadOnly);
                    foreach (var m in store.Certificates)
                    {
                        if (m.Subject.IndexOf("CN=" + certname, 0, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        m.Issuer.IndexOf("CN=" + certname, 0, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            store.Close();
                            localCertificate = m;
                        }
                    }

                    store.Close();
                }
            }

            // As fallback use application's filesystem
            if (localCertificate == null)
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ConfigurationException(("en", "A path must be set if certificate is not installed on the system"),
                    ("es", "Se debe establecer una ruta si el certificado no está instalado en el sistema"));
                }

                var pathToCertificate = Path.GetFullPath(Path.Combine(path, $"{certname}.pfx"));

                using (var stream = File.Open(pathToCertificate, FileMode.Open))
                {
                    var certificatePassword = password;

                    var cert = new X509Certificate2(ReadStream(stream), certificatePassword, X509KeyStorageFlags.MachineKeySet);
                    bool result = cert.Verify();
                    return cert;
                }
            }
            return localCertificate;
        }
    }
}
