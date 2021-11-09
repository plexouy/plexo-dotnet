using System;
using System.Security.Cryptography.X509Certificates;

namespace Plexo.Net.Helpers.Certificates
{
    public class CertificateHelperFactory
    {
        private static CertificateHelper _instance;
        public static CertificateHelper Instance => _instance ?? (_instance = new CertificateHelper());

        public static void Initialize(X509Certificate2 cert)
        {
            _instance = new CertificateHelper(cert);
        }
    }
}
