using Godot;
using System;

public partial class BattleScene : Node2D
{
    private class Character
    {
        public string Name;
        public int Health;
        public int MaxHealth;
        public int AttackPower;
        public int Defense;

        public Character(string name, int maxHealth, int attackPower, int defense)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            AttackPower = attackPower;
            Defense = defense;
        }

        public bool IsAlive()
        {
            return Health > 0;
        }
    }

    // Player e inimigo
    private Character player;
    private Character enemy;

    // Controle de turnos
    private bool isPlayerTurn = true;

    // UI Nodes
    private RichTextLabel combatLog;
    private Button attack_btn;
    private Button defense_btn;

	private TextureProgressBar playerHealthBar;  // Barra de vida do jogador
    private TextureProgressBar enemyHealthBar;   // Barra de vida do inimigo

    private bool isPlayerDefending;
    private bool isEnemyDefending;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Cena de batalha carregada com sucesso.");
        player = new Character("Player", 100, 20, 10);
        enemy = new Character("Diabrete", 80, 15, 8);

        // Inicializa a UI
        combatLog = GetNode<RichTextLabel>("Control/HBoxContainer/TextureRect/combatlog");
        attack_btn = GetNode<Button>("Control/attack_btn");
        defense_btn = GetNode<Button>("Control/defense_btn");
		playerHealthBar = GetNode<TextureProgressBar>("Control/PlayerHPBAR");
        enemyHealthBar = GetNode<TextureProgressBar>("Control/EnemyContainer/EnemyHPBAR");

        combatLog.Text = "Teste de visibilidade do Combat Log!";
        combatLog.QueueRedraw();  // Força a atualização visual do Label

        // Conecta os botões
        attack_btn.Pressed += OnAttackButtonPressed;
        defense_btn.Pressed += OnDefenseButtonPressed;

        // Começa o combate
        UpdateCombatLog($"A batalha começou! {player.Name} vs {enemy.Name}");
		UpdateHealthBars(); // Atualiza as barras de saúde
        StartTurn();
    }

    private void StartTurn()
    {
        // Verifique se o jogador ou inimigo estão mortos antes de processar o turno
        if (player.IsAlive() && enemy.IsAlive())
        {
            if (isPlayerTurn)
            {
                UpdateCombatLog("Seu turno! Escolha uma ação.");
                EnablePlayerActions(true);
            }
            else
            {
                UpdateCombatLog("Turno do inimigo...");
                EnablePlayerActions(false);
                EnemyTurn();
            }
        }
        else
        {
            // Se alguém morreu, terminar o combate
            EndCombat();
        }
    }

    private void OnAttackButtonPressed()
    {
        if (!isPlayerTurn) return; // Garante que o jogador só pode atacar no próprio turno

        Attack(player, enemy, isEnemyDefending);
        isPlayerTurn = false;

        attack_btn.Disabled = true; // Evita múltiplos cliques antes do turno terminar
        defense_btn.Disabled = true;

        if (enemy.IsAlive())
        {
            GetTree().CreateTimer(0.5f).Timeout += () => StartTurn(); // Pequeno delay para evitar execução dupla
        }
    }

    private void OnDefenseButtonPressed()
    {
        if (!isPlayerTurn) return;

        isPlayerDefending = true;
        UpdateCombatLog($"{player.Name} assume posição defensiva! Dano recebido será reduzido.");
        isPlayerTurn = false;

        attack_btn.Disabled = true;
        defense_btn.Disabled = true;

        if (enemy.IsAlive())
        {
            GetTree().CreateTimer(0.5f).Timeout += () => StartTurn();
        }
    }
    private void EnemyTurn()
    {
        if (!enemy.IsAlive()) return; // Impede que o inimigo execute ações se já estiver morto

        isEnemyDefending = GD.Randi() % 2 == 0; // Inimigo tem 50% de chance de defender

        if (isEnemyDefending)
        {
            UpdateCombatLog($"{enemy.Name} assume posição defensiva! Dano recebido será reduzido.");
        }
        else
        {
            Attack(enemy, player, isPlayerDefending);
        }

        isPlayerTurn = true;
        StartTurn();
    }

    private void Attack(Character attacker, Character target, bool isTargetDefending)
    {
        int baseDamage = attacker.AttackPower;

        // Aplica a defesa do alvo
        int damageReduction = target.Defense;
        if (isTargetDefending)
        {
            damageReduction += 5; // Bônus extra de defesa se o alvo estiver defendendo
        }

        int finalDamage = Mathf.Max(baseDamage - damageReduction, 1); // Garantir que o dano não seja menor que 1
        target.Health -= finalDamage;
        target.Health = Mathf.Max(target.Health, 0); // Evitar valores negativos

		UpdateHealthBars();

        UpdateCombatLog($"{attacker.Name} atacou {target.Name} causando {finalDamage} de dano!");

        if (!target.IsAlive())
        {
            UpdateCombatLog($"{target.Name} foi derrotado!");
            GetTree().ChangeSceneToFile("res://scenes/front_school.tscn");
        }
    }

    private void Heal(Character character)
    {
        int healAmount = 20;
        character.Health += healAmount;
        character.Health = Mathf.Min(character.Health, character.MaxHealth);

        UpdateCombatLog($"{character.Name} usou cura e recuperou {healAmount} HP!");
    }

    private void UpdateCombatLog(string message)
    {
        GD.Print("Atualizando log de combate: " + message);
        combatLog.Text += "\n" + message;
        combatLog.QueueRedraw();
    }

	private void UpdateHealthBars()
    {
        // Atualiza as barras de vida
        playerHealthBar.Value = player.Health;
        playerHealthBar.MaxValue = player.MaxHealth;

        enemyHealthBar.Value = enemy.Health;
        enemyHealthBar.MaxValue = enemy.MaxHealth;
    }

    private void EnablePlayerActions(bool enable)
    {
        attack_btn.Disabled = !enable;
        defense_btn.Disabled = !enable;
    }

    private void EndCombat()
    {
        if (!player.IsAlive())
        {
            UpdateCombatLog("Você foi derrotado! Fim de jogo.");
        }
        else if (!enemy.IsAlive())
        {
            UpdateCombatLog("Você venceu a batalha!");
        }

        EnablePlayerActions(false);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
