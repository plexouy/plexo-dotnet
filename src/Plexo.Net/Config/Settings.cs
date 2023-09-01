using System;
using System.Collections.Generic;
using System.Linq;
using Plexo.Config;

namespace Plexo
{
    public static class Settings
    {
        private const string DEFAULT_BASE_URL = "https://api.plexo.com.uy";
        public static string BaseUrl { get; private set; }
        public static Uri GatewayUrl { get; private set; }
        public static IEnumerable<ClientCertificateSettings>? Clients { get; private set; }

        internal static void Set(PlexoClientSettings plexoSettings)
        {
            GatewayUrl = new Uri(FormatGatewayUrl(plexoSettings.GatewayUrl));
            BaseUrl = plexoSettings.BaseUrl ?? DEFAULT_BASE_URL;

            if (plexoSettings.Clients is not null && plexoSettings.Clients.Any())
            {
                Clients = plexoSettings.Clients;
            }
            else
            {
                var clients = new List<ClientCertificateSettings>
                {
                    new ClientCertificateSettings
                    {
                        ClientName = plexoSettings.ClientName,
                        CertificateName = plexoSettings.CertificateName,
                        CertificatePassword = plexoSettings.CertificatePassword,
                        CertificatePath = plexoSettings.CertificatePath
                    }
                };

                Clients = clients;
            }
        }

        internal static string FormatGatewayUrl(string url)
        {
            return url[url.Length - 1] == '/' ? url : url + "/";
        }
    }
}
