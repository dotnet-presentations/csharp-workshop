# Demo script

Starter code (in the attached project):

```csharp
using static System.Console;

class Program
{
    static void Main(string[] args)
    {
        var person = new Person
        {
            FirstName = "Scott",
            LastName = "Hunter"
        };

        DisplayPerson(person);

        static void DisplayPerson(Person person)
        {
            WriteLine($"{person.FirstName} {person.LastName}");
        }
    }
}

class Person
{ 
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

class Student : Person
{
    public double Gpa { get; set; } = 4.0;
}
```

## Introduce top level statements

Show the initial program. It's a nice starting point. But, for a simple program, it dosn't fit on one page. That's because C# has some ceremony. You can't do anything until you learn `class Program` and `static void Main()`. That's just cruft. Let's remove it, and fix the extra indentation:

```csharp
using static System.Console;

var person = new Person
{
    FirstName = "Scott",
    LastName = "Hunter"
};

DisplayPerson(person);

static void DisplayPerson(Person person)
{
    WriteLine($"{person.FirstName} {person.LastName}");
}

class Person
{ 
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

class Student : Person
{
    public double Gpa { get; set; } = 4.0;
}
```

Currently, the Visual Studio templates default to targeting .NET Core 3.1. That means C# 8. You get compiler errors. Tell folks that will get updated for the next Visual Studio release. In the meantime, all the .NET 5.0 SDK is there, so you can target .NET 5. Changing the target (either in Project properties, or editing the CSPROJ file) to "net5" fixes the compiler errors.

Introduce some code to show some of the restrictions:

1. Add a `WriteLine` after the declaration of `Student`. That causes a compiler error. All top level statements *must* appear before any type declarations (like Person) or namespace declarations.
1. Point out that `DisplayPerson` is still a *static local function*. It (and any functions) may appear along with your top level statements. You can still declare local functions among your top level statements.

Finishing this section, discuss some real world use cases. Azure Functions are one example where these will likely be used in production. In fact, many console applications may benefit from this.

## Init only properties

Change the name of the person. Discuss that your design wants to be *immutable*. Make the `LastName` property readonly, point out the compiler error. Complain that you like the object initializer syntax, but want immutability. Change the `set` accessors to `init` accessors:

```csharp
class Person
{ 
    public string FirstName { get; init; }
    public string LastName { get; init; }
}
```

Now, you can still use the initializer syntax, but you can't change these properties after you've created the object. This is already better. Let's continue.

## Records and nondestructive mutation

In almost all programs, you probably want to change state on these objects at some point. One practice to do that is called *nondestructive mutation*. A *nondestructive mutation* means making a copy and making one or more modifications to that copy as part of its initialization. Instead of an exact copy, it's a similar copy. You can use "with" methods to make this similar, but not exact copy. This "with" methods are often referred to as "withers".

You can write with methods youself, but that is painful. Instead, leverage *records* a new type that creates a number of members for you. There are a few steps to make these changes.

1. Change `Person` to a record:
    ```csharp
    record Person
    { 
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
    ```
    Whoops! That's a compiler error. Records can only inherit from other records. Once you step into this world, you must embrace it throughout a hierarchy.
1. Change `Student` to a record:
    ```csharp
    record Student : Person
    {
        public double Gpa { get; set; } = 4.0;
    }
    ```
    Now, everything compiles. Both `Person` and `Student` are records.
1. Next, let's use a `with` method to create a changed copy of a person:
    ```csharp
    var person = new Person
    {
        FirstName = "Scott",
        LastName = "Hunter"
    };

    var otherPerson = person with
    {
        LastName = "Hanselman"
    };

    DisplayPerson(person);
    DisplayPerson(otherPerson);
    ```

The `otherPerson` was created by copying and modifying the original `person`.

## Records have other *synthesized mebmbers*

Remove the `DisplayPerson()` method.  Change the `DisplayPerson` calls to call `WriteLine`. Records create their own `ToString()` output. This method prints a more reasonable output for any record type. That means less boilerplate code for you. Let's add some more cases.

Records also create methods to test for equality based on the *values* of a record's properties. Let's demonstrate this. Create a copy of the original by changing the last name again:

```csharp
var originalPerson = otherPerson with
{
    LastName = "Hunter"
};

WriteLine($"Equals: {Equals(person, originalPerson)}");
WriteLine($"Reference Equals: {ReferenceEquals(person, originalPerson)}");
```

## Record hierarchies

You've got a hierarchy of records: `Student` derives from `Person`. Let's explore how the compiler handles those hierarchies. First, change "Person" to "Student" in the first declaration. Also, change the static type of the declaration to `Person` (instead of `var`):

```csharp
Person person = new Student
{
    FirstName = "Scott",
    LastName = "Hunter"
};
```

Run the application. Notice that all the records are now `Student`, not `Person` objects.
Add a GPA, and see that the `Gpa` property is copied on each record you create from the first record:

```csharp
Person person = new Student
{
    FirstName = "Scott",
    LastName = "Hunter",
    Gpa = 3.8,
};
```

