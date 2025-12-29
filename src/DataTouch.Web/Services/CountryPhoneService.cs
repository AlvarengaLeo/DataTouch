namespace DataTouch.Web.Services;

/// <summary>
/// Service providing country phone data for validation and formatting.
/// </summary>
public class CountryPhoneService
{
    private static readonly List<CountryPhoneData> _countries = new()
    {
        // Central America
        new CountryPhoneData { Code = "SV", Name = "El Salvador", DialCode = "+503", Flag = "🇸🇻", MinLength = 8, MaxLength = 8, Placeholder = "7000 0000", Pattern = @"^[267][0-9]{7}$" },
        new CountryPhoneData { Code = "GT", Name = "Guatemala", DialCode = "+502", Flag = "🇬🇹", MinLength = 8, MaxLength = 8, Placeholder = "5000 0000", Pattern = @"^[2-7][0-9]{7}$" },
        new CountryPhoneData { Code = "HN", Name = "Honduras", DialCode = "+504", Flag = "🇭🇳", MinLength = 8, MaxLength = 8, Placeholder = "9000 0000", Pattern = @"^[23789][0-9]{7}$" },
        new CountryPhoneData { Code = "NI", Name = "Nicaragua", DialCode = "+505", Flag = "🇳🇮", MinLength = 8, MaxLength = 8, Placeholder = "8000 0000", Pattern = @"^[578][0-9]{7}$" },
        new CountryPhoneData { Code = "CR", Name = "Costa Rica", DialCode = "+506", Flag = "🇨🇷", MinLength = 8, MaxLength = 8, Placeholder = "8000 0000", Pattern = @"^[2-8][0-9]{7}$" },
        new CountryPhoneData { Code = "PA", Name = "Panamá", DialCode = "+507", Flag = "🇵🇦", MinLength = 8, MaxLength = 8, Placeholder = "6000 0000", Pattern = @"^[236][0-9]{7}$" },
        new CountryPhoneData { Code = "BZ", Name = "Belice", DialCode = "+501", Flag = "🇧🇿", MinLength = 7, MaxLength = 7, Placeholder = "600 0000", Pattern = @"^[6][0-9]{6}$" },
        
        // North America
        new CountryPhoneData { Code = "MX", Name = "México", DialCode = "+52", Flag = "🇲🇽", MinLength = 10, MaxLength = 10, Placeholder = "55 1234 5678", Pattern = @"^[1-9][0-9]{9}$" },
        new CountryPhoneData { Code = "US", Name = "Estados Unidos", DialCode = "+1", Flag = "🇺🇸", MinLength = 10, MaxLength = 10, Placeholder = "555 123 4567", Pattern = @"^[2-9][0-9]{9}$" },
        new CountryPhoneData { Code = "CA", Name = "Canadá", DialCode = "+1", Flag = "🇨🇦", MinLength = 10, MaxLength = 10, Placeholder = "416 123 4567", Pattern = @"^[2-9][0-9]{9}$" },
        
        // South America
        new CountryPhoneData { Code = "CO", Name = "Colombia", DialCode = "+57", Flag = "🇨🇴", MinLength = 10, MaxLength = 10, Placeholder = "300 123 4567", Pattern = @"^[3][0-9]{9}$" },
        new CountryPhoneData { Code = "PE", Name = "Perú", DialCode = "+51", Flag = "🇵🇪", MinLength = 9, MaxLength = 9, Placeholder = "900 000 000", Pattern = @"^[9][0-9]{8}$" },
        new CountryPhoneData { Code = "AR", Name = "Argentina", DialCode = "+54", Flag = "🇦🇷", MinLength = 10, MaxLength = 10, Placeholder = "11 1234 5678", Pattern = @"^[1-9][0-9]{9}$" },
        new CountryPhoneData { Code = "CL", Name = "Chile", DialCode = "+56", Flag = "🇨🇱", MinLength = 9, MaxLength = 9, Placeholder = "9 1234 5678", Pattern = @"^[9][0-9]{8}$" },
        new CountryPhoneData { Code = "EC", Name = "Ecuador", DialCode = "+593", Flag = "🇪🇨", MinLength = 9, MaxLength = 9, Placeholder = "99 123 4567", Pattern = @"^[9][0-9]{8}$" },
        new CountryPhoneData { Code = "VE", Name = "Venezuela", DialCode = "+58", Flag = "🇻🇪", MinLength = 10, MaxLength = 10, Placeholder = "412 123 4567", Pattern = @"^[4][0-9]{9}$" },
        new CountryPhoneData { Code = "BO", Name = "Bolivia", DialCode = "+591", Flag = "🇧🇴", MinLength = 8, MaxLength = 8, Placeholder = "7000 0000", Pattern = @"^[67][0-9]{7}$" },
        new CountryPhoneData { Code = "PY", Name = "Paraguay", DialCode = "+595", Flag = "🇵🇾", MinLength = 9, MaxLength = 9, Placeholder = "981 123 456", Pattern = @"^[9][0-9]{8}$" },
        new CountryPhoneData { Code = "UY", Name = "Uruguay", DialCode = "+598", Flag = "🇺🇾", MinLength = 8, MaxLength = 8, Placeholder = "99 123 456", Pattern = @"^[9][0-9]{7}$" },
        
        // Caribbean
        new CountryPhoneData { Code = "DO", Name = "República Dominicana", DialCode = "+1", Flag = "🇩🇴", MinLength = 10, MaxLength = 10, Placeholder = "809 123 4567", Pattern = @"^(809|829|849)[0-9]{7}$" },
        new CountryPhoneData { Code = "PR", Name = "Puerto Rico", DialCode = "+1", Flag = "🇵🇷", MinLength = 10, MaxLength = 10, Placeholder = "787 123 4567", Pattern = @"^(787|939)[0-9]{7}$" },
        new CountryPhoneData { Code = "CU", Name = "Cuba", DialCode = "+53", Flag = "🇨🇺", MinLength = 8, MaxLength = 8, Placeholder = "5123 4567", Pattern = @"^[5][0-9]{7}$" },
        
        // Europe
        new CountryPhoneData { Code = "ES", Name = "España", DialCode = "+34", Flag = "🇪🇸", MinLength = 9, MaxLength = 9, Placeholder = "600 000 000", Pattern = @"^[6-7][0-9]{8}$" },
        new CountryPhoneData { Code = "FR", Name = "Francia", DialCode = "+33", Flag = "🇫🇷", MinLength = 9, MaxLength = 9, Placeholder = "6 12 34 56 78", Pattern = @"^[67][0-9]{8}$" },
        new CountryPhoneData { Code = "DE", Name = "Alemania", DialCode = "+49", Flag = "🇩🇪", MinLength = 10, MaxLength = 11, Placeholder = "151 1234 5678", Pattern = @"^[1][0-9]{9,10}$" },
        new CountryPhoneData { Code = "IT", Name = "Italia", DialCode = "+39", Flag = "🇮🇹", MinLength = 9, MaxLength = 10, Placeholder = "312 345 6789", Pattern = @"^[3][0-9]{8,9}$" },
        new CountryPhoneData { Code = "GB", Name = "Reino Unido", DialCode = "+44", Flag = "🇬🇧", MinLength = 10, MaxLength = 10, Placeholder = "7911 123456", Pattern = @"^[7][0-9]{9}$" },
        new CountryPhoneData { Code = "PT", Name = "Portugal", DialCode = "+351", Flag = "🇵🇹", MinLength = 9, MaxLength = 9, Placeholder = "912 345 678", Pattern = @"^[9][0-9]{8}$" },
    };

