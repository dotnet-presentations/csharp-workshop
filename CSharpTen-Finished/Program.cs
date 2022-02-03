using Model;
using static System.Console;

var person = new
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

Person p1 = default;
Person p2 = new();
