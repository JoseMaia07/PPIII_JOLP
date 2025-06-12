using Godot;
using System;

public partial class player : CharacterBody2D
{
    public const float Speed = 100.0f;

    private AnimationPlayer animation;
    private Vector2 direction;
    private string lastAnimation = "idle_down"; // Para armazenar a última animação tocada
    private SaveSystem saveSystem;

    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>("Animation");

        // Obtém o SaveSystem na cena
        saveSystem = GetNode<SaveSystem>("save_system");

        // // Tenta carregar a posição salva
        // if (saveSystem.LoadGame(out Vector2 loadedPosition))
        // {
        //     Position = loadedPosition;
        //     GD.Print("Jogo carregado! Posição: " + Position);
        // }
        // else
        // {
        //     Position = new Vector2(100, 100); // Posição inicial padrão
        //     GD.Print("Nenhum save encontrado. Usando posição inicial: " + Position);
        // }

        // Tenta carregar a posição salva
        if (saveSystem.LoadGame(out Vector2 loadedPosition))
        {
            Position = loadedPosition;
            GD.Print("Jogo carregado! Posição: " + Position);
        }
        else
        {
            Position = new Vector2(100, 100); // Posição inicial padrão
            GD.Print("Nenhum save encontrado. Usando posição inicial: " + Position);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Obter a direção do input
        direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (direction != Vector2.Zero)
        {
            // Atualizar a velocidade com base na direção
            velocity = direction * Speed;

            // Determinar animação de andar com base na direção
            string walkAnimation = GetAnimationName(direction, "walk");
            if (!animation.IsPlaying() || animation.CurrentAnimation != walkAnimation)
            {
                animation.Play(walkAnimation);
                lastAnimation = walkAnimation; // Atualizar a última animação
            }
        }
        else
        {
            velocity = Vector2.Zero;

            // Reproduzir a animação de idle com base na última direção
            string idleAnimation = lastAnimation.Replace("walk", "idle");
            if (!animation.IsPlaying() || animation.CurrentAnimation != idleAnimation)
            {
                animation.Play(idleAnimation);
            }
        }

        Velocity = velocity;
        MoveAndSlide();

        // Salva a posição ao apertar ESC
        if (Input.IsActionJustPressed("ui_save_game"))
        {
            saveSystem.SaveGame(Position);
            GD.Print("Jogo salvo! Posição: " + Position);
        }
    }

    private string GetAnimationName(Vector2 dir, string type)
    {
        dir = dir.Normalized();

        // Direções cardinais
        if (dir.X > 0.5f && dir.Y == 0)
            return $"{type}_right_down";
        if (dir.X < -0.5f && dir.Y == 0)
            return $"{type}_left_down";
        if (dir.Y > 0.5f && dir.X == 0)
            return $"{type}_down";
        if (dir.Y < -0.5f && dir.X == 0)
            return $"{type}_up";

        // Direções diagonais
        if (dir.X > 0.5f && dir.Y < -0.5f)
            return $"{type}_right_up";
        if (dir.X < -0.5f && dir.Y < -0.5f)
            return $"{type}_left_up";
        if (dir.X > 0.5f && dir.Y > 0.5f)
            return $"{type}_right_down";
        if (dir.X < -0.5f && dir.Y > 0.5f)
            return $"{type}_left_down";

        // Retornar uma animação padrão caso algo falhe
        return $"{type}_down";
    }
}
