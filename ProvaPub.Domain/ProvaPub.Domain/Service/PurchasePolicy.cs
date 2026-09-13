namespace ProvaPub.Domain.Service
{
    public class PurchasePolicy
    {
        public bool CanPurchase(
            Customer customer,
            decimal purchaseValue,
            bool haveBoughtBefore,
            bool hasPurchasedThisMonth,
            DateTime currentDateTime)
        {
            if (hasPurchasedThisMonth)
                return false;

            if (!haveBoughtBefore && purchaseValue > 100)
                return false;

            if (!IsBusinessDay(currentDateTime) || !IsBusinessHour(currentDateTime))
                return false;

            return true;
        }

        private bool IsBusinessDay(DateTime dateTime)
        {
            return dateTime.DayOfWeek != DayOfWeek.Saturday &&
                   dateTime.DayOfWeek != DayOfWeek.Sunday;
        }

        private bool IsBusinessHour(DateTime dateTime)
        {
            var time = dateTime.TimeOfDay;

            return time >= TimeSpan.FromHours(8) &&
                   time <= TimeSpan.FromHours(18);
        }
    }
}
