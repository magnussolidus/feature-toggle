using Microsoft.FeatureManagement;

namespace ToggleCalculator;

public class Calculator(IFeatureManager featureManager, bool useCache)
{
    // public double memory;

    private readonly bool[] _operations = useCache ? new bool[sizeof(OperationsEnum) + 1] : [false];

    private readonly List<OperationsEnum> _allowedOperations = new();

    // memory = 0;

    public async Task RunCalculator()
    {
        Console.WriteLine("Please wait while we initialize the calculator...");
        await CheckFlagOperations();
        OperationsEnum operation;
        do
        {
            Console.WriteLine("Please, select your operation: (Select 0 to exit)");
            DisplayAllowedOperations();
            var userInput = Console.ReadLine();
            operation = SelectOperation(userInput ?? string.Empty);
            await ExecuteOperation(operation);
        } while (operation != OperationsEnum.CloseProgram);
    }

    private async Task CheckFlagOperations()
    {
        var featureNamesAsync = featureManager.GetFeatureNamesAsync();
        
        await foreach (var featureName in featureNamesAsync)
        {
            var op = GetFlagName(featureName);
            if (!await ValidateOperations(op))
            {
                continue;
            }
            
            _allowedOperations.Add(op);
            if (useCache)
            {
                _operations[(int)op] = await ValidateOperations(op);
            }
        }
        
        Console.WriteLine("Flags read and validated, read to operate!");
    }

    private Task<bool> ValidateOperations(OperationsEnum operationToValidate)
    {
        var flagName = GetFlagName(operationToValidate);
        
        if (string.IsNullOrWhiteSpace(flagName))
        {
            Console.Error.WriteLine($"ERROR: Invalid Flag has been passed for operation {operationToValidate}!");
            return Task.FromResult(false);
        }

        return featureManager.IsEnabledAsync(GetFlagName(operationToValidate));
    }

    private async Task<double> GetUserNumberInput()
    {
        bool canParse;
        do
        {
            Console.WriteLine("Please insert a number for the next operation:");
            var input = Console.ReadLine()?.Trim();
            canParse = double.TryParse(input, out var output);

            if (!canParse)
            {
                Console.WriteLine("Invalid input! Try again with a valid input!");
                continue;
            }
            
            // check for negative numbers
            if (output >= 0)
            {
                return output;
            }

            if (!useCache && (await featureManager.IsEnabledAsync(GetFlagName(OperationsEnum.AllowNegativeNumbers))))
            {
                return output;
            }

            if (_operations[(int)OperationsEnum.AllowNegativeNumbers])
            {
                return output;
            }

            canParse = false;
            Console.WriteLine("Negative numbers are not allowed. Please try again.");


        } while (canParse == false);

        return double.NaN;
    }

    private void DisplayAllowedOperations()
    {
        foreach (var operation in _allowedOperations.Where(
                     operation => 
                         operation != OperationsEnum.CacheFeatures && 
                         operation != OperationsEnum.AllowNegativeNumbers))
        {
            Console.WriteLine($"{(int)operation} -  {operation}");
        }
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

    private OperationsEnum SelectOperation(string input)
    {
        if (string.Equals(input.Trim(), "exit") || string.Equals(input.Trim(), "0"))
        {
            return OperationsEnum.CloseProgram;
        }
        
        var parseResult = int.TryParse(input.Trim(), out int result);
        if (!parseResult)
        {
            Console.WriteLine("Invalid Input!");
            return OperationsEnum.InvalidOperation;
        }

        var chosenOperation = (OperationsEnum)result;

        if (_allowedOperations.Any(x => x == chosenOperation) == false)
        {
            Console.WriteLine("Invalid Operation! Please select a valid option!");
            return OperationsEnum.InvalidOperation;
        }

        return chosenOperation;
    }

    private async Task ExecuteOperation(OperationsEnum operationType)
    {
        var result = operationType switch
        {
            OperationsEnum.Sum => await Sum(),
            OperationsEnum.Subtraction => await Subtract(),
            OperationsEnum.Multiplication => await Multiply(),
            OperationsEnum.Division => await Divide(),
            _ => 0D
        };
        
        Console.WriteLine($"Result: {result}");
    }

    private async Task<double> Sum()
    {
        Console.WriteLine($"You choose the operation: {OperationsEnum.Sum}");
        var num1 = await GetUserNumberInput();
        var num2 = await GetUserNumberInput();
        
        return num1 + num2;
    }

    private async Task<double> Subtract()
    {
        Console.WriteLine($"You choose the operation: {OperationsEnum.Subtraction}");
        var num1 = await GetUserNumberInput();
        var num2 = await GetUserNumberInput();
        return num1 - num2;
    }

    private async Task<double> Multiply()
    {
        Console.WriteLine($"You choose the operation: {OperationsEnum.Multiplication}");
        var num1 = await GetUserNumberInput();
        var num2 = await GetUserNumberInput();
        return num1 * num2;
    }

    private async Task<double> Divide()
    {
        Console.WriteLine($"You choose the operation: {OperationsEnum.Division}");
        var num1 = await GetUserNumberInput();
        double num2;
        do
        {
            num2 = await GetUserNumberInput();
            Console.WriteLine("You can not divide by 0! Please select a different number.");
        } while (num2 == 0);
        return num1 / num2;
    }
}