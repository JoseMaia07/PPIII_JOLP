using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export] private options optionsMenu;
    private AudioStreamPlayer2D buttonSound;


    public override void _Ready()
    {
        optionsMenu = GetNode<options>("options");
        var returnGameButton = GetNode<Button>("return_game_btn");
        var playerLight = GetNode<PointLight2D>("PointLight2D");
        var optionspmButton = GetNode<Button>("options__pm_btn");
        var mainMenuButton = GetNode<Button>("main_menu_btn");
        var quitpmButton = GetNode<Button>("quit_game_pm_btn");

        buttonSound = GetNode<AudioStreamPlayer2D>("button_sound");

        returnGameButton.Connect("pressed", new Callable(this, nameof(_on_return_game_btn_pressed)));
        optionspmButton.Connect("pressed", new Callable(this, nameof(_on_options_pm_btn_pressed)));
        mainMenuButton.Connect("pressed", new Callable(this, nameof(_on_main_menu_btn_pressed)));
        quitpmButton.Connect("pressed", new Callable(this, nameof(_on_quit_game_pm_btn_pressed)));

    }

    public override void _UnhandledInput(InputEvent @event)
    {
    if (@event.IsActionPressed("ui_cancel"))
        {
            if (Visible)
            {
                PlayButtonSound();
                Visible = false;
                GetTree().Paused = false;
            }
            else
            {
                PlayButtonSound();
                Visible = true;
                GetTree().Paused = true;
            }
        }
    }

    private void _on_return_game_btn_pressed()
    {
        PlayButtonSound();
        this.Visible = false;
        GetTree().Paused = false;
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            buttonSound.Stop();
            buttonSound.Play();
        }
    }

    private void _on_options_pm_btn_pressed()
    {
        PlayButtonSound();
        GD.Print("Botão de Opções foi pressionado");
        optionsMenu.Visible = true;
    }

    private void _on_main_menu_btn_pressed()
    {
        PlayButtonSound();
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
    }

    private void _on_quit_game_pm_btn_pressed()
    {
        GetTree().Quit();
    }
}
