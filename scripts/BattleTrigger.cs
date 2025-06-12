using Godot;
using System;

public partial class BattleTrigger : Area2D
{
	[Export]
    public PackedScene BattleScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_body_entered(Node body)
    {
         if (body is player)
        {
            GD.Print("O Player entrou na área de batalha.");

            if (BattleScene != null)
            {
                // Carrega e troca para a cena de batalha
                GetTree().ChangeSceneToPacked(BattleScene);
            }
            else
            {
                GD.PrintErr("BattleScene não está configurada no Inspector.");
            }

            // Opcional: Remova o gatilho após a troca de cena, se necessário
            QueueFree();
        }
    }
}
