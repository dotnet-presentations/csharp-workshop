# C# 10 Demo Script

# Preamble

1. Open `Program.cs`.
2. Build and run (`Ctrl+F5`) to show the program output.

# Record Improvements

## Record Structs

Record structs have the same features and very similar semantics as record classes. They provide the full machinery for value equality, including an implementation of `IEquatable<T>` and `==`/`!=` operators. They also support `with` expressions.

1. Add the `class` keyword after `record` for `Person`. It should now read as `record class Person`.
2. Change `class` to `struct` for `Person`. It should now read as `record struct Person`.
3. Build and run (`Ctrl+F5`) to show that the program output is unchanged.

## With Expressions for Non-Record Structs

`with` expression support has been added for vanilla non-record structs. This works seamlessly because structs are copy-by-value.

1. Remove the `record` keyword from `Person`. It should now read as `struct Person`.

## With Expressions for Anonymous Types

Semantically, anonymous types are really "anonymous records". So, we've made `with` expressions work with them as well.

1. Remove `Person` from `var person = new Person` on line 4. It should now read as `var person = new`. 
2. Hover over `var` to show that `person` is now an anonymous type.

## Other Struct Improvements

We've added several other features to vanilla non-record structs. First, it is now possible to declare public, parameterless constructor.

1. Add a public, parameterless constructor to `Person` that initializes both the `FirstName` and `LastName` properties.
    
    ```csharp
    struct Person
    {
        public Person()
        {
            FirstName = "John";
            LastName = "Doe";
        }
    
        public string FirstName { get; init; }
        public string LastName { get; init; }
        
        public void WriteToFile(string filePath)
            => File.WriteAllText(filePath, ToString());
    }
    ```
    

This represents a change in philosophy for C# with regard to structs. Previously, we chose not allow public, parameterless constructors because it means that `default(SomeStruct)` may result in a different value than `new SomeString()`. So, the following code results in different `Person` values.

1. Add the following lines above the declaration of `Person` struct
    
    ```csharp
    Person p1 = default;
    Person p2 = new();
    ```
    

In addition, it is now possible to use field and property initializers in structs, which weren't previously allowed for the same reason.

1. Remove the constructor that was just added and use initializers for the properties.
    
    ```csharp
    struct Person
    {
        public string FirstName { get; init; } = "John";
        public string LastName { get; init; } = "Doe";
    }
    ```
    

# Removing Clutter

## File-Scoped Namespaces

Our project is starting to grow, so let's move `Person` into a separate file. Fortunately, the IDE has several refactorings that can help with that.

1. Move editor caret to `Person` and press `Ctrl+.` to bring up the list of available refactorings.
2. Choose "Move type to Person.cs" to move the `Person` struct to a new file.
3. Click on a reference to `Person` and press `F12` to go to the definition of `Person` in `Person.cs`.
4. Move the editor caret to `Person` and press `Ctrl+.` to bring up the list of available refactorings.
5. Choose "Move to namespace..." and type "Model" to move `Person` into a namespace named `Model`.

It's unfortunate that namespaces have forced nearly every C# class to be indented. With file-scoped namespaces, that's no longer a concern.

1. Remove the curly braces for the `Model` namespace and add a `;` after `Model`.

```csharp
using System.IO;

namespace Model;

struct Person
{
    public Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; init; } = "John";
    public string LastName { get; init; } = "Doe";

    public void WriteToFile(string filePath)
        => File.WriteAllText(filePath, ToString());
}
```

## Global Usings

Another bit of C# clutter that has "infected" every C# file is the block of using directives. In C# 10, this can be cleaned up with global using directives, which are essentially using directives that apply throughout the project.

1. Open `Usings.cs`.
2. Move the editor caret before the first `using` and press `Shift+Alt+Down` until there is a vertical selection in front of all the using directives.
3. Type `global` before the using directives to transform all of them into global usings.

We should go ahead and add a global using directive for `Model`, since we expect to use that namespace throughout our project.

1. Immediately before the last global using, and one one more: `global using Model;`.
    
    ```csharp
    global using System;
    global using System.Collections.Generic;
    global using System.IO;
    global using System.Linq;
    global using System.Text;
    global using System.Threading.Tasks;
    global using Model;
    global using static System.Console;
    ```
    

And now we can clean up the using directives in our other files, since they're declared globally here.

1. Use the "Remove Unnecessary Usings" code fix at the top of both `Person.cs` and `Program.cs`.

## Implicit Usings

For .NET 6, we've built a tooling feature that allows global using directives to be generated from information in the project file. We call this feature "implicit usings". This feature is enabled by default for new projects created with .NET 6 SDK, and it's easy to enable it for existing projects.

1. Open the project file (`CSharpTen.csproj`) and add `<ImplicitUsings>enable</ImplicitUsings>`.
    
    ```xml
    <Project Sdk="Microsoft.NET.Sdk">
    
      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net6.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
      </PropertyGroup>
    
    </Project>
    ```
    

