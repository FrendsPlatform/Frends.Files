using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Frends.Files.WriteBytes.Definitions;
using NUnit.Framework;

namespace Frends.Files.WriteBytes.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new Input
    {
        ContentBytes = "not a byte array",
        Path = Path.Combine(Path.GetTempPath(), "errorhandler-test.txt"),
    };

    private static Options DefaultOptions() => new Options { WriteBehaviour = WriteBehaviour.Overwrite };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() =>
            Files.WriteBytes(InvalidInput(), DefaultOptions(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = await Files.WriteBytes(InvalidInput(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.ThrowsAsync<Exception>(() =>
            Files.WriteBytes(InvalidInput(), options, CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Contains.Substring(CustomErrorMessage));
    }
}
