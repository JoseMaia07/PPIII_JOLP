using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BattleScene1 : Node2D
{
    private class Character
    {
        public string Name;
        public int Health;
        public int MaxHealth;

		public int Mana;
		public int MaxMana;
        public int AttackPower;
        public int Defense;

        public Character(string name, int maxHealth, int maxMana, int attackPower, int defense)
        {
            Name = name;
            MaxHealth = maxHealth;
			MaxMana = maxMana;
            Health = maxHealth;
			Mana = maxMana;
            AttackPower = attackPower;
            Defense = defense;
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

		 public bool HasEnoughMana(int cost)
        {
            return Mana >= cost;
        }

        public void ConsumeMana(int cost)
        {
            Mana -= cost;
            Mana = Math.Max(Mana, 0);
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

	private Button skills_btn;

	private TextureRect bg_skills_menu;

    private VBoxContainer skills_menu;
    private Button skill1_btn;
    private Button skill2_btn;

    private Button back_skills_btn;
    private AudioStreamPlayer2D buttonSound;


    private TextureProgressBar playerHealthBar;  // Barra de vida do jogador
	private TextureProgressBar playerManaBar;
    private TextureProgressBar enemyHealthBar;   // Barra de vida do inimigo
    private TextureProgressBar enemyManaBar;

    private RichTextLabel playerHpLabel; // Texto do HP do jogador
	private RichTextLabel playerMpLabel;
    private RichTextLabel enemyHpLabel;  // Texto do HP do inimigo
    private RichTextLabel enemyMpLabel;

    private bool isPlayerDefending;
    private bool isEnemyDefending;

    // Controle do Log de Combate
    private Queue<string> messageQueue = new Queue<string>(); // Fila de mensagens
    private bool isDisplayingMessages = false;
    private float messageDelay = 1.5f; // Delay de 1.5 segundos entre mensagens

	private int skill1ManaCost = 40;
    private int skill2ManaCost = 20;

    public override void _Ready()
    {
        GD.Print("Cena de batalha carregada com sucesso.");
        player = new Character("Player", 100, 100, 15, 10);
        enemy = new Character("Diabrete", 120, 60, 20, 4);

        // Inicializa a UI
        combatLog = GetNode<RichTextLabel>("menu/combat_log");
        attack_btn = GetNode<Button>("menu/combat_menu/HBoxContainer3/attack_btn");
        defense_btn = GetNode<Button>("menu/combat_menu/HBoxContainer3/defense_btn");
        playerHealthBar = GetNode<TextureProgressBar>("menu/player_hpbar");
		playerManaBar = GetNode<TextureProgressBar>("menu/player_mpbar");
        enemyHealthBar = GetNode<TextureProgressBar>("menu/enemy_hpbar");
        enemyManaBar = GetNode<TextureProgressBar>("menu/enemy_mpbar");
        buttonSound = GetNode<AudioStreamPlayer2D>("button_sound");

		skills_btn = GetNode<Button>("menu/combat_menu/HBoxContainer2/skills_btn");
		bg_skills_menu = GetNode<TextureRect>("menu/bg_skills_menu");
        skills_menu = GetNode<VBoxContainer>("menu/skills_menu");
        skill1_btn = GetNode<Button>("menu/skills_menu/VBoxContainer/skill1_btn");
        skill2_btn = GetNode<Button>("menu/skills_menu/VBoxContainer/skill2_btn");
        back_skills_btn = GetNode<Button>("menu/back_skills_btn");

        // Inicializa os Labels do HP
        playerHpLabel = GetNode<RichTextLabel>("menu/hp_player");
		playerMpLabel = GetNode<RichTextLabel>("menu/mp_player");
        enemyHpLabel = GetNode<RichTextLabel>("menu/hp_enemy");
        enemyMpLabel = GetNode<RichTextLabel>("menu/mp_enemy");

        combatLog.QueueRedraw();  // Força a atualização visual do Label

        // Conecta os botões
        attack_btn.Pressed += OnAttackButtonPressed;
        defense_btn.Pressed += OnDefenseButtonPressed;
		skills_btn.Pressed += OnSkillsButtonPressed;
        skill1_btn.Pressed += OnSkill1ButtonPressed;
        skill2_btn.Pressed += OnSkill2ButtonPressed;
        back_skills_btn.Pressed += OnBackSkillButtonPressed;

		bg_skills_menu.Visible = false;
		skills_menu.Visible = false;
        back_skills_btn.Visible = false;
        // Começa o combate
        UpdateHealthBars(); // Atualiza as barras de saúde e os labels
		UpdateManaBars();
        StartTurn();
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            buttonSound.Stop();
            buttonSound.Play();
        }
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
        PlayButtonSound();
        if (!isPlayerTurn) return;

        Attack(player, enemy, isEnemyDefending);
        isPlayerTurn = false;

        attack_btn.Disabled = true;
        defense_btn.Disabled = true;
        skills_btn.Disabled = true;

        if (enemy.IsAlive())
        {
            GetTree().CreateTimer(0.5f).Timeout += () => StartTurn();
        }
    }

    private void OnDefenseButtonPressed()
    {
        PlayButtonSound();
        if (!isPlayerTurn) return;

        isPlayerDefending = true;
        UpdateCombatLog($"{player.Name} assume posição defensiva! Dano recebido será reduzido.");
        isPlayerTurn = false;

        attack_btn.Disabled = true;
        defense_btn.Disabled = true;
        skills_btn.Disabled = true;

        if (enemy.IsAlive())
        {
            GetTree().CreateTimer(0.5f).Timeout += () => StartTurn();
        }
    }

	private void OnSkillsButtonPressed()
    {
        PlayButtonSound();
		bg_skills_menu.Visible = true;
        skills_menu.Visible = true; // Alterna a visibilidade do menu de habilidades
        back_skills_btn.Visible = true;
    }

    private void OnBackSkillButtonPressed()
    {
        PlayButtonSound();
        bg_skills_menu.Visible = false;
		skills_menu.Visible = false;
        back_skills_btn.Visible = false;
    }

	 private void OnSkill1ButtonPressed()
    {
        PlayButtonSound();
		if (!isPlayerTurn || !player.HasEnoughMana(skill1ManaCost)) return;

        player.ConsumeMana(skill1ManaCost);
        UpdateManaBars();
        UpdateSkillButtons();

        int specialDamage = 30;
        enemy.Health -= specialDamage;
        UpdateCombatLog($"{player.Name} usou Soco Carregado causando {specialDamage} de dano!");
        UpdateHealthBars();
        skills_menu.Visible = false;
        bg_skills_menu.Visible = false;
        back_skills_btn.Visible = false;

        isPlayerTurn = false;
        GetTree().CreateTimer(0.5f).Timeout += () => StartTurn();
    }

    private void OnSkill2ButtonPressed()
    {
        PlayButtonSound();
        if (!isPlayerTurn || !player.HasEnoughMana(skill2ManaCost)) return;

        player.ConsumeMana(skill2ManaCost);
        UpdateManaBars();
        UpdateSkillButtons();

        int specialDamage = 1;
        enemy.Health -= specialDamage;
        UpdateCombatLog($"{player.Name} usou Caneta! causando {specialDamage} de dano!");
        UpdateCombatLog($"Caneta? Que ideia foi essa?");
        UpdateHealthBars();
        skills_menu.Visible = false;
        bg_skills_menu.Visible = false;
        back_skills_btn.Visible = false;

        isPlayerTurn = false;
        GetTree().CreateTimer(0.5f).Timeout += () => StartTurn();
    }


    private void EnemyTurn()
    {
        if (!enemy.IsAlive()) return;

        isEnemyDefending = GD.Randi() % 4 == 0;

        bool canUseSpecial = enemy.HasEnoughMana(60) && GD.Randi() % 4 == 0;
        if (canUseSpecial)
        {
            int specialDamage = 25; // Defina o dano do ataque especial
            enemy.ConsumeMana(60); // Consome 60 de mana
            UpdateManaBars(); // Atualiza a barra de mana do Diabrete

            player.Health -= specialDamage;
            player.Health = Math.Max(player.Health, 0);

            UpdateCombatLog($"{enemy.Name} usou uma bola de fogo causando {specialDamage} de dano!");
            UpdateHealthBars();
        }

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
            damageReduction += 1;
        }

        int finalDamage = Math.Max(baseDamage - damageReduction, 1);
        target.Health -= finalDamage;
        target.Health = Math.Max(target.Health, 0);

        UpdateHealthBars();

        UpdateCombatLog($"{attacker.Name} atacou {target.Name} causando {finalDamage} de dano!");

        if (!target.IsAlive())
        {
            UpdateCombatLog($"{target.Name} foi derrotado!");
            EndCombat();
        }
    }

    private void UpdateCombatLog(string message)
    {
        GD.Print("Adicionando mensagem ao log de combate: " + message);
        messageQueue.Enqueue(message);

        if (!isDisplayingMessages)
        {
            DisplayNextMessage();
        }
    }

    private async void DisplayNextMessage()
    {
        isDisplayingMessages = true;

        while (messageQueue.Count > 0)
        {
            string nextMessage = messageQueue.Dequeue();
            combatLog.Text += "\n" + nextMessage;
            combatLog.QueueRedraw();

            await ToSignal(GetTree().CreateTimer(messageDelay), "timeout");
        }

        isDisplayingMessages = false;

    }

    private void UpdateHealthBars()
    {
        // Atualiza as barras de vida
        playerHealthBar.Value = player.Health;
        playerHealthBar.MaxValue = player.MaxHealth;

        enemyHealthBar.Value = enemy.Health;
        enemyHealthBar.MaxValue = enemy.MaxHealth;

        // Atualiza os Labels de HP
        playerHpLabel.Text = $"{player.Health} / {player.MaxHealth}";
        enemyHpLabel.Text = $"{enemy.Health} / {enemy.MaxHealth}";
    }
	private void UpdateManaBars()
    {
        playerManaBar.Value = player.Mana;
        playerManaBar.MaxValue = player.MaxMana;
        playerMpLabel.Text = $"{player.Mana} / {player.MaxMana}";
        enemyManaBar.Value = enemy.Mana;
        enemyManaBar.MaxValue = enemy.MaxMana;
        enemyMpLabel.Text = $"{enemy.Mana} / {enemy.MaxMana}";
    }

	private void UpdateSkillButtons()
    {
        skill1_btn.Disabled = !player.HasEnoughMana(skill1ManaCost);
        skill2_btn.Disabled = !player.HasEnoughMana(skill2ManaCost);
    }

    private void EnablePlayerActions(bool enable)
    {
        attack_btn.Disabled = !enable;
        defense_btn.Disabled = !enable;
        skills_btn.Disabled = !enable;
    }

    private async Task EndCombat()
    {
        if (!player.IsAlive())
        {
            UpdateCombatLog("Você foi derrotado! Fim de jogo.");
            await ToSignal(GetTree().CreateTimer(5.0f), "timeout");
            GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
        }
        else if (!enemy.IsAlive())
        {
            UpdateCombatLog("Você venceu a batalha!");
            await ToSignal(GetTree().CreateTimer(5.0f), "timeout");
            GetTree().ChangeSceneToFile("res://scenes/front_school.tscn");
        }

        EnablePlayerActions(false);
    }


    public override void _Process(double delta)
    {
    }
}
