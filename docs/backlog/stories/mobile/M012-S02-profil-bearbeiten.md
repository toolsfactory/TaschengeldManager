# Story M012-S02: Profil bearbeiten

## Epic

M012 - Profil & Account-Verwaltung

## User Story

Als **Benutzer** moechte ich **meinen Namen und Avatar aendern koennen**, damit **mein Profil aktuell und persoenlich ist**.

## Akzeptanzkriterien

- [ ] Gegeben ein Benutzer im Bearbeitungsmodus, wenn er seinen Vornamen aendert, dann wird die Aenderung gespeichert
- [ ] Gegeben ein Benutzer im Bearbeitungsmodus, wenn er seinen Nachnamen aendert, dann wird die Aenderung gespeichert
- [ ] Gegeben ein Benutzer, wenn er ein neues Avatar-Bild auswaehlt, dann wird es hochgeladen und gespeichert
- [ ] Gegeben eine erfolgreiche Aenderung, wenn sie gespeichert wird, dann wird eine Bestaetigung angezeigt
- [ ] Gegeben ungueltige Eingaben, wenn der Benutzer speichern will, dann werden Validierungsfehler angezeigt

## UI-Entwurf

```
+------------------------------------+
|  <- Zurueck    Profil bearbeiten   |
+------------------------------------+
|                                    |
|            [ Avatar ]              |
|         [Bild aendern]             |
|                                    |
|  Vorname                           |
|  +--------------------------------+|
|  | Max                            ||
|  +--------------------------------+|
|                                    |
|  Nachname                          |
|  +--------------------------------+|
|  | Mustermann                     ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |       [Save] Speichern         ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### Avatar-Auswahl Dialog
```
+------------------------------------+
|        Profilbild aendern          |
+------------------------------------+
|                                    |
|  [Camera] Foto aufnehmen           |
|                                    |
|  [Gallery] Aus Galerie waehlen     |
|                                    |
|  [Trash] Bild entfernen            |
|                                    |
|  [X] Abbrechen                     |
|                                    |
+------------------------------------+
```

## API-Endpunkte

### Profil aktualisieren
```
PUT /api/users/me
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "Max",
  "lastName": "Mustermann"
}

Response 200:
{
  "id": "guid",
  "firstName": "Max",
  "lastName": "Mustermann",
  "updatedAt": "2026-01-20T10:00:00Z"
}

Response 400:
{
  "errors": {
    "firstName": ["Vorname darf nicht leer sein"],
    "lastName": ["Nachname darf nicht leer sein"]
  }
}
```

### Avatar hochladen
```
POST /api/users/me/avatar
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- avatar: (file)

Response 200:
{
  "avatarUrl": "https://storage.../avatar_guid.jpg",
  "updatedAt": "2026-01-20T10:00:00Z"
}

Response 400:
{
  "error": "Datei zu gross (max. 5 MB)"
}
```

### Avatar entfernen
```
DELETE /api/users/me/avatar
Authorization: Bearer {token}

Response 204 No Content
```

## Technische Notizen

- ViewModel: `EditProfileViewModel`
- Foto-Auswahl: `MediaPicker` von MAUI
- Avatar-Groesse: max. 5 MB, wird auf 200x200 verkleinert
- Validierung: Vor- und Nachname nicht leer, max. 50 Zeichen

## Implementierungshinweise

```csharp
public partial class EditProfileViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IMediaService _mediaService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _avatarUrl = string.Empty;

    [ObservableProperty]
    private ImageSource? _avatarPreview;

    private FileResult? _selectedImage;

    private bool CanSave =>
        !string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        FirstName.Length <= 50 &&
        LastName.Length <= 50;

    [RelayCommand]
    private async Task ChangeAvatarAsync()
    {
        var action = await Application.Current!.MainPage!.DisplayActionSheet(
            "Profilbild aendern",
            "Abbrechen",
            "Bild entfernen",
            "Foto aufnehmen",
            "Aus Galerie waehlen");

        switch (action)
        {
            case "Foto aufnehmen":
                await TakePhotoAsync();
                break;
            case "Aus Galerie waehlen":
                await PickPhotoAsync();
                break;
            case "Bild entfernen":
                await RemoveAvatarAsync();
                break;
        }
    }

    private async Task TakePhotoAsync()
    {
        if (!MediaPicker.IsCaptureSupported)
        {
            await _toastService.ShowErrorAsync("Kamera nicht verfuegbar");
            return;
        }

        _selectedImage = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = "Profilbild aufnehmen"
        });

        if (_selectedImage != null)
        {
            AvatarPreview = ImageSource.FromFile(_selectedImage.FullPath);
        }
    }

    private async Task PickPhotoAsync()
    {
        _selectedImage = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Profilbild auswaehlen"
        });

        if (_selectedImage != null)
        {
            AvatarPreview = ImageSource.FromFile(_selectedImage.FullPath);
        }
    }

    private async Task RemoveAvatarAsync()
    {
        await _userService.RemoveAvatarAsync();
        AvatarUrl = "default_avatar.png";
        AvatarPreview = null;
        _selectedImage = null;
        await _toastService.ShowSuccessAsync("Profilbild entfernt");
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        IsBusy = true;

        try
        {
            // Avatar hochladen wenn geaendert
            if (_selectedImage != null)
            {
                using var stream = await _selectedImage.OpenReadAsync();
                var result = await _userService.UploadAvatarAsync(stream, _selectedImage.FileName);
                AvatarUrl = result.AvatarUrl;
            }

            // Profildaten speichern
            await _userService.UpdateProfileAsync(new UpdateProfileRequest
            {
                FirstName = FirstName,
                LastName = LastName
            });

            await _toastService.ShowSuccessAsync("Profil gespeichert");
            await Shell.Current.GoToAsync("..");
        }
        catch (ValidationException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Vorname aendern | Wird gespeichert |
| TC-002 | Nachname aendern | Wird gespeichert |
| TC-003 | Foto aus Galerie | Vorschau wird angezeigt |
| TC-004 | Foto aufnehmen | Kamera oeffnet sich |
| TC-005 | Avatar entfernen | Standardbild wird gesetzt |
| TC-006 | Leerer Vorname | Validierungsfehler |
| TC-007 | Name > 50 Zeichen | Validierungsfehler |
| TC-008 | Bild > 5 MB | Fehlermeldung |

## Story Points

2

## Prioritaet

Mittel
