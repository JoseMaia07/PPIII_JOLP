using Godot;
using System.IO;
using System.Text.Json;

public partial class SaveSystem : Node2D
{
    private string savePath;

    public override void _Ready()
    {
        savePath = Path.Combine(OS.GetUserDataDir(), "savegame.json");
    }

    public void SaveGame(Vector2 playerPosition)
    {
        var saveData = new 
        {
            PositionX = playerPosition.X,
            PositionY = playerPosition.Y
        };

        string jsonData = JsonSerializer.Serialize(saveData);
        File.WriteAllText(savePath, jsonData);
    }

    public bool LoadGame(out Vector2 playerPosition)
    {
        if (!File.Exists(savePath))
        {
            playerPosition = Vector2.Zero;
            return false;
        }

        string jsonData = File.ReadAllText(savePath);
        var saveData = JsonSerializer.Deserialize<JsonElement>(jsonData);

        playerPosition = new Vector2(
            saveData.GetProperty("PositionX").GetSingle(),
            saveData.GetProperty("PositionY").GetSingle()
        );

        return true;
    }

    public void ResetSave()
    {
        // Define a posição inicial para o novo jogo
        var saveData = new
        {
            PositionX = 518f,
            PositionY = 397f
        };

        string jsonData = JsonSerializer.Serialize(saveData);
        File.WriteAllText(savePath, jsonData);
    }
}
