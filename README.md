# Feature Toggle Experiments

This repository purpose is for one to experiment with the [native feature toggle management](https://learn.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement?view=azure-dotnet) provided by microsoft.

I am making a Calculator that deals with some async code and with the feature management. 
Later I will add a Rules Engine to help setting up the features.

> **Note: This is a Work In Progress repo**

## Toggle Calculator

This is an async calculator that has its features controlled by the [Feature Management](https://learn.microsoft.com/en-us/dotnet/api/microsoft.featuremanagement?view=azure-dotnet) resource.

You can control its behaviour solely by adjusting the `appsettings.json` file.

Currently, you can control the following:

* Basic Operations
    * Sum
    * Subtraction
    * Multiplication
    * Division
* Allow Negative Numbers
    * When disabled, negative inputs are **NOT** accepted
* Allow Cache of the Features
    * When Enabled, the program will cache the Feature values, and it won't check it again before doing any operations.
    * When Disabled, the program will validate if the operation is allowed, by checking if it's feature is enabled.

# Version

This project uses .NET 8 and some nuget packages that are in a preview state.

# License

This project is under the [GNU GPL v3.0](LICENSE) license. For more info, read the license agreement. 