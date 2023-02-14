using Frends.Files.Copy.Definitions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;


namespace Frends.Files.Copy.Tests;

[TestFixture]
public class UnitTests
{
    private readonly string _dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/");
    Input? _input;
    Options? _options;

    [SetUp]
    public void Setup()
    {
        Helper.CreateTestFiles(_dir);

        _input = new Input
        {
            Directory = _dir,
            Pattern = "*"
        };

        _options = new Options
        {
            UseGivenUserCredentialsForRemoteConnections = false
        };
    }

    [TearDown]
    public void TearDown()
    {
        Helper.DeleteTestFolder(_dir);
    }

    
}
