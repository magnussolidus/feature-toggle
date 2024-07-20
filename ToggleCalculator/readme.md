# Toggle Calculator

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

