using Godot;
using System;

public partial class OptionsDisplay : Control
{
    private OptionButton windowModeButton;
    private OptionButton resolutionButton;
    private AudioStreamPlayer2D buttonSound;

    private Vector2I[] availableResolutions = new Vector2I[]
    {
        new Vector2I(1200, 800),
        new Vector2I(800, 600),
        new Vector2I(1024, 768),
        new Vector2I(1620, 960)
    };

    public override void _Ready()
    {
        windowModeButton = GetNode<OptionButton>("window_mode_button");
        resolutionButton = GetNode<OptionButton>("resolution_button");
        buttonSound = GetNode<AudioStreamPlayer2D>("button_sound");

        windowModeButton.Connect("item_selected", new Callable(this, nameof(_on_window_mode_button_item_selected)));
        resolutionButton.Connect("item_selected", new Callable(this, nameof(_on_resolution_button_item_selected)));

        AddWindowModeItems();
        AddResolutionItems();

        SetCurrentResolutionSelection();
        UpdateResolutionVisibility();
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
        windowModeButton.AddItem("Janela", 0);
        windowModeButton.AddItem("Tela Cheia", 1);

        DisplayServer.WindowMode currentMode = DisplayServer.WindowGetMode();
        windowModeButton.Selected = (currentMode == DisplayServer.WindowMode.Windowed) ? 0 : 1;
    }

    private void AddResolutionItems()
    {
        resolutionButton.Clear();
        resolutionButton.AddItem("1200x800", 0);
        resolutionButton.AddItem("800x600", 1);
        resolutionButton.AddItem("1024x768", 2);
        resolutionButton.AddItem("1620x960", 3);
    }

    private void SetCurrentResolutionSelection()
    {
        Vector2I currentResolution = DisplayServer.WindowGetSize();
        
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            if (availableResolutions[i] == currentResolution)
            {
                resolutionButton.Selected = i;
                return;
            }
        }

        resolutionButton.Selected = 0;
    }

    private void UpdateResolutionVisibility()
    {
        resolutionButton.Visible = (windowModeButton.Selected == 0); 
    }

    private void _on_window_mode_button_item_selected(int index)
    {
        PlayButtonSound();

        switch (index)
        {
            case 0:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                break;
            case 1:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                break;
        }

        UpdateResolutionVisibility();
    }

    private void _on_resolution_button_item_selected(int index)
    {
        PlayButtonSound();
        if (index >= 0 && index < availableResolutions.Length)
        {
            DisplayServer.WindowSetSize(availableResolutions[index]);
        }
    }
}
