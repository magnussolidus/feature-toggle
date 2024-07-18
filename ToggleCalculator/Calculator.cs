using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

namespace ToggleCalculator;

public class Calculator
{
    public double memory;

    private IFeatureManager _featureManager;

    private bool[] _operations;
    // private double _readerHelper;

    public Calculator(IFeatureManager featureManager)
    {
        memory = 0;
        _operations = new bool[sizeof(OperationsEnum)+1];
        _featureManager = featureManager;
    }

    public async Task RunCalculator()
    {
        Console.WriteLine("Please wait while we initialize the calculator...");
        var opr = Enum.GetValues<OperationsEnum>();
        foreach (var entry in opr)
        {
            _operations[(int)entry] = await ValidateOperations(entry);
        }
        Console.WriteLine("Flags read and validated, read to operate.");
        
        // TODO - Input Loop
    }

    private Task<bool> ValidateOperations(OperationsEnum operationToValidate)
    {
        var flagName = GetFlagName(operationToValidate);
        
        if (string.IsNullOrWhiteSpace(flagName))
        {
            Console.Error.WriteLine($"ERROR: Invalid Flag has been passed for operation {operationToValidate}!");
            return Task.FromResult(false);
        }

        return _featureManager.IsEnabledAsync(GetFlagName(operationToValidate));
    }

    private static string GetFlagName(OperationsEnum operation)
    {
        return operation switch
        {
            OperationsEnum.AllowNegativeNumbers => "AllowNegativeNumbers",
            OperationsEnum.Sum => "AllowSum",
            OperationsEnum.Subtraction => "AllowSubtraction",
            OperationsEnum.Multiplication => "AllowMultiplication",
            OperationsEnum.Division => "AllowDivision",
            _ => string.Empty
        };
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