using Godot;
using System;

public partial class main_menu : Control
{
    [Export] private credits creditsMenu;
    [Export] private options optionsMenu;
    private AudioStreamPlayer2D buttonSound;
    private AnimationPlayer transition;
    private string nextScenePath = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        creditsMenu = GetNode<credits>("credits");
        optionsMenu = GetNode<options>("options");
	    var newgButton = GetNode<Button>("new_game_btn");
	    var loadgButton = GetNode<Button>("load_game_btn");
        var optionsButton = GetNode<Button>("options_btn");
        var creditsButton = GetNode<Button>("credits_btn");
        var quitButton = GetNode<Button>("quit_game_btn");


        buttonSound = GetNode<AudioStreamPlayer2D>("button_sound");
        transition = GetNode<AnimationPlayer>("transition");

        newgButton.Connect("pressed", new Callable(this, nameof(_on_new_game_btn_pressed)));
	    loadgButton.Connect("pressed", new Callable(this, nameof(_on_load_game_btn_pressed)));
        optionsButton.Connect("pressed", new Callable(this, nameof(_on_options_btn_pressed)));
        creditsButton.Connect("pressed", new Callable(this, nameof(_on_credits_btn_pressed)));
        quitButton.Connect("pressed", new Callable(this, nameof(_on_quit_game_btn_pressed)));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            buttonSound.Stop();
            buttonSound.Play();
        }
    }


	 private void _on_new_game_btn_pressed()
    {
        // GD.Print("Botão de New Game foi pressionado");
        // PlayButtonSound();
        // GetTree().ChangeSceneToFile("res://scenes/terrain_manager.tscn");
        GD.Print("Botão de New Game foi pressionado");
        PlayButtonSound();

        // Obtém a instância de SaveSystem
        var saveSystem = GetNode<SaveSystem>("save_system");

        // Reseta o progresso
        saveSystem.ResetSave();

        // Carrega a cena do jogo
        nextScenePath = "res://scenes/front_school.tscn";
        transition.Play("transition");
    }

	private void _on_load_game_btn_pressed()
    {
        // PlayButtonSound();
        // GD.Print("Botão de Load Game foi pressionado");
        GD.Print("Botão de Load Game foi pressionado");
        PlayButtonSound();

        // Carrega a cena do jogo
        nextScenePath = "res://scenes/front_school.tscn";
        transition.Play("transition");
    }

    private void _on_options_btn_pressed()
    {
        PlayButtonSound();
        GD.Print("Botão de Opções foi pressionado");
        optionsMenu.Visible = true;
    }

    private void _on_credits_btn_pressed()
    {
        PlayButtonSound();
		 GD.Print("Botão dos Créditos foi pressionado");
         creditsMenu.Visible = true;

    }

    private void _on_quit_game_btn_pressed()
    {
        GetTree().Quit();
    }

    private void OnTransitionFinished(string animName)
    {
        if (!string.IsNullOrEmpty(nextScenePath))
        {
            GetTree().ChangeSceneToFile(nextScenePath);
        }
    }
}
