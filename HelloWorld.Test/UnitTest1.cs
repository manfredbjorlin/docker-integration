using Microsoft.AspNetCore.Mvc.Testing;
using HelloWorld;

namespace HelloWorld.Test;

public class UnitTest1
{
    [Fact]
    public async void Test1()
    {
        await using var application = new WebApplicationFactory<Program>();
    }
}