using ProvaPub.Application.Interfaces.DataFormatProvider;

namespace ProvaPub.Application.Services.DataFormatProvider
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
