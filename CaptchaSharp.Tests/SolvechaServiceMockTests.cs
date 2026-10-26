using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Services;
using Xunit;

namespace CaptchaSharp.Tests;

// These tests use a fake HTTP handler, so they do not need an API key.
public class SolvechaServiceMockTests
{
    private class FakeHandler(Func<HttpRequestMessage, string> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(respond(request))
            });
    }

    private static SolvechaService CreateService(Func<HttpRequestMessage, string> respond)
        => new("key", new HttpClient(new FakeHandler(respond)))
        {
            PollingInterval = TimeSpan.FromMilliseconds(10)
        };

    [Fact]
    public async Task SolveImageCaptchaAsync_ReadyImmediately_ReturnsText()
    {
        using var service = CreateService(_ =>
            """{"errorId":0,"status":"ready","taskId":"42","solution":{"text":"w68hp"}}""");

        var result = await service.SolveImageCaptchaAsync("aGk=");

        Assert.Equal("42", result.Id);
        Assert.Equal("w68hp", result.Response);
    }

    [Fact]
    public async Task SolveImageCaptchaAsync_Processing_PollsUntilReady()
    {
        var polls = 0;
        using var service = CreateService(request =>
        {
            if (request.RequestUri!.AbsolutePath == "/createTask")
            {
                return """{"errorId":0,"status":"processing","taskId":"42"}""";
            }

            return ++polls < 2
                ? """{"errorId":0,"status":"processing","taskId":"42"}"""
                : """{"errorId":0,"status":"ready","taskId":"42","solution":{"text":"abc"}}""";
        });

        var result = await service.SolveImageCaptchaAsync("aGk=");

        Assert.Equal("abc", result.Response);
        Assert.Equal(2, polls);
    }

    [Fact]
    public async Task SolveImageCaptchaAsync_BadKey_ThrowsTaskCreationException()
    {
        using var service = CreateService(_ =>
            """{"errorId":1,"errorCode":"ERROR_KEY_DENIED_ACCESS","errorDescription":"The clientKey is missing or wrong."}""");

        var ex = await Assert.ThrowsAsync<TaskCreationException>(() => service.SolveImageCaptchaAsync("aGk="));
        Assert.Contains("ERROR_KEY_DENIED_ACCESS", ex.Message);
    }

    [Fact]
    public async Task GetBalanceAsync_ValidKey_ReturnsBalance()
    {
        using var service = CreateService(_ => """{"errorId":0,"balance":4.9998}""");

        Assert.Equal(4.9998m, await service.GetBalanceAsync());
    }

    [Fact]
    public async Task GetBalanceAsync_BadKey_ThrowsBadAuthenticationException()
    {
        using var service = CreateService(_ =>
            """{"errorId":1,"errorCode":"ERROR_KEY_DENIED_ACCESS","errorDescription":"The clientKey is missing or wrong."}""");

        await Assert.ThrowsAsync<BadAuthenticationException>(() => service.GetBalanceAsync());
    }

    [Fact]
    public async Task SolveRecaptchaV2Async_NotSupported()
    {
        using var service = CreateService(_ => "{}");

        await Assert.ThrowsAsync<NotSupportedException>(
            () => service.SolveRecaptchaV2Async("sitekey", "https://example.com"));
    }
}
