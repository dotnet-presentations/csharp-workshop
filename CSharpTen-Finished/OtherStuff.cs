using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

internal class Lambdas
{
    public Lambdas()
    {
        LambdaExpression parse = (string s) => int.Parse(s);

        var choose = object (bool b) => b ? 1 : "two";
    }
}

internal class MethodGroups
{
    public MethodGroups()
    {
        var read = Console.Read;
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