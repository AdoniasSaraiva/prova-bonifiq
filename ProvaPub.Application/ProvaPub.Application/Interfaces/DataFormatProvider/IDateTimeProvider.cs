namespace ProvaPub.Application.Interfaces.DataFormatProvider
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
