using System.Text.RegularExpressions;

namespace Scheduler.API.Common.Extensions
{
    public static class StringExtensions
    {
        public static string CheckIfNullThenDefault(this string str, string name)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : $"{name.SplitWords()}: {str}";
        }

        public static string RemoveEmptyLines(this string str)
        {
            return Regex.Replace(str, @"^(\s|\t)+$[\r\n]*", string.Empty, RegexOptions.Multiline);
        }

        public static string SplitWords(this string str)
        {
            var words = Regex.Split(str, "(?<=\\p{Ll})(?=\\p{Lu})|(?<=\\p{L})(?=\\p{Lu}\\p{Ll})");

            var header = string.Join(" ", words);

            return header;
        }

        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            TEnum result;
            return Enum.TryParse<TEnum>(value, true, out result) ? result : defaultValue;
        }

        public static string GetStripeTestToken(this string cardNumber)
        {
            // Stripe test tokens for different card scenarios
            // These are safe to use in test mode
            var lastFourDigits = cardNumber?.Length >= 4 ? cardNumber.Substring(cardNumber.Length - 4) : "0000";

            switch (lastFourDigits)
            {
                case "0000": // Generic success
                    return "tok_visa";
                case "1111": // Visa success
                    return "tok_visa";
                case "2222": // Visa debit success
                    return "tok_visa_debit";
                case "3333": // Mastercard success
                    return "tok_mastercard";
                case "4444": // American Express success
                    return "tok_amex";
                case "5555": // Discover success
                    return "tok_discover";
                case "6666": // Declined card
                    return "tok_chargeDeclined";
                case "7777": // Insufficient funds
                    return "tok_chargeDeclinedInsufficientFunds";
                case "8888": // Expired card
                    return "tok_chargeDeclinedExpiredCard";
                case "9999": // Incorrect CVC
                    return "tok_chargeDeclinedIncorrectCvc";
                default:
                    return "tok_visa"; // Default to successful Visa
            }
        }

    }
}
