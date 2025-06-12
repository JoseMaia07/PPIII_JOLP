using Godot;
using System;

public partial class WindowModeButton : OptionButton
{
	private OptionButton windowModeButton;
	private AudioStreamPlayer2D buttonSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		windowModeButton = GetNode<OptionButton>("window_mode_button");
		buttonSound = GetNode<AudioStreamPlayer2D>("button_sound");

		windowModeButton.Connect("item_selected", new Callable(this, nameof(_on_window_mode_button_item_selected)));

		AddWindowModeItems();
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

	 private void AddWindowModeItems()
    {
        windowModeButton.Clear();
        windowModeButton.AddItem("Tela Cheia", 0);
        windowModeButton.AddItem("Tela Cheia sem Bordas", 1);
        windowModeButton.AddItem("Janela", 2);

        // Definir o estado inicial correto do botão
        DisplayServer.WindowMode currentMode = DisplayServer.WindowGetMode();

        if (currentMode == DisplayServer.WindowMode.Fullscreen)
            windowModeButton.Selected = 0;
        else if (currentMode == DisplayServer.WindowMode.ExclusiveFullscreen)
            windowModeButton.Selected = 1;
        else
            windowModeButton.Selected = 2;

        GD.Print("Modo de tela atual: " + currentMode);
    }

	private void _on_window_mode_button_item_selected(int index)
    {
        PlayButtonSound();
        GD.Print("Mudando para modo de tela: " + index);

        switch (index)
        {
            case 0:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                GD.Print("Modo alterado para Tela Cheia");
                break;

            case 1:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                GD.Print("Modo alterado para Tela Cheia sem Bordas");
                break;

            case 2:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                GD.Print("Modo alterado para Janela");
                break;
        }
    }
}
