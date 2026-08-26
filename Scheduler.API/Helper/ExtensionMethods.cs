using System.ComponentModel;
using System.Data;

namespace Scheduler.API.Helper
{
    public static class ExtensionMethods
    {
        public static DataTable ToDataTable<T>(this List<T> iList)
        {
            DataTable dataTable = new DataTable();
            PropertyDescriptorCollection propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T));
            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                Type type = propertyDescriptor.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);


                dataTable.Columns.Add(propertyDescriptor.Name, type);
            }
            object[] values = new object[propertyDescriptorCollection.Count];
            foreach (T iListItem in iList)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public static string MaskString(this string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= 2)
                return input; // If string is empty or has only two characters, return as is.

            int middleLength = input.Length - 2; // Number of characters to replace with '*'
            string masked = new string('*', middleLength); // Create string of '*' with required length

            // Concatenate the first character, masked part, and last character.
            return input[0] + masked + input[^1];
        }

        public static string GetCountryFromIBAN(this string iban)
        {
            if (string.IsNullOrEmpty(iban))
                return "US"; // Default to US if no IBAN provided

            // IBAN format: Country Code (2 chars) + Check Digits (2 chars) + Bank Code + Account Number
            // Examples:
            // GB29NWBK60161331926819 -> GB (United Kingdom)
            // DE89370400440532013000 -> DE (Germany)
            // FR1420041010050500013M02606 -> FR (France)
            // US32110000000000000000000000000000 -> US (United States)

            if (iban.Length >= 2)
            {
                var countryCode = iban.Substring(0, 2).ToUpper();

                // Map common IBAN country codes to Stripe supported countries
                switch (countryCode)
                {
                    case "GB": return "GB"; // United Kingdom
                    case "DE": return "DE"; // Germany
                    case "FR": return "FR"; // France
                    case "IT": return "IT"; // Italy
                    case "ES": return "ES"; // Spain
                    case "NL": return "NL"; // Netherlands
                    case "BE": return "BE"; // Belgium
                    case "AT": return "AT"; // Austria
                    case "CH": return "CH"; // Switzerland
                    case "SE": return "SE"; // Sweden
                    case "NO": return "NO"; // Norway
                    case "DK": return "DK"; // Denmark
                    case "FI": return "FI"; // Finland
                    case "PL": return "PL"; // Poland
                    case "CZ": return "CZ"; // Czech Republic
                    case "HU": return "HU"; // Hungary
                    case "RO": return "RO"; // Romania
                    case "BG": return "BG"; // Bulgaria
                    case "HR": return "HR"; // Croatia
                    case "SI": return "SI"; // Slovenia
                    case "SK": return "SK"; // Slovakia
                    case "LT": return "LT"; // Lithuania
                    case "LV": return "LV"; // Latvia
                    case "EE": return "EE"; // Estonia
                    case "IE": return "IE"; // Ireland
                    case "PT": return "PT"; // Portugal
                    case "GR": return "GR"; // Greece
                    case "CY": return "CY"; // Cyprus
                    case "MT": return "MT"; // Malta
                    case "LU": return "LU"; // Luxembourg
                    case "US": return "US"; // United States
                    case "CA": return "CA"; // Canada
                    case "AU": return "AU"; // Australia
                    case "NZ": return "NZ"; // New Zealand
                    case "JP": return "JP"; // Japan
                    case "SG": return "SG"; // Singapore
                    case "HK": return "HK"; // Hong Kong
                    case "MY": return "MY"; // Malaysia
                    case "TH": return "TH"; // Thailand
                    case "IN": return "IN"; // India
                    case "BR": return "BR"; // Brazil
                    case "MX": return "MX"; // Mexico
                    case "AR": return "AR"; // Argentina
                    case "CL": return "CL"; // Chile
                    case "CO": return "CO"; // Colombia
                    case "PE": return "PE"; // Peru
                    case "VE": return "VE"; // Venezuela
                    case "ZA": return "ZA"; // South Africa
                    case "EG": return "EG"; // Egypt
                    case "NG": return "NG"; // Nigeria
                    case "KE": return "KE"; // Kenya
                    case "GH": return "GH"; // Ghana
                    case "UG": return "UG"; // Uganda
                    case "TZ": return "TZ"; // Tanzania
                    case "RW": return "RW"; // Rwanda
                    case "ET": return "ET"; // Ethiopia
                    case "SD": return "SD"; // Sudan
                    case "DZ": return "DZ"; // Algeria
                    case "MA": return "MA"; // Morocco
                    case "TN": return "TN"; // Tunisia
                    case "LY": return "LY"; // Libya
                    case "TR": return "TR"; // Turkey
                    case "IL": return "IL"; // Israel
                    case "SA": return "SA"; // Saudi Arabia
                    case "AE": return "AE"; // United Arab Emirates
                    case "QA": return "QA"; // Qatar
                    case "KW": return "KW"; // Kuwait
                    case "BH": return "BH"; // Bahrain
                    case "OM": return "OM"; // Oman
                    case "JO": return "JO"; // Jordan
                    case "LB": return "LB"; // Lebanon
                    case "SY": return "SY"; // Syria
                    case "IQ": return "IQ"; // Iraq
                    case "IR": return "IR"; // Iran
                    case "PK": return "PK"; // Pakistan
                    case "AF": return "AF"; // Afghanistan
                    case "BD": return "BD"; // Bangladesh
                    case "LK": return "LK"; // Sri Lanka
                    case "NP": return "NP"; // Nepal
                    case "BT": return "BT"; // Bhutan
                    case "MV": return "MV"; // Maldives
                    case "MM": return "MM"; // Myanmar
                    case "LA": return "LA"; // Laos
                    case "KH": return "KH"; // Cambodia
                    case "VN": return "VN"; // Vietnam
                    case "PH": return "PH"; // Philippines
                    case "ID": return "ID"; // Indonesia
                    case "BN": return "BN"; // Brunei
                    case "TL": return "TL"; // Timor-Leste
                    case "PG": return "PG"; // Papua New Guinea
                    case "FJ": return "FJ"; // Fiji
                    case "SB": return "SB"; // Solomon Islands
                    case "VU": return "VU"; // Vanuatu
                    case "NC": return "NC"; // New Caledonia
                    case "PF": return "PF"; // French Polynesia
                    case "WS": return "WS"; // Samoa
                    case "TO": return "TO"; // Tonga
                    case "KI": return "KI"; // Kiribati
                    case "TV": return "TV"; // Tuvalu
                    case "NR": return "NR"; // Nauru
                    case "PW": return "PW"; // Palau
                    case "MH": return "MH"; // Marshall Islands
                    case "FM": return "FM"; // Micronesia
                    case "CK": return "CK"; // Cook Islands
                    case "NU": return "NU"; // Niue
                    case "TK": return "TK"; // Tokelau
                    case "AS": return "AS"; // American Samoa
                    case "GU": return "GU"; // Guam
                    case "MP": return "MP"; // Northern Mariana Islands
                    case "PR": return "PR"; // Puerto Rico
                    case "VI": return "VI"; // U.S. Virgin Islands
                    case "AI": return "AI"; // Anguilla
                    case "AG": return "AG"; // Antigua and Barbuda
                    case "AW": return "AW"; // Aruba
                    case "BS": return "BS"; // Bahamas
                    case "BB": return "BB"; // Barbados
                    case "BZ": return "BZ"; // Belize
                    case "BM": return "BM"; // Bermuda
                    case "BO": return "BO"; // Bolivia
                    case "CR": return "CR"; // Costa Rica
                    case "CU": return "CU"; // Cuba
                    case "DM": return "DM"; // Dominica
                    case "DO": return "DO"; // Dominican Republic
                    case "EC": return "EC"; // Ecuador
                    case "SV": return "SV"; // El Salvador
                    case "GD": return "GD"; // Grenada
                    case "GT": return "GT"; // Guatemala
                    case "GY": return "GY"; // Guyana
                    case "HT": return "HT"; // Haiti
                    case "HN": return "HN"; // Honduras
                    case "JM": return "JM"; // Jamaica
                    case "NI": return "NI"; // Nicaragua
                    case "PA": return "PA"; // Panama
                    case "PY": return "PY"; // Paraguay
                    case "SR": return "SR"; // Suriname
                    case "TT": return "TT"; // Trinidad and Tobago
                    case "UY": return "UY"; // Uruguay
                    default: return "US"; // Default to US for unsupported countries
                }
            }

            return "US"; // Default to US if IBAN format is invalid
        }

        public static string GetCurrencyFromIBAN(this string iban)
        {
            if (string.IsNullOrEmpty(iban))
                return "USD"; // Default currency if no IBAN provided

            if (iban.Length >= 2)
            {
                var countryCode = iban.Substring(0, 2).ToUpper();

                switch (countryCode)
                {
                    // Eurozone countries
                    case "DE": // Germany
                    case "FR": // France
                    case "ES": // Spain
                    case "IT": // Italy
                    case "NL": // Netherlands
                    case "BE": // Belgium
                    case "AT": // Austria
                    case "PT": // Portugal
                    case "FI": // Finland
                    case "GR": // Greece
                    case "IE": // Ireland
                    case "LU": // Luxembourg
                    case "CY": // Cyprus
                    case "MT": // Malta
                    case "SI": // Slovenia
                    case "SK": // Slovakia
                    case "EE": // Estonia
                    case "LV": // Latvia
                    case "LT": // Lithuania
                        return "EUR";

                    case "GB": return "GBP"; // United Kingdom
                    case "CH": return "CHF"; // Switzerland
                    case "NO": return "NOK"; // Norway
                    case "SE": return "SEK"; // Sweden
                    case "DK": return "DKK"; // Denmark
                    case "PL": return "PLN"; // Poland
                    case "CZ": return "CZK"; // Czech Republic
                    case "HU": return "HUF"; // Hungary
                    case "RO": return "RON"; // Romania
                    case "BG": return "BGN"; // Bulgaria
                    case "HR": return "EUR"; // Croatia (since 2023 uses EUR)

                    case "US": return "USD"; // United States
                    case "CA": return "CAD"; // Canada
                    case "AU": return "AUD"; // Australia
                    case "NZ": return "NZD"; // New Zealand
                    case "JP": return "JPY"; // Japan
                    case "SG": return "SGD"; // Singapore
                    case "HK": return "HKD"; // Hong Kong
                    case "IN": return "INR"; // India
                    case "BR": return "BRL"; // Brazil
                    case "MX": return "MXN"; // Mexico
                    case "ZA": return "ZAR"; // South Africa
                    case "AE": return "AED"; // United Arab Emirates
                    case "SA": return "SAR"; // Saudi Arabia
                    case "QA": return "QAR"; // Qatar
                    case "KW": return "KWD"; // Kuwait
                    case "OM": return "OMR"; // Oman
                    case "PK": return "PKR"; // Pakistan
                    case "BD": return "BDT"; // Bangladesh
                    case "LK": return "LKR"; // Sri Lanka
                    case "NP": return "NPR"; // Nepal
                    case "CN": return "CNY"; // China
                    case "TH": return "THB"; // Thailand
                    case "MY": return "MYR"; // Malaysia
                    case "ID": return "IDR"; // Indonesia
                    case "PH": return "PHP"; // Philippines

                    default: return "USD"; // Fallback currency
                }
            }

            return "USD"; // Default if IBAN is invalid
        }

    }
}
