# Sử dụng hình ảnh .NET SDK chính thức
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao chép tệp csproj và khôi phục các dependency
COPY *.csproj ./
RUN dotnet restore

# Sao chép phần còn lại của ứng dụng và build
COPY . ./
RUN dotnet publish -c Release -o out

# Sử dụng hình ảnh ASP.NET Core runtime chính thức
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "pbl3_course.dll"]
