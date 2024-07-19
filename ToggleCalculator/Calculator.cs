using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

namespace ToggleCalculator;

public class Calculator
{
    public double memory;

    private IFeatureManager _featureManager;
    private bool _useCache;
    private bool[] _operations;
    // private double _readerHelper;

    public Calculator(IFeatureManager featureManager, bool useCache)
    {
        memory = 0;
        _featureManager = featureManager;
        _operations = useCache ? new bool[sizeof(OperationsEnum) + 1] : null;
        _useCache = useCache;
    }

    public async Task RunCalculator()
    {
        Console.WriteLine("Please wait while we initialize the calculator...");
        await CheckFlagOperations();
        
        // TODO - Input Loop
    }

    private async Task CheckFlagOperations()
    {
        var allowedOperations = new List<OperationsEnum>();
        var featureNamesAsync = _featureManager.GetFeatureNamesAsync();
        
        await foreach (var featureName in featureNamesAsync)
        {
            var op = GetFlagName(featureName);
            if(await ValidateOperations(op))
            {
                allowedOperations.Add(op);
                if (_useCache)
                {
                    _operations[(int)op] = await ValidateOperations(op);
                }
            }
        }
        Console.WriteLine("Flags read and validated, read to operate!\nCurrent allowed operations:");
        foreach (var operation in allowedOperations)
        {
            Console.WriteLine($"{operation}");
        }
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

    private double GetUserNumberInput()
    {
        bool canParse;
        do
        {
            Console.WriteLine("Please insert a number for the next operation:");
            var input = Console.ReadLine().Trim();
            canParse = double.TryParse(input, out var output);

            if (canParse) 
                return output;
            Console.WriteLine("Invalid input! Try again with a valid input!");

        } while (canParse == false);

        return double.NaN;
    }

    [Obsolete]
    private async void CacheFeatures()
    {
        var opr = Enum.GetValues<OperationsEnum>();
        foreach (var entry in opr)
        {
            _operations[(int)entry] = await ValidateOperations(entry);
        }
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
            OperationsEnum.CacheFeatures => "CacheFlags",
            _ => string.Empty
        };
    }

    private static OperationsEnum GetFlagName(string featureName)
    {
        return featureName switch
        {
            "AllowNegativeNumbers" => OperationsEnum.AllowNegativeNumbers,
            "AllowSum" => OperationsEnum.Sum,
            "AllowSubtraction" => OperationsEnum.Subtraction,
            "AllowMultiplication" => OperationsEnum.Multiplication,
            "AllowDivision" => OperationsEnum.Division,
            "CacheFlags" => OperationsEnum.CacheFeatures,
            _ => throw new NotImplementedException()
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