using Microsoft.AspNetCore.Mvc.Testing;
using HelloWorld;

namespace HelloWorld.Test;

public class UnitTest1
{
    [Fact]
    public async void TestDefault()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var response = await client.GetStringAsync("/");

        Assert.Equal("You shouldn't be here!", response);
    }
}