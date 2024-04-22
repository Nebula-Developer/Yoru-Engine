using System;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Yoru.Tests;

public class TemporaryTests(ITestOutputHelper output) {
    [Fact]
    public void Temp() => output.WriteLine("Temporary test execution");
}