    /// <summary>
    /// Gets all available countries for phone input.
    /// </summary>
    public IReadOnlyList<CountryPhoneData> GetAllCountries() => _countries.AsReadOnly();

    /// <summary>
    /// Gets country data by country code (e.g., "SV", "MX").
    /// </summary>
    public CountryPhoneData? GetByCode(string countryCode) =>
        _countries.FirstOrDefault(c => c.Code.Equals(countryCode, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets country data by dial code (e.g., "+503", "+52").
    /// </summary>
    public CountryPhoneData? GetByDialCode(string dialCode) =>
        _countries.FirstOrDefault(c => c.DialCode.Equals(dialCode, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets the default country (El Salvador).
    /// </summary>
    public CountryPhoneData GetDefaultCountry() => 
        _countries.First(c => c.Code == "SV");

    /// <summary>
    /// Validates a phone number against a country's rules.
    /// </summary>
    public PhoneValidationResult ValidatePhone(string countryCode, string nationalNumber)
    {
        var country = GetByCode(countryCode);
        if (country == null)
        {
            return new PhoneValidationResult
            {
                IsValid = false,
                ErrorMessage = "Selecciona un país"
            };
        }

        // Remove any non-digit characters
        var digitsOnly = new string(nationalNumber?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());

        if (string.IsNullOrEmpty(digitsOnly))
        {
            return new PhoneValidationResult
            {
                IsValid = false,
                ErrorMessage = "Ingresa tu número de teléfono",
                CurrentLength = 0,
                RequiredLength = country.MinLength
            };
        }

        if (digitsOnly.Length < country.MinLength)
        {
            var missing = country.MinLength - digitsOnly.Length;
            return new PhoneValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Faltan {missing} dígito{(missing > 1 ? "s" : "")}",
                CurrentLength = digitsOnly.Length,
                RequiredLength = country.MinLength
            };
        }

        if (digitsOnly.Length > country.MaxLength)
        {
            return new PhoneValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Máximo {country.MaxLength} dígitos para {country.Name}",
                CurrentLength = digitsOnly.Length,
                RequiredLength = country.MaxLength
            };
        }

        // Check pattern if defined
        if (!string.IsNullOrEmpty(country.Pattern))
        {
            var regex = new System.Text.RegularExpressions.Regex(country.Pattern);
            if (!regex.IsMatch(digitsOnly))
            {
                return new PhoneValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Número inválido para {country.Name}",
                    CurrentLength = digitsOnly.Length,
                    RequiredLength = country.MinLength
                };
            }
        }

        return new PhoneValidationResult
        {
            IsValid = true,
            ErrorMessage = null,
            CurrentLength = digitsOnly.Length,
            RequiredLength = country.MinLength,
            FormattedE164 = $"{country.DialCode}{digitsOnly}"
        };
    }

    /// <summary>
    /// Formats a phone number to E.164 standard format.
    /// </summary>
    public string FormatToE164(string countryCode, string nationalNumber)
    {
        var country = GetByCode(countryCode);
        if (country == null) return nationalNumber;

        var digitsOnly = new string(nationalNumber?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
        return $"{country.DialCode}{digitsOnly}";
    }
}

/// <summary>
/// Represents phone data for a specific country.
/// </summary>
public class CountryPhoneData
{
    /// <summary>ISO 3166-1 alpha-2 country code (e.g., "SV", "MX").</summary>
    public string Code { get; set; } = default!;
    
    /// <summary>Country name in Spanish (e.g., "El Salvador").</summary>
    public string Name { get; set; } = default!;
    
    /// <summary>International dial code with + prefix (e.g., "+503").</summary>
    public string DialCode { get; set; } = default!;
    
    /// <summary>Flag emoji for the country (e.g., "🇸🇻").</summary>
    public string Flag { get; set; } = default!;
    
    /// <summary>Minimum number of digits for national number.</summary>
    public int MinLength { get; set; }
    
    /// <summary>Maximum number of digits for national number.</summary>
    public int MaxLength { get; set; }
    
    /// <summary>Example placeholder for the input field.</summary>
    public string Placeholder { get; set; } = default!;
    
    /// <summary>Regex pattern for validation (optional).</summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// Returns a formatted display string: "🇸🇻 El Salvador (+503)".
    /// </summary>
    public string DisplayText => $"{Flag} {Name} ({DialCode})";
}

/// <summary>
/// Result of phone number validation.
/// </summary>
public class PhoneValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public int CurrentLength { get; set; }
    public int RequiredLength { get; set; }
    public string? FormattedE164 { get; set; }
}
