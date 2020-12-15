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
