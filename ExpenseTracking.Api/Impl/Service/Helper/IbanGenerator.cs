namespace ExpenseTracking.Api.Impl.Service.Helper;
public static class IbanGenerator
{
    private const string CountryCode = "TR";
    private const string BankCode = "00000";
    private static Random random = new();

    public static string GenerateIban()
    {
        const string countryCode = "TR";
        const int ibanLength = 26;
        var random = new Random();
        var numericPart = new string(Enumerable.Range(0, ibanLength - countryCode.Length)
            .Select(_ => (char)('0' + random.Next(0, 10)))
            .ToArray());

        return countryCode + numericPart;
    }

    private static int Mod97(string input)
    {
        const int chunkSize = 9;
        long total = 0;
        for (int pos = 0; pos < input.Length; pos += chunkSize)
        {
            var chunk = total + long.Parse(input.Substring(pos, Math.Min(chunkSize, input.Length - pos)));
            total = chunk % 97;
        }
        return (int)total;
    }

    private static long NextLong(this Random random, long min, long max)
    {
        var buf = new byte[8];
        random.NextBytes(buf);
        var longRand = Math.Abs(BitConverter.ToInt64(buf, 0));
        return (long)(min + (longRand / (double)long.MaxValue * (max - min)));
    }
}
