using Godot;
using System;

public partial class Angelo : Node2D
{
	// Called when the node enters the scene tree for the first time.
	private Character angelo;
	public override void _Ready()
	{
		// Cria uma instância do personagem
        angelo = new Character("Angelo", 200, 50, 15, 30, 50, 2);

        // Configura textura e animações
        var texture = GD.Load<Texture>("res://assets/characters/secondarycharas_sprites/Angelo/Magus_Ref4-removebg-preview.png");
        var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        angelo.Initialize(
            name: "Angelo",
            maxHP: 200,
            maxMP: 50,
            bas_attack: 15,
            spattack1: 30,
            spattack2: 50,
            defense: 2,
            texture: texture,
            animations: animationPlayer,
            attackAnim: "attack",
            spAttack1Anim: "claws",
            spAttack2Anim: "evade",
            defeatAnim: "defeat"
        );

        AddChild(angelo);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
