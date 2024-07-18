namespace ToggleCalculator;

public class Calculator
{
    public double memory;
    private readonly bool acceptNegativeNumbers;

    public Calculator()
    {
        memory = 0;
        acceptNegativeNumbers = true;
    }

    public Calculator(bool negativeNumbers)
    {
        memory = 0;
        acceptNegativeNumbers = negativeNumbers;
    }

    public Calculator(double memoryValue, bool negativeNumber)
    {
        memory = memoryValue;
        acceptNegativeNumbers = negativeNumber;
    }

    public double Sum(double num1, double num2)
    {
        return num1 + num2;
    }

    public double Subtract(double num1, double num2)
    {
        return num1 - num2;
    }

    public double Multiply(double num1, double num2)
    {
        return num1 * num2;
    }

    public double Divide(double num1, double num2)
    {
        return num1 / num2;
    }
}