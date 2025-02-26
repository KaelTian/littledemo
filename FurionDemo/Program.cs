using Furion;

Serve.Run();

//WebApplicationBuilder builder = new WebApplicationBuilder();
//builder.Inject

[DynamicApiController]
public class HelloService
{
    public string Say() => "Hello,Furion " + App.Configuration["Logging:LogLevel:Microsoft.AspNetCore"];
}