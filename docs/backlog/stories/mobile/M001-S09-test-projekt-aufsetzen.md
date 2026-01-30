# Story M001-S09: Test-Projekt aufsetzen

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Entwickler** möchte ich **ein Test-Projekt mit notwendigen Mocking-Frameworks einrichten**, damit **ich Unit-Tests für ViewModels und Services schreiben kann**.

## Akzeptanzkriterien

- [ ] Gegeben das Test-Projekt, wenn es erstellt wird, dann referenziert es das Mobile-Projekt
- [ ] Gegeben xUnit, wenn es konfiguriert ist, dann können Tests ausgeführt werden
- [ ] Gegeben Moq, wenn es konfiguriert ist, dann können Interfaces gemockt werden
- [ ] Gegeben FluentAssertions, wenn es konfiguriert ist, dann können lesbare Assertions geschrieben werden

## Technische Hinweise

### Projektstruktur
```
/tests/TaschengeldManager.Mobile.Tests
  /ViewModels
    LoginViewModelTests.cs
    DashboardViewModelTests.cs
  /Services
    TokenServiceTests.cs
    ConnectivityServiceTests.cs
  /Helpers
    TestBase.cs
    MockServiceProvider.cs
```

### NuGet-Pakete
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
<PackageReference Include="Moq" Version="4.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="coverlet.collector" Version="6.*" />
```

### TestBase Klasse
```csharp
public abstract class TestBase
{
    protected Mock<INavigationService> NavigationServiceMock { get; }
    protected Mock<IConnectivityService> ConnectivityServiceMock { get; }
    protected Mock<ITokenService> TokenServiceMock { get; }
    protected Mock<IDatabaseService> DatabaseServiceMock { get; }

    protected TestBase()
    {
        NavigationServiceMock = new Mock<INavigationService>();
        ConnectivityServiceMock = new Mock<IConnectivityService>();
        TokenServiceMock = new Mock<ITokenService>();
        DatabaseServiceMock = new Mock<IDatabaseService>();

        // Standard-Setup: Online
        ConnectivityServiceMock
            .Setup(x => x.IsConnected)
            .Returns(true);
    }
}
```

### Beispiel-Test: LoginViewModel
```csharp
public class LoginViewModelTests : TestBase
{
    private readonly Mock<IAuthApi> _authApiMock;
    private readonly LoginViewModel _sut;

    public LoginViewModelTests()
    {
        _authApiMock = new Mock<IAuthApi>();

        _sut = new LoginViewModel(
            _authApiMock.Object,
            NavigationServiceMock.Object,
            TokenServiceMock.Object,
            ConnectivityServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_NavigatesToDashboard()
    {
        // Arrange
        _sut.Email = "test@example.com";
        _sut.Password = "password123";

        _authApiMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(new ApiResponse<LoginResponse>(
                new HttpResponseMessage(HttpStatusCode.OK),
                new LoginResponse { Token = "valid-token" },
                new RefitSettings()));

        // Act
        await _sut.LoginCommand.ExecuteAsync(null);

        // Assert
        NavigationServiceMock.Verify(
            x => x.NavigateToAsync("//main/dashboard"),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShowsError()
    {
        // Arrange
        _sut.Email = "test@example.com";
        _sut.Password = "wrong";

        _authApiMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(new ApiResponse<LoginResponse>(
                new HttpResponseMessage(HttpStatusCode.Unauthorized),
                null,
                new RefitSettings()));

        // Act
        await _sut.LoginCommand.ExecuteAsync(null);

        // Assert
        _sut.ErrorMessage.Should().NotBeNullOrEmpty();
        NavigationServiceMock.Verify(
            x => x.NavigateToAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("email@test.com", "")]
    [InlineData("", "")]
    public void CanLogin_WithMissingCredentials_ReturnsFalse(string email, string password)
    {
        // Arrange
        _sut.Email = email;
        _sut.Password = password;

        // Assert
        _sut.LoginCommand.CanExecute(null).Should().BeFalse();
    }
}
```

### CI/CD Test-Befehl
```bash
dotnet test tests/TaschengeldManager.Mobile.Tests --logger "trx" --collect:"XPlat Code Coverage"
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-30 | Test-Projekt kompilieren | Build erfolgreich |
| TC-M001-31 | dotnet test ausführen | Tests werden gefunden |
| TC-M001-32 | Mock erstellen und verwenden | Mock funktioniert |
| TC-M001-33 | FluentAssertions verwenden | Lesbare Fehlermeldungen |

## Story Points
2

## Priorität
Hoch
