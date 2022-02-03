using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

internal class Lambdas
{
    public Lambdas()
    {
        Func<string, int> parse = (string s) => int.Parse(s);

        Func<bool, object> choose = (bool b) => b ? 1 : "two";
    }
}

internal class MethodGroups
{
    public MethodGroups()
    {
        Func<int> read = Console.Read;
        Action<string> write = Console.Write;
    }
}

internal class InterpolatedStringHandlers
{
    public string BuildString(object[] args)
    {
        var sb = new StringBuilder();
        sb.Append($"Hello {args[0]}, how are you?");

        return sb.ToString();
    }

    public void DebugAssert(bool condition)
    {
        Debug.Assert(condition, $"{DateTime.Now} - {ExpensiveCalculation()}");
    }

    public string CreateInvariantString(int result)
        => string.Create(CultureInfo.InvariantCulture, $"The result is {result}");

    private static object ExpensiveCalculation()
    {
        Thread.Sleep(1000);
        return 0;
    }
}