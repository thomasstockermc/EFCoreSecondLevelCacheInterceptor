dotnet tool install --global dotnet-ef --version 3.1.1
dotnet tool update --global dotnet-ef --version 3.1.1
dotnet build
dotnet ef --startup-project ../EFCoreSecondLevelCacheInterceptor.AspNetCoreSample/ database update --context ApplicationDbContext
pause