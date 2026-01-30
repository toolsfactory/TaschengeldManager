# Story M012-S01: Eigenes Profil anzeigen

## Epic

M012 - Profil & Account-Verwaltung

## User Story

Als **Benutzer** moechte ich **mein eigenes Profil mit allen wichtigen Informationen einsehen koennen**, damit **ich einen Ueberblick ueber meine Account-Daten habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Benutzer, wenn er sein Profil oeffnet, dann sieht er seinen Namen, E-Mail und Avatar
- [ ] Gegeben ein Elternteil, wenn er sein Profil sieht, dann wird seine Rolle als "Elternteil" angezeigt
- [ ] Gegeben ein Kind, wenn es sein Profil sieht, dann wird seine Rolle als "Kind" und das Alter angezeigt
- [ ] Gegeben ein Profil, wenn der Benutzer es sieht, dann sind alle Bearbeitungs-Optionen erreichbar

## UI-Entwurf

```
+------------------------------------+
|  <- Zurueck           Profil       |
+------------------------------------+
|                                    |
|            [ Avatar ]              |
|                                    |
|         Max Mustermann             |
|         max@example.com            |
|                                    |
|         Rolle: Elternteil          |
|         Mitglied seit: Jan. 2025   |
|                                    |
+------------------------------------+
|                                    |
|  +--------------------------------+|
|  | [Edit] Profil bearbeiten    > ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | [Lock] Passwort aendern     > ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | [Mail] Email aendern        > ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | [Shield] Zwei-Faktor-Auth   > ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | [Privacy] Datenschutz       > ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | [Export] Daten exportieren  > ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Trash] Account loeschen    > ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkt

```
GET /api/users/me
Authorization: Bearer {token}

Response 200:
{
  "id": "guid",
  "email": "max@example.com",
  "firstName": "Max",
  "lastName": "Mustermann",
  "avatarUrl": "https://...",
  "role": "Parent",
  "createdAt": "2025-01-15T10:00:00Z",
  "mfaEnabled": true,
  "emailVerified": true
}
```

## Technische Notizen

- ViewModel: `ProfileViewModel`
- Navigation zu Unterseiten via Shell
- Avatar: Standardbild wenn keins gesetzt
- Rolle lokalisiert: "Parent" -> "Elternteil", "Child" -> "Kind"

## Implementierungshinweise

```csharp
public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _avatarUrl = string.Empty;

    [ObservableProperty]
    private string _role = string.Empty;

    [ObservableProperty]
    private DateTime _memberSince;

    [ObservableProperty]
    private bool _mfaEnabled;

    public string FullName => $"{FirstName} {LastName}";

    public string RoleDisplay => Role switch
    {
        "Parent" => "Elternteil",
        "Child" => "Kind",
        "Relative" => "Verwandter",
        _ => Role
    };

    public string MemberSinceDisplay => MemberSince.ToString("MMM yyyy", new CultureInfo("de-DE"));

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        var user = await _userService.GetCurrentUserAsync();

        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        AvatarUrl = user.AvatarUrl ?? "default_avatar.png";
        Role = user.Role;
        MemberSince = user.CreatedAt;
        MfaEnabled = user.MfaEnabled;
    }

    [RelayCommand]
    private async Task NavigateToEditProfileAsync()
        => await Shell.Current.GoToAsync("editProfile");

    [RelayCommand]
    private async Task NavigateToChangePasswordAsync()
        => await Shell.Current.GoToAsync("changePassword");

    [RelayCommand]
    private async Task NavigateToChangeEmailAsync()
        => await Shell.Current.GoToAsync("changeEmail");

    [RelayCommand]
    private async Task NavigateToMfaSettingsAsync()
        => await Shell.Current.GoToAsync("mfaSettings");

    [RelayCommand]
    private async Task NavigateToPrivacySettingsAsync()
        => await Shell.Current.GoToAsync("privacySettings");

    [RelayCommand]
    private async Task NavigateToDataExportAsync()
        => await Shell.Current.GoToAsync("dataExport");

    [RelayCommand]
    private async Task NavigateToDeleteAccountAsync()
        => await Shell.Current.GoToAsync("deleteAccount");
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Profil laden | Name, Email, Avatar werden angezeigt |
| TC-002 | Elternteil-Rolle | "Elternteil" wird angezeigt |
| TC-003 | Kind-Rolle | "Kind" wird angezeigt |
| TC-004 | Kein Avatar gesetzt | Standardbild wird angezeigt |
| TC-005 | Navigation zu Unterseiten | Funktioniert fuer alle Optionen |

## Story Points

1

## Prioritaet

Hoch
