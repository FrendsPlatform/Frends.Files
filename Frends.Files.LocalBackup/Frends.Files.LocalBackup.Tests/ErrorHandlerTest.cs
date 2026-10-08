using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using Frends.Files.LocalBackup.Definitions;
using NUnit.Framework;

namespace Frends.Files.LocalBackup.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "Backup operation failed";

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            Files.LocalBackup(new Input(), new Options(), CancellationToken.None));

        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = Files.LocalBackup(new Input(), options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = Assert.Throws<Exception>(() =>
            Files.LocalBackup(new Input(), options, CancellationToken.None));

        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Does.Contain(CustomErrorMessage));
    }
}
