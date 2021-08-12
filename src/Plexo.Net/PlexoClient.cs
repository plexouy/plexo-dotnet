using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Plexo.Net.Helpers.Certificates;
using Plexo.Net.Helpers.Signatures;
using Plexo.Models;
using Plexo.Config;
using Authorization = Plexo.Models.Authorization;
using Transaction = Plexo.Models.Transaction;

namespace Plexo
{
    public class PlexoClient : IPlexoClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlexoClient> _logger;

        public PlexoClient(IOptions<PlexoClientSettings> plexoClientSettings, ILogger<PlexoClient> logger = null)
        {
            if (plexoClientSettings is null)
            {
                throw new ArgumentNullException(nameof(plexoClientSettings));
            }

            _httpClient = new HttpClient();
            _logger = logger;

            Settings.Set(plexoClientSettings.Value);

            _httpClient.BaseAddress = Settings.GatewayUrl;
        }

        public async Task<ServerResponse<Session>> AuthorizeAsync(Authorization authorization)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(authorization);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            ServerSignedResponse<Session> signedServerResponse;

            // Post async signed request as ByteArrayContent
            using (var request = new HttpRequestMessage(HttpMethod.Post, "Auth"))
            {
                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    signedServerResponse = serializer.Deserialize<ServerSignedResponse<Session>>(jsonReader);
                }
            }

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<List<IssuerInfo>>> GetSupportedIssuersAsync()
        {
            // Sign request
            var signedClientRequest = SignClientRequest();

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Issuer", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<List<IssuerInfo>>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<List<Commerce>>> GetCommercesAsync()
        {
            // Sign request
            var signedClientRequest = SignClientRequest();

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Commerce", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<List<Commerce>>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Commerce>> AddCommerceAsync(CommerceRequest commerce)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(commerce);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Commerce/Add", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<Commerce>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Commerce>> ModifyCommerceAsync(CommerceModifyRequest commerce)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(commerce);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Commerce/Modify", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<Commerce>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse> DeleteCommerceAsync(CommerceIdRequest commerce)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(commerce);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Commerce/Delete", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse> SetDefaultCommerceAsync(CommerceIdRequest commerce)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(commerce);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Commerce/SetDefault", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<List<IssuerData>>> GetCommerceIssuersAsync(CommerceIdRequest commerce)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(commerce);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Commerce/Issuer", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<List<IssuerData>>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<IssuerData>> AddIssuerCommerceAsync(IssuerData commerce)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(commerce);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Commerce/Issuer/Add", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<IssuerData>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse> DeleteIssuerCommerceAsync(CommerceIssuerIdRequest commerce)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(commerce);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Commerce/Issuer/Delete", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<TransactionCursor>> ObtainTransactionsAsync(TransactionQuery query)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(query);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Transactions", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<TransactionCursor>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<string>> ObtainCsvTransactionsAsync(TransactionQuery query)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(query);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            ServerSignedResponse<string> signedServerResponse;

            // Post async signed request as ByteArrayContent
            using (var request = new HttpRequestMessage(HttpMethod.Post, "Transactions/CSV"))
            {
                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    signedServerResponse = serializer.Deserialize<ServerSignedResponse<string>>(jsonReader);
                }
            }

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Transaction>> CodeActionAsync(CodeRequest request)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(request);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Code", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<Transaction>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Transaction>> PurchaseAsync(PaymentRequest payment)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(payment);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            ServerSignedResponse<Transaction> signedServerResponse;

            // Post async signed request as ByteArrayContent
            using (var request = new HttpRequestMessage(HttpMethod.Post, "Operation/Purchase"))
            {
                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    signedServerResponse = serializer.Deserialize<ServerSignedResponse<Transaction>>(jsonReader);
                }
            }

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Transaction>> CancelAsync(CancelRequest cancel)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(cancel);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            ServerSignedResponse<Transaction> signedServerResponse;

            // Post async signed request as ByteArrayContent
            using (var request = new HttpRequestMessage(HttpMethod.Post, "Operation/Cancel"))
            {
                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    signedServerResponse = serializer.Deserialize<ServerSignedResponse<Transaction>>(jsonReader);
                }
            }

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Transaction>> RefundAsync(RefundRequest payment)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(payment);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            ServerSignedResponse<Transaction> signedServerResponse;

            // Post async signed request as ByteArrayContent
            using (var request = new HttpRequestMessage(HttpMethod.Post, "Operation/Refund"))
            {
                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    signedServerResponse = serializer.Deserialize<ServerSignedResponse<Transaction>>(jsonReader);
                }
            }

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Transaction>> StartReserveAsync(ReserveRequest payment)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(payment);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Operation/StartReserve", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<Transaction>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Transaction>> EndReserveAsync(Reserve reserve)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(reserve);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Operation/EndReserve", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<Transaction>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Transaction>> StatusAsync(Reference payment)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(payment);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Operation/Status", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<Transaction>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<List<PaymentInstrument>>> GetInstrumentsAsync(AuthorizationInfo info)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(info);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            ServerSignedResponse<List<PaymentInstrument>> signedServerResponse;

            // Post async signed request as ByteArrayContent
            using (var request = new HttpRequestMessage(HttpMethod.Post, "Instruments"))
            {
                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    signedServerResponse = serializer.Deserialize<ServerSignedResponse<List<PaymentInstrument>>>(jsonReader);
                }
            }

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse> DeleteInstrumentAsync(DeleteInstrumentRequest info)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(info);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            ServerSignedResponse signedServerResponse;

            // Post async signed request as ByteArrayContent
            using (var request = new HttpRequestMessage(HttpMethod.Post, "Instruments/Delete"))
            {
                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    signedServerResponse = serializer.Deserialize<ServerSignedResponse>(jsonReader);
                }
            }

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<PaymentInstrument>> CreateBankInstrumentAsync(CreateBankInstrumentRequest request)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(request);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            // Post async signed request as ByteArrayContent
            var response = await _httpClient.PostAsync("Instruments/Bank", byteContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var signedServerResponse = JsonConvert.DeserializeObject<ServerSignedResponse<PaymentInstrument>>(result);

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<ServerResponse<IntrumentCallback>> UnwrapInstrumentCallbackAsync(
            ServerSignedCallback<IntrumentCallback> serverSignedInstrumentCallback)
        {

            return await UnwrapCallbackAsync(serverSignedInstrumentCallback).ConfigureAwait(false);
        }

        public Task<ClientSignedResponse> SignInstrumentCallback(ServerResponse<IntrumentCallback> serverResponse)
        {
            var cr = new ClientResponse
            {
                ResultCode = serverResponse.ResultCode,
                ErrorMessage = serverResponse.ErrorMessage,
                Client = Settings.ClientName
            };

            return Task.FromResult(
                CertificateHelperFactory.Instance.SignClient<ClientSignedResponse, ClientResponse>(cr.Client, cr));
        }

        public Task<ClientSignedResponse> SignTransactionCallback(ServerResponse<TransactionCallback> serverResponse)
        {
            var cr = new ClientResponse
            {
                ResultCode = serverResponse.ResultCode,
                ErrorMessage = serverResponse.ErrorMessage,
                Client = Settings.ClientName
            };

            return Task.FromResult(
                CertificateHelperFactory.Instance.SignClient<ClientSignedResponse, ClientResponse>(cr.Client, cr));
        }

        private ByteArrayContent SignByteArrayContent(object signedClientRequest)
        {
            var content = JsonConvert.SerializeObject(signedClientRequest);
            var buffer = Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        private async Task<ServerSignedResponse<PublicKeyInfo>> GetServerPublicKeyAsync(string fingerprint)
        {
            // Post async signed request as ByteArrayContent
            var response = await _httpClient.GetAsync($"Key/{fingerprint}").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ServerSignedResponse<PublicKeyInfo>>(result);
        }

        private async Task<SignatureHelper> GetSignatureHelperAsync(string fingerprint, ServerResponse response)
        {
            var c = CertificateHelperFactory.Instance._verifyKeys.FirstOrDefault(a => a.Key == fingerprint).Value;
            if (c == null)
            {
                await CertificateHelperFactory.Instance._serverCertSemaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    var r = await GetServerPublicKeyAsync(fingerprint).ConfigureAwait(false);
                    if (r.Object.Object.ResultCode != ResultCodes.Ok)
                    {
                        throw new FingerprintException(
                            ("en",
                                "Invalid or outdated Fingerprint, server returns: " +
                                (r.Object.Object.ErrorMessage ?? "")),
                            ("es",
                                "Huella invalida o vencida, el servido retorna: " +
                                ((r.Object.Object.I18NErrorMessages.ContainsKey("es")
                                     ? r.Object.Object.I18NErrorMessages["es"]
                                     : r.Object.Object.ErrorMessage) ?? "")));
                    }

                    var cert = new X509Certificate2(Convert.FromBase64String(r.Object.Object.Response.Key));
                    c = new SignatureHelper(cert, false);
                    SignatureHelper verify = null;
                    if (CertificateHelperFactory.Instance._verifyKeys.ContainsKey(r.Object.Fingerprint))
                    {
                        verify = CertificateHelperFactory.Instance._verifyKeys[r.Object.Fingerprint];
                    }
                    else if (r.Object.Fingerprint.Equals(r.Object.Object.Response.Fingerprint,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        verify = c;
                    }

                    if (verify == null)
                    {
                        throw new FingerprintException(("en", "Fingerprint not found: " + r.Object.Fingerprint),
                            ("es", "Huella no encontrada: " + r.Object.Fingerprint));
                    }

                    verify.Verify<ServerSignedResponse<PublicKeyInfo>, ServerResponse<PublicKeyInfo>>(r);
                    if (CertificateHelperFactory.Instance._verifyKeys.ContainsKey(r.Object.Object.Response.Fingerprint))
                    {
                        CertificateHelperFactory.Instance._verifyKeys[r.Object.Object.Response.Fingerprint] = c;
                    }
                    else
                    {
                        CertificateHelperFactory.Instance._verifyKeys.Add(r.Object.Object.Response.Fingerprint, c);
                    }
                    return c;
                }
                finally
                {
                    CertificateHelperFactory.Instance._serverCertSemaphore.Release();
                }
            }

            return c;
        }

        private ClientSignedRequest<T> SignClientRequest<T>(T unsignedRequest)
        {
            var clientRequest = WrapClient(unsignedRequest);
            return CertificateHelperFactory.Instance
                    .SignClient<ClientSignedRequest<T>, ClientRequest<T>>(
                        Settings.ClientName, clientRequest);
        }

        private ClientSignedRequest SignClientRequest()
        {
            var r = new ClientRequest { Client = Settings.ClientName };
            return CertificateHelperFactory.Instance.SignClient<ClientSignedRequest, ClientRequest>(Settings.ClientName, r);
        }

        private ClientRequest<T> WrapClient<T>(T obj)
        {
            return new ClientRequest<T> { Client = Settings.ClientName, Request = obj };
        }


        private async Task<ServerResponse<T>> UnwrapCallbackAsync<T>(ServerSignedCallback<T> resp)
        {
            var response = new ServerResponse<T>();
            try
            {
                var c = await GetSignatureHelperAsync(resp.Object.Fingerprint, response).ConfigureAwait(false);
                var obj = c.Verify<ServerSignedCallback<T>, T>(resp);
                return new ServerResponse<T> { ResultCode = ResultCodes.Ok, Response = obj };
            }
            catch (FingerprintException e)
            {
                _logger?.LogError(e, "Exception trying to unwrap the request");
                response.ErrorMessage = e.Message;
                response.I18NErrorMessages = e.I18NErrorMessages;
                response.ResultCode = e.Code;
                return response;
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e, "Critical Exception trying to unwrap the request");
                response.ErrorMessage = e.Message;
                response.ResultCode = ResultCodes.SystemError;
                return response;
            }
        }

        private async Task<ServerResponse<T>> UnwrapResponseAsync<T>(ServerSignedResponse<T> resp)
        {
            var response = new ServerResponse<T>();

            try
            {
                var c = await GetSignatureHelperAsync(resp.Object.Fingerprint, response).ConfigureAwait(false);
                return c.Verify<ServerSignedResponse<T>, ServerResponse<T>>(resp);
            }
            catch (FingerprintException e)
            {
                _logger?.LogError(e, "Exception trying to unwrap the response");
                response.ErrorMessage = e.Message;
                response.I18NErrorMessages = e.I18NErrorMessages;
                response.ResultCode = e.Code;
                return response;
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e, "Critical Exception trying to unwrap the response");
                response.ErrorMessage = e.Message;
                response.ResultCode = ResultCodes.SystemError;
                return response;
            }
        }

        private async Task<ServerResponse> UnwrapResponseAsync(ServerSignedResponse resp)
        {
            var response = new ServerResponse();
            try
            {
                var c = await GetSignatureHelperAsync(resp.Object.Fingerprint, response).ConfigureAwait(false);
                return c.Verify<ServerSignedResponse, ServerResponse>(resp);
            }

            catch (FingerprintException e)
            {
                _logger?.LogError(e, "Fingerprint Exception trying to unwrap the response");
                response.ErrorMessage = e.Message;
                response.I18NErrorMessages = e.I18NErrorMessages;
                response.ResultCode = e.Code;
                return response;
            }
            catch (Exception e)
            {
                _logger?.LogCritical(e, "Critical Exception trying to unwrap the response");
                response.ErrorMessage = e.Message;
                response.ResultCode = ResultCodes.SystemError;
                return response;
            }
        }

        public async Task<ServerResponse<TransactionCallback>> UnwrapTransactionCallbackAsync(ServerSignedCallback<TransactionCallback> serverSignedTransactionCallback)
        {
            return await UnwrapCallbackAsync(serverSignedTransactionCallback).ConfigureAwait(false);
        }

        public async Task<ServerResponse<Session>> ExpressCheckoutAsync(ExpressCheckoutRequest expressCheckout)
        {
            // Sign request
            var signedClientRequest = SignClientRequest(expressCheckout);

            // Signed request to ByteArrayContent
            var byteContent = SignByteArrayContent(signedClientRequest);

            ServerSignedResponse<Session> signedServerResponse;

            // Post async signed request as ByteArrayContent
            using (var request = new HttpRequestMessage(HttpMethod.Post, "ExpressCheckout"))
            {
                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    signedServerResponse = serializer.Deserialize<ServerSignedResponse<Session>>(jsonReader);
                }
            }

            return await UnwrapResponseAsync(signedServerResponse).ConfigureAwait(false);
        }

        public async Task<PlexoResponse<IEnumerable<PaymentIssuerDto>>> GetPaymentIssuersAsync()
        {

            var response = await _httpClient.GetAsync(Settings.BaseUrl.TrimEnd('/') + "/v1/issuers", HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    return new PlexoResponse<IEnumerable<PaymentIssuerDto>>(true, serializer.Deserialize<IEnumerable<PaymentIssuerDto>>(jsonReader));
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();

                    // Read the response content from a stream
                    return new PlexoResponse<IEnumerable<PaymentIssuerDto>>(false, null, serializer.Deserialize<ProblemDetails>(jsonReader));
                }
            }
            else
            {
                return new PlexoResponse<IEnumerable<PaymentIssuerDto>>(false, null, new ProblemDetails
                {
                    Detail = "An unknown error ocurred on the server",
                    Title = "Client exception",
                    Status = (int)response.StatusCode,
                    Instance = "Plexo-net"
                });
            }
        }


    }
}
