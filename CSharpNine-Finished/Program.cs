using System;
using static System.Console;

Person person = new Student("Scott", "Hunter")
{
    Gpa = 3.8,
};

var otherPerson = person with
{
    LastName = "Hanselman"
};

WriteLine(person);
WriteLine(otherPerson);

var originalPerson = otherPerson with
{
    LastName = "Hunter"
};

WriteLine($"Equals: {Equals(person, originalPerson)}");
WriteLine($"Reference Equals: {ReferenceEquals(person, originalPerson)}");
var p = new Person("Scott", "Hunter");
WriteLine($"Person and Student: Equals: {Equals(person, p)}");

var (first, last) = person;
WriteLine($"{first}, {last}");

WriteLine($"Person status: {PrintStudentHonorarium(p)}");
WriteLine($"Student status: {PrintStudentHonorarium(otherPerson)}");


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

record Person(string FirstName, string LastName);

record Student(string FirstName, string LastName) : Person(FirstName, LastName)
{
    public double Gpa { get; set; } = 4.0;
}
