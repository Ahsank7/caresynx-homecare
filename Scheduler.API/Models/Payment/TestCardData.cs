namespace Scheduler.API.Models.Payment
{
    /// <summary>
    /// Test card data for development and testing purposes
    /// These are Stripe's official test card numbers
    /// </summary>
    public static class TestCardData
    {
        public static class Visa
        {
            public const string Number = "4242424242424242";
            public const string ExpiryMonth = "12";
            public const string ExpiryYear = "2025";
            public const string CVC = "123";
            public const string Token = "tok_visa";
        }

        public static class VisaDebit
        {
            public const string Number = "4000056655665556";
            public const string ExpiryMonth = "12";
            public const string ExpiryYear = "2025";
            public const string CVC = "123";
            public const string Token = "tok_visa_debit";
        }

        public static class Mastercard
        {
            public const string Number = "5555555555554444";
            public const string ExpiryMonth = "12";
            public const string ExpiryYear = "2025";
            public const string CVC = "123";
            public const string Token = "tok_mastercard";
        }

        public static class AmericanExpress
        {
            public const string Number = "378282246310005";
            public const string ExpiryMonth = "12";
            public const string ExpiryYear = "2025";
            public const string CVC = "1234";
            public const string Token = "tok_amex";
        }

        public static class Discover
        {
            public const string Number = "6011111111111117";
            public const string ExpiryMonth = "12";
            public const string ExpiryYear = "2025";
            public const string CVC = "123";
            public const string Token = "tok_discover";
        }

        public static class DeclinedCards
        {
            public static class GenericDecline
            {
                public const string Number = "4000000000000002";
                public const string ExpiryMonth = "12";
                public const string ExpiryYear = "2025";
                public const string CVC = "123";
                public const string Token = "tok_chargeDeclined";
            }

            public static class InsufficientFunds
            {
                public const string Number = "4000000000009995";
                public const string ExpiryMonth = "12";
                public const string ExpiryYear = "2025";
                public const string CVC = "123";
                public const string Token = "tok_chargeDeclinedInsufficientFunds";
            }

            public static class ExpiredCard
            {
                public const string Number = "4000000000000069";
                public const string ExpiryMonth = "12";
                public const string ExpiryYear = "2020";
                public const string CVC = "123";
                public const string Token = "tok_chargeDeclinedExpiredCard";
            }

            public static class IncorrectCVC
            {
                public const string Number = "4000000000000127";
                public const string ExpiryMonth = "12";
                public const string ExpiryYear = "2025";
                public const string CVC = "999";
                public const string Token = "tok_chargeDeclinedIncorrectCvc";
            }
        }

        /// <summary>
        /// Get test card data by card type
        /// </summary>
        public static TestCard GetTestCard(string cardType)
        {
            return cardType.ToLower() switch
            {
                "visa" => new TestCard
                {
                    Number = Visa.Number,
                    ExpiryMonth = Visa.ExpiryMonth,
                    ExpiryYear = Visa.ExpiryYear,
                    CVC = Visa.CVC,
                    Token = Visa.Token,
                    CardType = "Visa"
                },
                "visadebit" => new TestCard
                {
                    Number = VisaDebit.Number,
                    ExpiryMonth = VisaDebit.ExpiryMonth,
                    ExpiryYear = VisaDebit.ExpiryYear,
                    CVC = VisaDebit.CVC,
                    Token = VisaDebit.Token,
                    CardType = "Visa Debit"
                },
                "mastercard" => new TestCard
                {
                    Number = Mastercard.Number,
                    ExpiryMonth = Mastercard.ExpiryMonth,
                    ExpiryYear = Mastercard.ExpiryYear,
                    CVC = Mastercard.CVC,
                    Token = Mastercard.Token,
                    CardType = "Mastercard"
                },
                "amex" => new TestCard
                {
                    Number = AmericanExpress.Number,
                    ExpiryMonth = AmericanExpress.ExpiryMonth,
                    ExpiryYear = AmericanExpress.ExpiryYear,
                    CVC = AmericanExpress.CVC,
                    Token = AmericanExpress.Token,
                    CardType = "American Express"
                },
                "discover" => new TestCard
                {
                    Number = Discover.Number,
                    ExpiryMonth = Discover.ExpiryMonth,
                    ExpiryYear = Discover.ExpiryYear,
                    CVC = Discover.CVC,
                    Token = Discover.Token,
                    CardType = "Discover"
                },
                "declined" => new TestCard
                {
                    Number = DeclinedCards.GenericDecline.Number,
                    ExpiryMonth = DeclinedCards.GenericDecline.ExpiryMonth,
                    ExpiryYear = DeclinedCards.GenericDecline.ExpiryYear,
                    CVC = DeclinedCards.GenericDecline.CVC,
                    Token = DeclinedCards.GenericDecline.Token,
                    CardType = "Declined Card"
                },
                "insufficient" => new TestCard
                {
                    Number = DeclinedCards.InsufficientFunds.Number,
                    ExpiryMonth = DeclinedCards.InsufficientFunds.ExpiryMonth,
                    ExpiryYear = DeclinedCards.InsufficientFunds.ExpiryYear,
                    CVC = DeclinedCards.InsufficientFunds.CVC,
                    Token = DeclinedCards.InsufficientFunds.Token,
                    CardType = "Insufficient Funds"
                },
                "expired" => new TestCard
                {
                    Number = DeclinedCards.ExpiredCard.Number,
                    ExpiryMonth = DeclinedCards.ExpiredCard.ExpiryMonth,
                    ExpiryYear = DeclinedCards.ExpiredCard.ExpiryYear,
                    CVC = DeclinedCards.ExpiredCard.CVC,
                    Token = DeclinedCards.ExpiredCard.Token,
                    CardType = "Expired Card"
                },
                "wrongcvc" => new TestCard
                {
                    Number = DeclinedCards.IncorrectCVC.Number,
                    ExpiryMonth = DeclinedCards.IncorrectCVC.ExpiryMonth,
                    ExpiryYear = DeclinedCards.IncorrectCVC.ExpiryYear,
                    CVC = DeclinedCards.IncorrectCVC.CVC,
                    Token = DeclinedCards.IncorrectCVC.Token,
                    CardType = "Wrong CVC"
                },
                _ => new TestCard
                {
                    Number = Visa.Number,
                    ExpiryMonth = Visa.ExpiryMonth,
                    ExpiryYear = Visa.ExpiryYear,
                    CVC = Visa.CVC,
                    Token = Visa.Token,
                    CardType = "Visa (Default)"
                }
            };
        }
    }

    public class TestCard
    {
        public string Number { get; set; } = string.Empty;
        public string ExpiryMonth { get; set; } = string.Empty;
        public string ExpiryYear { get; set; } = string.Empty;
        public string CVC { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
    }
} 