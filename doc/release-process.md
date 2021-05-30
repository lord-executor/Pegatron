# Release Process
Build and package the project with the _Release_ configuration.

```
dotnet pack .\src\Pegatron.sln -c Release
```

Publish package with
```
# Using https://github.com/lord-executor/pwshrun to retrieve API key from windows credentials
# store using the Read-CredentialsStore Cmdlet
$apiKey = (Read-CredentialsStore -Target "NuGet:Pegatron:APIKey").GetNetworkCredential().Password
dotnet nuget push src\Pegatron\bin\Release\Pegatron.[X.Y.Z].nupkg -k $apiKey -s https://api.nuget.org/v3/index.json
```
