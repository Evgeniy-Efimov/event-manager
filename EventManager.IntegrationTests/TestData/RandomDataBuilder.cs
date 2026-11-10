using System.Text;

namespace EventManager.IntegrationTests.TestData;

public static class RandomDataBuilder
{
    private const string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static Random Rnd => Random.Shared;

    public static string CreateRandomString(int length = 10)
    {
        var stringBuilder = new StringBuilder(length);

        for (var i = 0; i < length; i++)
            stringBuilder.Append(_chars[Rnd.Next(_chars.Length)]);

        return stringBuilder.ToString();
    }

    public static int CreateRandomInt(int min = int.MinValue, int max = int.MaxValue)
    {
        if (min > max)
            throw new ArgumentException($"min ({min}) cannot be greater than max ({max})");

        if (min == max)
            return min;

        return min + (int)((max - min) * Rnd.NextDouble());
    }
}
