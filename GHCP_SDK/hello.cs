namespace Demo;

public class Greeter
{
    private readonly string _name;

    public Greeter(string name)
    {
        _name = name;
    }

    // Returns a personalised greeting
    public string SayHello() => $"Hello, {_name}! Welcome to the SDK demo.";

    // Returns how many characters are in the name
    public int NameLength() => _name.Length;
}
