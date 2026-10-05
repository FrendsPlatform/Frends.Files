using Frends.Files.Find.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace Frends.Files.Find.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "File discovery failed";

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var options = new Options();
        var ex = Assert.Throws<DirectoryNotFoundException>(() =>
            Files.Find(new Input { Directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")) }, options, CancellationToken.None));

        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = Files.Find(new Input { Directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")) }, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = Assert.Throws<Exception>(() =>
            Files.Find(new Input { Directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")) }, options, CancellationToken.None));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Does.Contain(CustomErrorMessage));
    }

    [Test]
    public void Should_Throw_When_CancellationToken_Is_Canceled()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var options = new Options { ThrowErrorOnFailure = false };
        Assert.Throws<OperationCanceledException>(() =>
            Files.Find(new Input(), options, cancellationTokenSource.Token));
    }
}
