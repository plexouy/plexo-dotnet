using Plexo.Config;
using System;
using System.Collections.Generic;

namespace Plexo
{
    public static class Settings
    {
        public static Uri GatewayUrl { get; private set; }
        public static string ClientName { get; private set; }
        public static string CertificateName { get; private set; }
        public static string CertificatePassword { get; private set; }

        public static string CertificatePath { get; private set; }                

        internal static void Set(PlexoClientSettings plexoSettings)
        {
            GatewayUrl = new Uri(FormatGatewayUrl(plexoSettings.GatewayUrl));
            ClientName = plexoSettings.ClientName;
            CertificateName = plexoSettings.CertificateName;
            CertificatePassword = plexoSettings.CertificatePassword;
            CertificatePath = plexoSettings.CertificatePath;
        }

        internal static string FormatGatewayUrl(string url)
        {
            return url[url.Length - 1] == '/' ? url : url + "/";
        }
    }
}
