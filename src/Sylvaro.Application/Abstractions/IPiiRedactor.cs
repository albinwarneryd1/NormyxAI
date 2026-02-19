namespace Sylvaro.Application.Abstractions;

public interface IPiiRedactor
{
    string Redact(string input);
}
