using System.IO;
using static System.Console;

var person = new Person
{
    FirstName = "Scott",
    LastName = "Hunter"
};

var otherPerson = person with { LastName = "Hanselman" };

WriteLine(person);
WriteLine(otherPerson);

var originalPerson = otherPerson with { LastName = "Hunter" };

WriteLine(originalPerson);
WriteLine($"Equals: {Equals(person, originalPerson)}");
WriteLine($"== operator: {person == originalPerson}");

record Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }

    public void WriteToFile(string filePath)
        => File.WriteAllText(filePath, ToString());
}