Gpa flows through. Let's make one more change to test equality between these record types. Add the following two lines after the current equality tests:

```csharp
var p = new Person("Scott", "Hunter");
WriteLine($"Person and Student: Equals: {Equals(person, p)}");
```

Note that `p`, which is a `Person` is not equal to `person`, which is a student. The equality tests do compare the types of records

## Positional records

Records are classes. But, we make a few assumptions about how records will be used. We assume that records are primarily defined by their public properties, not by their behavior. In addition, because of that, we assume you'll likely want records to be immutable. These aren't enforced, but when those assuptions are true you can use a more concise syntax called *Positional records*. Update the Person record as follows:

```csharp
record Person(string FirstName, string LastName);
```

Positional records create a constructor called a *primary constructor*. That's a constructor that takes parameter matching each of the public properties declared in the record declaration. That means derived records must call that primary constructor. Update the `Student` to match:

```csharp
record Student(string FirstName, string LastName) : Person(FirstName, LastName)
{
    public double Gpa { get; set; } = 4.0;
}
```

Now, both the `Student` and `Person` records are positional records. That means you'll need to craeate 
To compile this, you'll need to change the construction of `person`:

```csharp
Person person = new Student("Scott", "Hunter")
{
    Gpa = 3.8,
};
```

Point out that you can mix construction and object initializers for positional records. Note that GPA doesn't have to be immutable. 

Positional records also add deconstruction methods, because the compiler *assumes* the order you supplied the properties for the record. Add the following to test it:

```csharp
var (first, last) = person;
WriteLine($"{first}, {last}");
```

## On to patterns

Let's add a local function that prints a person's honors status. You can use some of the new pattern matching features for *relational patterns* to return the correct descriptions:

```csharp
WriteLine($"Person status: {PrintStudentHonorarium(p)}");
WriteLine($"Student status: {PrintStudentHonorarium(otherPerson)}");

static string PrintStudentHonorarium(Person p)
{
    if (p is Student s)
    {
        return s.Gpa switch
        {
            4.0 => "Distinguished honors",
            >= 3.5 => "High honors",
            >= 3.0 => "honors",
            > 1.0 => "Satisfactory",
            _ => "pass"
        };
    }
    else
    {
        return "graduate";
    }
}
```

This shows some of our newer patterns, and some of the earlier patterns as well. This `is` pattern match checks if the person is a student. That is from C# 7. We continue to invest in patterns, and the new syntax in the switch is from C# 9. These *relational patterns* provide a richer syntax for testing numeric values. In this case, the switch variable is a number, so the switch arms start with the relation operator. The variable isn't needed.

This can use some refactoring. There isn't a good `null` check. There's also a type check followed by a pattern testing the value of the `Gpa` property. These comparisons can be combined into a nested switch statement.

Let's try this next:

```csharp
static string PrintStudentHonorarium(Person p)
{
    return p switch
    {
        null => throw new ArgumentNullException(nameof(p), "Person can't be null"),
        Person _ => "graduate",
        Student s => s.Gpa switch
        {
            4.0 => "Distinguished honors",
            >= 3.5 => "High honors",
            >= 3.0 => "honors",
            > 1.0 => "Satisfactory",
            _ => "pass"
        },
    };
}
```

You'll have errors on the `Student` line. That's because that case is already handled by the `Person` case arm. You'll need to move the student above the `Person` arm.
The important point about this is that the compiler will warn you if you have switch arms in an order that prevents any from being reachable. You must arrange them so that each can be reached.

You could introduce an *and* pattern in the Student switch expression to process all students without honors first:

```csharp
static string PrintStudentHonorarium(Person p)
{
    return p switch
    {
        null => throw new ArgumentNullException(nameof(p), "Person can't be null"),
        Student s => s.Gpa switch
        {
            < 3.0 and > 1.0 => "Satisfactory",
            4.0 => "Distinguished honors",
            >= 3.5 => "High honors",
            >= 3.0 => "Honors",
            _ => "pass"
        },
        Person _ => "graduate",
    };
}
``` 

For other uses, you can aslo use `or` and `not` patterns. The `not null` patterns is a handy way to perform a null check on any variable.

## Summary

Go to the next slide.

Summarize the list of features. You saw all the features in the left column:

- Top level statements
- Init only setters
- Records
- Positional records
- Pattern matching enhancements

We didn't have time to cover all these other features:

- static anonymous functions: Like local functions, you can prevent anonymous functions (lambdas) from capturing variables
- Native sized integers: In some scenarios, you want an integral value to match the machine's natural CPU size. Use `nint` and `nuint`.
- Function pointers: Function pointers provide an easy syntax to access the IL opcodes `ldftn` and `calli`. You can declare function pointers using new `delegate*` syntax. These are useful for interop scenarios.
- Supress `localsinit`. This disables the standard .NET behavior to initialize memory used for local variables to all 0s. Disabling it in hot paths can improve performance.
- Partial method features. Partial methods no longer must not have any access modifiers and must return `void`. This supports code generators. However, to avoid any breaking change, any partial method that doesn't follow the existing rules must have an implementation.

Final slide. Download and use it.

https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-9

