using System;
using System.IO;
using Frends.Files.Delete.Definitions;
using NUnit.Framework;

namespace Frends.Files.Delete.Tests;

[TestFixture]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Throws<DirectoryNotFoundException>(() => Files.Delete(InvalidInput(), new Options(), default));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = Files.Delete(InvalidInput(), options, default);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = Assert.Throws<Exception>(() => Files.Delete(InvalidInput(), options, default));
        Assert.That(ex!.Message, Does.Contain(CustomErrorMessage));
    }

    private static Input InvalidInput() => new()
    {
        Directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NonExistingDirectory"),
        Pattern = "*",
    };
}
