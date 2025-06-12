using Godot;
using System;

public partial class credits : Control
{
	private AudioStreamPlayer2D buttonSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var quitcButton = GetNode<Button>("quit_credits_btn");
		buttonSound = GetNode<AudioStreamPlayer2D>("button_sound");

		quitcButton.Connect("pressed", new Callable(this, nameof(_on_quit_credits_btn_pressed)));
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

	private void _on_quit_credits_btn_pressed()
	{
		PlayButtonSound();
		this.Visible = false;
	}
}
