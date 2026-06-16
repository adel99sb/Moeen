using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Abstractions;

public interface IDashboardErrorFormatter
{
    string FromException(Exception exception, string operationName);
    string FromResponse(GeneralResponse? response, string fallbackMessage);
    string FromValidation(string message);
}
