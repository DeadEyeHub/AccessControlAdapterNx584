using AccessControlAdapterSample;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
	options.ServiceName = "Access Control Adapter Sample Service";
});

builder.Services.AddHostedService<AccessControlAdapterService>();

var host = builder.Build();
host.Run();
