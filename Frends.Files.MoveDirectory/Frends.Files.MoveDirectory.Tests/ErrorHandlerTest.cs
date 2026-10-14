using System;
using System.IO;
using System.Threading;
using Frends.Files.MoveDirectory.Definitions;
using NUnit.Framework;

namespace Frends.Files.MoveDirectory.Tests;

[TestFixture]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new()
    {
        SourceDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()),
        TargetDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()),
    };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Throws<DirectoryNotFoundException>(() =>
            Files.MoveDirectory(InvalidInput(), new Options(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var result = Files.MoveDirectory(InvalidInput(), new Options { ThrowErrorOnFailure = false }, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.AdditionalInfo, Is.InstanceOf<DirectoryNotFoundException>());
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var ex = Assert.Throws<Exception>(() =>
            Files.MoveDirectory(InvalidInput(), new Options { ErrorMessageOnFailure = CustomErrorMessage }, CancellationToken.None));
        Assert.That(ex.Message, Contains.Substring(CustomErrorMessage));
    }
}
