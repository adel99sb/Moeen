using Microsoft.JSInterop;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations;

public sealed class DashboardErrorFormatter : IDashboardErrorFormatter
{
    private readonly ILogger<DashboardErrorFormatter> _logger;

    public DashboardErrorFormatter(ILogger<DashboardErrorFormatter> logger)
    {
        _logger = logger;
    }

    public string FromException(Exception exception, string operationName)
    {
        _logger.LogError(exception, "Dashboard operation failed. Operation={OperationName}, Error={Error}", operationName, exception.Message);

        return exception switch
        {
            HttpRequestException => $"فشل الاتصال بالباك اند أثناء: {operationName}. التفاصيل: {exception.Message}",
            TaskCanceledException => $"انتهت مهلة الطلب أثناء: {operationName}. جرّب التحديث وافحص لوق الـ API.",
            JSException => $"تعذر الوصول إلى خدمات المتصفح أثناء: {operationName}. التفاصيل: {exception.Message}",
            InvalidOperationException => $"تعذر تنفيذ العملية: {operationName}. التفاصيل: {exception.Message}",
            _ => $"حدث خطأ أثناء: {operationName}. التفاصيل: {exception.Message}"
        };
    }

    public string FromResponse(GeneralResponse? response, string fallbackMessage)
    {
        if (response == null)
            return fallbackMessage;

        if (!string.IsNullOrWhiteSpace(response.Message))
            return response.StatusCode > 0 ? $"{response.Message} (StatusCode: {response.StatusCode})" : response.Message;

        return response.StatusCode > 0 ? $"{fallbackMessage} (StatusCode: {response.StatusCode})" : fallbackMessage;
    }

    public string FromValidation(string message)
        => string.IsNullOrWhiteSpace(message) ? "يرجى التحقق من البيانات المدخلة." : message;
}
