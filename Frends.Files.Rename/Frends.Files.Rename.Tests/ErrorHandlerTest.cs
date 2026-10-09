using Frends.Files.Rename.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Threading;

namespace Frends.Files.Rename.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new()
    {
        Path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "missing.txt"),
        NewFileName = "new.txt",
    };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        Assert.Throws<DirectoryNotFoundException>(() =>
            Files.Rename(InvalidInput(), new Options(), CancellationToken.None));
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var result = Files.Rename(InvalidInput(), new Options { ThrowErrorOnFailure = false }, CancellationToken.None);
        ClassicAssert.IsFalse(result.Success);
        ClassicAssert.IsNotNull(result.Error);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var ex = Assert.Throws<Exception>(() =>
            Files.Rename(InvalidInput(), new Options { ErrorMessageOnFailure = CustomErrorMessage }, CancellationToken.None));
        StringAssert.Contains(CustomErrorMessage, ex.Message);
    }
}
