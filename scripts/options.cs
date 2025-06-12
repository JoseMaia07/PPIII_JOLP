using Godot;
using System;

public partial class options : Control
{
    private AudioStreamPlayer2D buttonSound;

    public override void _Ready()
    {
        // Obtendo os nós
        var quitopButton = GetNode<Button>("quit_options_btn");
        buttonSound = GetNode<AudioStreamPlayer2D>("button_sound");

        // Adicionar os modos ao OptionButton

        // Conectar botão de sair
        quitopButton.Connect("pressed", new Callable(this, nameof(_on_quit_options_btn_pressed)));

        GD.Print("Script carregado corretamente!");
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            buttonSound.Stop();
            buttonSound.Play();
        }
    }

    private void _on_quit_options_btn_pressed()
    {
        PlayButtonSound();
        this.Visible = false;
    }
}
