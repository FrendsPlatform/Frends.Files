using System;
using System.Threading;
using Frends.Files.DeleteDirectory.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Files.DeleteDirectory.Tests;

[TestClass]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            Files.DeleteDirectory(new Input(), new Options(), CancellationToken.None));
    }

    [TestMethod]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = Files.DeleteDirectory(new Input(), options, CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("Directory cannot be empty.", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var exception = Assert.ThrowsException<Exception>(() =>
            Files.DeleteDirectory(new Input(), options, CancellationToken.None));

        Assert.IsTrue(exception.Message.Contains(CustomErrorMessage, StringComparison.Ordinal));
    }
}
