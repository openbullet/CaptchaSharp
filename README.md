# CaptchaSharp
A .NET Standard 2.0 library that implements the APIs of the most used **captcha solving services** out there.
The library is fully documented, asynchronous and very easy to use. All services derive from the same `CaptchaService` class so you can allow users to use their favourite service without the need of a separate library for each one of them!

## Supported Captchas
This library supports the following captcha types
- Text (with language options)
- Image (with options like phrase, case sensitivity, calculations)
- FunCaptcha
- ReCaptcha V2 (invisible too) and V3
- HCaptcha
- KeyCaptcha
- GeeTest
- Capy

Although this sounds very exciting, sadly not all captcha types are supported by the services. You can find a table of the supported captcha types for each major service below.

![Supported Captcha Types](https://i.imgur.com/NMUiGRf.png)

## Adding CaptchaSharp to your project
Simply install the nuget package via

`Install-Package CaptchaSharp`

If you need more solvers you can install additional solvers (mostly ones that implement the 2captcha API) like this

`Install-Package CaptchaSharp.Services.More`

## Usage
First of all, initialize your solver of choice by providing your credentials, for example
```csharp
CaptchaService service = new TwoCaptchaService("MY_API_KEY");
```

You can get your remaining balance like this
```csharp
decimal balance = await service.GetBalanceAsync();
```

If the provided credentials are wrong, the method above will return a `BadAuthenticationException` that you can catch and process in order to let the user know he needs to check his credentials.

If the credentials are correct and the balance is greater than the minimum required for solving a captcha, we can proceed to solve a ReCaptchaV2 task (you will need to provide the required parameters found in the webpage source code).

```csharp
StringResponse solution = await service.SolveRecaptchaV2Async("site key", "site url");
```

The method above can throw the following exceptions:
- `TaskCreationException` when the task could not be created for example due to zero balance or incorrect parameters.
- `TaskSolutionException` when the task could not be completed, for example when an image captcha cannot be decoded.
- `TimeoutException` when the captcha solution took too long to complete.

You can configure a custom timeout like this

```csharp
service.Timeout = TimeSpan.FromMinutes(3);
```

The returned `StringResponse` will contain two fields:

An `Id` which you can use for reporting a bad solution (if the service supports it) like this
```csharp
await service.ReportSolution(solution.Id, CaptchaType.ReCaptchaV2);
```
if the report failed, the method above will throw a `TaskReportException`.

And finally your solution as plaintext
```csharp
Console.WriteLine($"The solution is {solution.Response}");
```

In addition, mind that not every service supports every type of captcha! If a method or specific parameters are not supported, a `NotSupportedException` will be thrown.

## SolverTester
Along with the library and its extension, the solution also includes a WPF based solver that can be used to quickly test most of the features of the library to see if the methods work properly.

#### Why not a unit test framework?
Implementing proper unit tests would have made little sense since valid credentials are needed for each service and sometimes they are overloaded and they won't solve captchas, so unit tests would have been unreliable and I decided to go for a manual testing approach.

## What needs to be improved
- Use C# anonymous types or `JObject` for the `AntiCaptchaSolver` instead of a million classes!
- Implement better exception handling, for example when there is zero balance or when a solver method returns a bad authentication, they will currently fall in the generic `TaskCreationException` type.
