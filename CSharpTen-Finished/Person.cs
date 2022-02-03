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
