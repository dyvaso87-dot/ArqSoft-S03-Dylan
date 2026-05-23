
using System.Text.Json;
using Catalogo.Domain.Models;

namespace Catalogo.Application.Services;

public class ItemService
{
    private readonly string _filePath;

    public ItemService(string filePath)
    {
        _filePath = filePath;
    }

    private List<Item> LoadItems()
    {
        if (!File.Exists(_filePath)) return new();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Item>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }

    private void SaveItems(List<Item> items)
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public List<Item> GetAll() => LoadItems();

    public Item? GetById(string id) =>
        LoadItems().FirstOrDefault(i => i.Id == id);

    public (bool success, string message) AddReview(string itemId, Review review)
    {
        var items = LoadItems();
        var item = items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return (false, "Producto no encontrado.");

        item.Reviews.Add(review);
        SaveItems(items);
        return (true, "Reseña agregada correctamente.");
    }
}