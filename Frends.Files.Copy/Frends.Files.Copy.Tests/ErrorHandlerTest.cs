using Frends.Files.Copy.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Frends.Files.Copy.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "File copy failed";

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var options = new Options();
        var ex = Assert.ThrowsAsync<DirectoryNotFoundException>(() =>
            Files.Copy(InvalidInput(), options, default));

        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = await Files.Copy(InvalidInput(), options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = Assert.ThrowsAsync<Exception>(async () => await Files.Copy(InvalidInput(), options, default));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Does.Contain(CustomErrorMessage));
    }

    private static Input InvalidInput()
    {
        return new Input
        {
            Directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")),
            Pattern = "*",
            TargetDirectory = Path.GetTempPath()
        };
    }
}
