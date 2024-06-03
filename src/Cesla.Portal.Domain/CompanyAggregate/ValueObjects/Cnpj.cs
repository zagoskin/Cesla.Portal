using System.Text.Json.Serialization;
using Cesla.Portal.Domain.Common.Models;
using ErrorOr;

namespace Cesla.Portal.Domain.CompanyAggregate.ValueObjects;

public sealed class Cnpj : ValueObject
{
    public string Value { get; init; }

    [JsonConstructor]
    private Cnpj(string value)
    {
        Value = value;
    }

    public static ErrorOr<Cnpj> Create(string cnpj)
    {
        if (!TryFormatAsCnpj(cnpj, out string formattedCnpj))
        {
            return Error.Validation(code: "Cnpj.Invalid", description: "Cnpj validation failed");
        }

        return new Cnpj(formattedCnpj);
    }
    public static bool IsValid(string cnpj)
    {
        // Remove any non-digit characters from the input
        string cleanedCnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        return IsValidCleanedInput(cleanedCnpj);        
    }

    private static bool TryFormatAsCnpj(ReadOnlySpan<char> input, out string formattedCnpj)
    {
        // Remove any non-digit characters from the input
        formattedCnpj = string.Empty;
        Span<char> digits = stackalloc char[14];
        int index = 0;

        foreach (char c in input)
        {
            if (!char.IsDigit(c))
            {
                continue;
            }

            if (index >= 14)
            {
                return false;
            }
            digits[index++] = c;
        }

        if (index != 14)
        {
            return false;
        }

        if (!IsValidCleanedInput(digits))
        {
            return false;
        }


        formattedCnpj = BuildCnpjStringFromDigits(digits);
        return true;
    }

    private static bool IsValidCleanedInput(ReadOnlySpan<char> cleanedCnpj)
    {
        // Check if the cleaned CNPJ has the correct length
        if (cleanedCnpj.Length != 14)
        {
            return false;
        }

        // Predefined weights for CNPJ validation
        int[] weightsFirstDigit = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] weightsSecondDigit = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        // Calculate the first verification digit
        int sumFirstDigit = 0;
        for (int i = 0; i < 12; i++)
        {
            sumFirstDigit += (cleanedCnpj[i] - '0') * weightsFirstDigit[i];
        }
        int firstDigit = sumFirstDigit % 11 < 2 ? 0 : 11 - (sumFirstDigit % 11);

        // Calculate the second verification digit
        int sumSecondDigit = 0;
        for (int i = 0; i < 13; i++)
        {
            sumSecondDigit += (cleanedCnpj[i] - '0') * weightsSecondDigit[i];
        }
        int secondDigit = sumSecondDigit % 11 < 2 ? 0 : 11 - (sumSecondDigit % 11);

        // Check if the verification digits match the input
        return cleanedCnpj[12] - '0' == firstDigit && cleanedCnpj[13] - '0' == secondDigit;
    }    

    private static string BuildCnpjStringFromDigits(ReadOnlySpan<char> digits)
    {
        Span<char> formattedCnpj =
        [
            digits[0],
            digits[1],
            '.',
            digits[2],
            digits[3],
            digits[4],
            '.',
            digits[5],
            digits[6],
            digits[7],
            '/',
            digits[8],
            digits[9],
            digits[10],
            digits[11],
            '-',
            digits[12],
            digits[13],
        ];
        return new string(formattedCnpj);
    }
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