We can easily add our `Model` namespace here as well.

1. Add a new `<ItemGroup>` containing `<Using Include="Model"/>`.
    
    ```xml
    <Project Sdk="Microsoft.NET.Sdk">
    
      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net6.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
      </PropertyGroup>
    
      <ItemGroup>
        <Using Include="Model" />
      </ItemGroup>
    
    </Project>
    ```
    
2. Open `Using.cs` and delete all of the using directives except for `global using static System.Console;`.

# Lambda and Method Group Improvements

Let's take a look at a few other features of C# 10, such as improvements to lambda expressions and method groups.

1. Open `OtherStuff.cs`.

## Natural Types for Lambdas

At long last, we've defined natural types for lambda expressions, which are the `Func` and `Action` delegates that were first introduced in C# 3.0.

Now, we can can correctly infer a delegate type for a lambda expression if there is enough type information.

1. On line 9, change `Func<string, int>` to `var`.
    
    ```csharp
    var parse = (string s) => int.Parse(s);
    ```
    

Of course, if there isn't enough information to infer a delegate type, the C# compiler will produce an error.

1. Change `(string s) =>` to `s`.
    
    ```csharp
    var parse = s => int.Parse(s);
    ```
    

It's also legal to assign a lambda to a type that is convertible from the inferred delegate type, such as  `object` or `Delegate`.

1. Change `var` to `object`.
    
    ```csharp
    object parse = (string s) => int.Parse(s);
    ```
    
2. Change `object` to `Delegate`.
    
    ```csharp
    Delegate parse = (string s) => int.Parse(s);
    ```
    

Similarly, it is possible to assign a lambda to a legal `Expression` type.

1. Change `Delegate` to `Expression` and press `Ctrl+.` to add a using directive for `System.Linq.Expressions`.
    
    ```csharp
    Expression parse = (string s) => int.Parse(s);
    ```
    
2. Change `Expression` to `LambdaExpression`.
    
    ```csharp
    LambdaExpression parse = (string s) => int.Parse(s);
    ```
    

## Return Types for Lambdas

In this case, not enough information is provided by the lambda expression to infer a return type. So, the compiler produces an error.

1. On line 11, change `Func<bool, object>` to `var`.
    
    ```csharp
    var choose = (bool b) => b ? 1 : "two";
    ```
    

In C# 10, it's possible to add return types for lambda expressions, which allows `Func<bool, object>` to be inferred.

1. Add `object` after `=` to give the lambda expression a return type.
    
    ```csharp
    var choose = object (bool b) => b ? 1 : "two";
    ```
    

## Natural Types for Method Groups

Finally, C# 10 will infer delegate types for method groups if possible.

1. On line 20, change `Func<int>` to `var`.
    
    ```csharp
    var read = Console.Read;
    ```
    

In this case, there are multiple overloads, so a delegate type can't be inferred.

1. On line 21, change `Action<string>` to `var`.
    
    ```csharp
    var write = Console.Write;
    ```

    Notice that this will cause an error.
    

# Interpolated String Handlers

1. Open `OtherStuff.cs`.

C# 6 introduced an incredibly powerful and useful feature: interpolated strings. At a high level, interpolated strings are highly readable  `string.Format(...)` calls. Unfortunately, they bring a lot of costs in the form of hidden allocations. In addition, interpolated strings are built eagerly, 

In C# 10, we've introduced a new library pattern that allows APIs to be written that work directly with interpolated strings and can make the code that we're already writing far more efficient. Several of APIs that take advantage of this pattern have been added in .NET 6.

1. Show the interpolated string usage on line 32.

```csharp
public string BuildString(object[] args)
{
    var sb = new StringBuilder();
    sb.Append($"Hello {args[0]}, how are you?");

    return sb.ToString();
}
```

In C# 6, this interpolated string would result a `string.Format(...)` call which would use a different `StringBuilder` to produce a string. Then the string would be added to `sb`. In C# 10, the interpolated string is handled specially and is added directly to `sb` without requiring another `StringBuilder`. So, the code that we were already writing is just faster.

Hovering over the `StringBuilder.Append` call on line 32 reveals that the overload of `Append` that is being called takes a `StringBuilder.AppendInterpolatedStringHandler` rather than a `string`. This allows the API to perform custom processing of the interpolated string arguments.

1. Show the interpolated string usage on line 39.

```csharp
public void DebugAssert(bool condition)
{
    Debug.Assert(condition, $"{DateTime.Now} - {ExpensiveCalculation()}");
}
```

In C# 6, the interpolated string passed to `Debug.Assert` is *always* created, even if `condition` is `true`. In C# 10, the arguments to the interpolated string won't be evaluated unless `condition` is `false`.