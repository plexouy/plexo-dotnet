using System.Collections.Generic;
using System.Threading.Tasks;
using Plexo.Models;

namespace Plexo
{
    public interface IPlexoClient
    {
        // Commerces
        Task<ServerResponse<Commerce>> AddCommerceAsync(CommerceRequest commerce);

        Task<ServerResponse<IssuerData>> AddIssuerCommerceAsync(IssuerData commerce);
        Task<ServerResponse> DeleteCommerceAsync(CommerceIdRequest commerce);

        Task<ServerResponse> DeleteIssuerCommerceAsync(CommerceIssuerIdRequest commerce);
        Task<ServerResponse<List<IssuerData>>> GetCommerceIssuersAsync(CommerceIdRequest commerce);

        Task<ServerResponse<List<Commerce>>> GetCommercesAsync();
        Task<ServerResponse<Commerce>> ModifyCommerceAsync(CommerceModifyRequest commerce);

        Task<ServerResponse> SetDefaultCommerceAsync(CommerceIdRequest commerce);

        // Sessions
        Task<ServerResponse<Session>> AuthorizeAsync(Authorization authorization);
        Task<ServerResponse<Session>> ExpressCheckoutAsync(ExpressCheckoutRequest expressCheckout);

        // Transactions
        Task<ServerResponse<Transaction>> PurchaseAsync(PaymentRequest payment);
        Task<ServerResponse<Transaction>> CancelAsync(CancelRequest cancel);
        Task<ServerResponse<Transaction>> CodeActionAsync(CodeRequest request);
        Task<ServerResponse<Transaction>> EndReserveAsync(Reserve reserve);
        Task<ServerResponse<string>> ObtainCsvTransactionsAsync(TransactionQuery query);
        Task<ServerResponse<TransactionCursor>> ObtainTransactionsAsync(TransactionQuery query);
        Task<ServerResponse<Transaction>> StartReserveAsync(ReserveRequest payment);
        Task<ServerResponse<Transaction>> StatusAsync(Reference payment);
        Task<ServerResponse<Transaction>> RefundAsync(RefundRequest payment);

        // Instruments
        Task<ServerResponse<List<PaymentInstrument>>> GetInstrumentsAsync(AuthorizationInfo info);
        Task<ServerResponse> DeleteInstrumentAsync(DeleteInstrumentRequest info);
        Task<ServerResponse<PaymentInstrument>> CreateBankInstrumentAsync(CreateBankInstrumentRequest request);
        Task<ServerResponse<ExternalPaymentInstrument>> CreateInstrumentAsync(CreateExternalInstrumentRequest request);
        
        // Issuers
        Task<ServerResponse<List<IssuerInfo>>> GetSupportedIssuersAsync();
        Task<ServerResponse<List<IssuerPaymentProcessor>>> GetSupportedPaymentProcessorsAsync();

        // Utils
        Task<ServerResponse<IntrumentCallback>> UnwrapInstrumentCallbackAsync(ServerSignedCallback<IntrumentCallback> serverSignedInstrumentCallback);
        Task<ServerResponse<TransactionCallback>> UnwrapTransactionCallbackAsync(ServerSignedCallback<TransactionCallback> serverSignedTransactionCallback);
        Task<ClientSignedResponse> SignInstrumentCallback(ServerResponse<IntrumentCallback> serverResponse);
        Task<ClientSignedResponse> SignTransactionCallback(ServerResponse<TransactionCallback> serverResponse);


        // V2 Issuers
        Task<PlexoResponse<IEnumerable<PaymentIssuerDto>>> GetPaymentIssuersAsync();
    }
}
