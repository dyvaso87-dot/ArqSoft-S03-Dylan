using System.Text.Json;
using Catalogo.Domain.Models;

namespace Catalogo.Application.Services;

public class UserService
{
    private readonly string _filePath;

    public UserService (string filePath)
    {
        _filePath = filePath;
    }

    private List<User> LoadUsers()
    {
        if (!File.Exists(_filePath)) return new List<User>();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new();
    }

    private void SaveUsers(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public User? GetByEmail(string email) =>
        LoadUsers().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public User? GetByUsername(string username) =>
        LoadUsers().FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public bool ValidateCredentials(string email, string password)
    {
        var user = GetByEmail(email);
        return user != null && user.PasswordHash == password; // En producción usa hash real
    }

    public (bool success, string message) Register(string username, string email, string password)
    {
        var users = LoadUsers();
        if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            return (false, "El correo ya está registrado.");
        if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            return (false, "El nombre de usuario ya existe.");

        users.Add(new User
        {
            Username = username,
            Email = email,
            PasswordHash = password // En producción usa BCrypt
        });
        SaveUsers(users);
        return (true, "Registro exitoso.");
    }
}