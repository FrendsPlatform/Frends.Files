using System;
using Frends.Files.CreateDirectory.Definitions;
using NUnit.Framework;

namespace Frends.Files.CreateDirectory.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "Directory creation failed";

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var options = new Options();
        var ex = Assert.Throws<ArgumentNullException>(() =>
            Files.CreateDirectory(new Input(), options));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = Files.CreateDirectory(new Input(), options);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = Assert.Throws<Exception>(() => Files.CreateDirectory(new Input(), options));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Does.Contain(CustomErrorMessage));
    }
}
