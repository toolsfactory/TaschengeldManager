var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("taschengelddb");

var valkey = builder.AddValkey("cache");

// Backend API
var api = builder.AddProject<Projects.TaschengeldManager_Api>("api")
    .WithReference(postgres)
    .WithReference(valkey)
    .WaitFor(postgres)
    .WaitFor(valkey);

// Frontend Web Client (React)
builder.AddNpmApp("react", "../TaschengeldManager.React", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(port: 5173, env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
