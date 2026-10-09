using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Frends.Files.ReadBytes.Definitions;
using NUnit.Framework;

namespace Frends.Files.ReadBytes.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input MissingFileInput() => new() { Path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "missing.txt") };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() =>
            Files.ReadBytes(MissingFileInput(), new Options(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var result = await Files.ReadBytes(MissingFileInput(), new Options { ThrowErrorOnFailure = false }, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = Assert.ThrowsAsync<Exception>(() =>
            Files.ReadBytes(MissingFileInput(), options, CancellationToken.None));
        Assert.That(ex.Message, Contains.Substring(CustomErrorMessage));
    }
}
