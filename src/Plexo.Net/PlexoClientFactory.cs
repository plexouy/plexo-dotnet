using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Plexo.Config;
using Plexo.Models;

namespace Plexo
{
    public class PlexoClientFactory
    {
        private static Dictionary<string, PlexoClient> _instance = new Dictionary<string, PlexoClient>();
        public static Dictionary<string, PlexoClient> Instance => _instance;

        public PlexoClientFactory(PlexoClientSettings plexoClientSettings, HttpClient httpClient = null)
        {
            Init(plexoClientSettings, httpClient);
        }

        public void Init(PlexoClientSettings plexoClientSettings, HttpClient httpClient = null)
        {
            lock (_instance)
            {
                var clients = new List<ClientCertificateSettings>();
                if (plexoClientSettings.Clients is not null && plexoClientSettings.Clients.Any())
                {
                    clients.AddRange(plexoClientSettings.Clients);
                }
                else
                {
                    clients.Add(
                        new ClientCertificateSettings
                        {
                            ClientName = plexoClientSettings.ClientName,
                            CertificateName = plexoClientSettings.CertificateName,
                            CertificatePassword = plexoClientSettings.CertificatePassword,
                            CertificatePath = plexoClientSettings.CertificatePath
                        });
                }

                foreach (var client in clients)
                {
                    var plexoClient = new PlexoClient(client.ClientName, plexoClientSettings, httpClient);

                    _instance ??= new Dictionary<string, PlexoClient>();

                    if (_instance.ContainsKey(client.ClientName))
                    {
                        _instance.Remove(client.ClientName);
                    }

                    _instance.Add(client.ClientName, plexoClient);
                }
            }
        }

        public PlexoClient GetClient(string clientName)
        {
            lock (_instance)
            {
                if (!_instance.ContainsKey(clientName))
                {
                    throw new ConfigurationException(("en", $"The request client '{clientName}' was not found"),
                        ("es", $"El cliente '{clientName}' solicitado no existe"));
                }
                return _instance[clientName];
            }
        }
    }
}
