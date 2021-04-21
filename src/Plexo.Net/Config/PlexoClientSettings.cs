using System.Collections.Generic;

namespace Plexo.Config
{
    public class PlexoClientSettings
    {
        public string GatewayUrl { get; set; }
        public string ClientName { get; set; }
        public string CertificateName { get; set; }
        public string CertificatePassword { get; set; }        
        public string CertificatePath { get; set; }
    }
}
