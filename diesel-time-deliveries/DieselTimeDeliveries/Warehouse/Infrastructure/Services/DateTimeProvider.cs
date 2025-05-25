using RegistR.Attributes.Base;
using Warehouse.Domain.Services;

namespace Warehouse.Infrastructure.Services;

[Register<IDateTimeProvider>]
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;

    public DateTime UtcNow => DateTime.UtcNow;
}