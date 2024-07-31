# CaptchaSharp
A .NET library that implements the APIs of the most used **captcha solving services** out there.
The library is fully documented, asynchronous and very easy to use.

All services derive from the same `CaptchaService` class and have the same code API, so it's very easy to switch between services!

## Supported Captcha Types
This library supports the following captcha types
- Text (with language options)
- Image (with options like phrase, case sensitivity, calculations)
- ReCaptcha V2 (incl. invisible, enterprise)
- ReCaptcha V3 (incl. enterprise)
- FunCaptcha
- HCaptcha
- KeyCaptcha
- GeeTest
- Capy
- DataDome
- Cloudflare Turnstile
- Lemin Cropped
- Amazon WAF
- Cyber SiARA
- MT Captcha
- CutCaptcha
- Friendly Captcha
- atb Captcha
- Tencent Captcha
- Audio Captcha (with language options)

Proxies are supported for services that support them.

Not every captcha type is supported by each service. You can find a spreadsheet with a breakdown of the supported captcha types for each implemented service at the following link

[CaptchaSharp Services Availability](https://1drv.ms/x/s!Al8HxSfx2JL3ePfRK23aUt34eCk?e=WNCPh9)

## Adding CaptchaSharp to your project
Simply install the nuget package via

`Install-Package CaptchaSharp`

## Usage
First of all, initialize your solver of choice by providing your credentials, for example
```csharp
CaptchaService service = new TwoCaptchaService("MY_API_KEY");
```

You can get your remaining balance like this
```csharp
decimal balance = await service.GetBalanceAsync();
```

If the provided credentials are wrong, the method above will return a `BadAuthenticationException`.

If the credentials are correct and the balance is greater than the minimum required for solving a captcha, you can proceed to solve a captcha (e.g., a ReCaptchaV2 task) like this.

```csharp
StringResponse solution = await service.SolveRecaptchaV2Async("site key", "site url");
```

The method above can throw the following exceptions:
- `TaskCreationException` when the task could not be created, for example due to zero balance or incorrect parameters.
- `TaskSolutionException` when the task could not be completed, for example when an image captcha cannot be decoded.
- `TimeoutException` when the captcha solution took too long to complete.

You can configure a custom timeout like this

```csharp
service.Timeout = TimeSpan.FromMinutes(3);
```

The returned solution will contain two fields:

-   an `Id` which you can use for reporting a bad solution (if the service supports it) like this
    ```csharp
    await service.ReportSolution(solution.Id, CaptchaType.ReCaptchaV2);
    ```
    if the report failed, the method above will throw a `TaskReportException`.


-   your solution as plaintext
    ```csharp
    Console.WriteLine($"The solution is {solution.Response}");
    ```

If a method or some of its parameters are not supported, a `NotSupportedException` or `ArgumentException` will be thrown.

## The service I want to use is not implemented
If the service you want to use is not implemented, you can easily implement it yourself by deriving from the `CaptchaService` class and implementing the abstract methods, or you can open an issue, and we will try to implement it as soon as possible.

## Unit Tests
Unit tests are included in the `CaptchaSharp.Tests` project. In order to test, you need to:
1. Run any test once and let it fail. It will create a file called `config.json` in your `bin/Debug/net8.0` folder.
2. Add your credentials to the `config.json` file. You can also add a proxy to test proxied endpoints.
3. Run the tests (one at a time, to avoid overloading the service and overspending).
4. If you need to test a captcha on a specific website, you can edit the `ServiceTests` class and change the parameters as you need.

## What needs to be improved
- Drop `Newtonsoft.Json` for `System.Text.Json`
- `StringPairCollection.Add()` should also take null values, and by default ignore the key-value pair if the value is null.
- Implement better exception handling for specific error codes. For example when there is zero balance or when a solver method returns a bad authentication, they will currently fall in the generic `TaskCreationException` type.
- Add support for recognition APIs (right now only token-based APIs are supported).
