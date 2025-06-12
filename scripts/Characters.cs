using Godot;
using System;

public partial class Character : Node
{
    // Propriedades do personagem
    public string Name;
    public int MaxHP;
    public int CurrentHP;
    public int MaxMP;
    public int CurrentMP;
    public int BasAttack;
    public int Defense;
    public int SPAttack1;
    public int SPAttack2;

    // Componentes visuais
    private Sprite2D sprite;
    private AnimationPlayer animationPlayer;

    // Nomes das animações
    private string attackAnimation;
    private string spAttack1Animation;
    private string spAttack2Animation;
    private string defeatAnimation;

    public Character(string name, int maxHP, int maxMP, int bas_attack, int spattack1, int spattack2, int defense)
    {
        Name = name;
        MaxHP = maxHP;
        CurrentHP = maxHP;
        MaxMP = maxMP;
        CurrentMP = maxMP;
        BasAttack = bas_attack;
        SPAttack1 = spattack1;
        SPAttack2 = spattack2;
        Defense = defense;

        // Inicializa os nodos visuais
        sprite = new Sprite2D();
        AddChild(sprite);

        animationPlayer = new AnimationPlayer();
        AddChild(animationPlayer);

        // Animações padrão
        attackAnimation = "attack";
        spAttack1Animation = "spattack1";
        spAttack2Animation = "spattack2";
        defeatAnimation = "defeat";
    }

    public void Initialize(string name, int maxHP, int maxMP, int bas_attack, int spattack1, int spattack2, int defense, Texture texture, AnimationPlayer animations,
                            string attackAnim = "attack", string spAttack1Anim = "spattack1", string spAttack2Anim = "spattack2", string defeatAnim = "defeat")
    {
        Name = name;
        MaxHP = maxHP;
        CurrentHP = maxHP;
        MaxMP = maxMP;
        CurrentMP = maxMP;
        BasAttack = bas_attack;
        SPAttack1 = spattack1;
        SPAttack2 = spattack2;
        Defense = defense;

        // Configurar a textura
        SetTexture(texture);

        // Configurar animações (se houver)
        SetAnimationPlayer(animations);

        // Configurar os nomes das animações
        attackAnimation = attackAnim;
        spAttack1Animation = spAttack1Anim;
        spAttack2Animation = spAttack2Anim;
        defeatAnimation = defeatAnim;
    }

    // Define a textura do personagem
    public void SetTexture(Texture texture)
    {
        if (texture != null)
        {
            sprite.Texture = (Texture2D)texture;
        }
    }

    // Define o AnimationPlayer do personagem
    public void SetAnimationPlayer(AnimationPlayer animations)
    {
        if (animations != null)
        {
            animationPlayer = animations;
        }
    }

    // Métodos de dano e verificação de estado
    public int TakeDamage(int damage)
    {
        int finalDamage = Math.Max(damage - Defense, 0);
        CurrentHP = Math.Max(CurrentHP - finalDamage, 0);

        // Tocar animação de dano (se configurada)
        PlayAnimation("TakeDamage");

        return finalDamage;
    }

    public bool IsAlive()
    {
        return CurrentHP > 0;
    }

    // Método para tocar animações
    public void PlayAnimation(string animationName)
    {
        if (animationPlayer != null && animationPlayer.HasAnimation(animationName))
        {
            animationPlayer.Play(animationName);
        }
    }

    // Tocar animações específicas
    public void PlayAttackAnimation()
    {
        PlayAnimation(attackAnimation);
    }

    public void PlaySpAttack1Animation()
    {
        PlayAnimation(spAttack1Animation);
    }

    public void PlaySpAttack2Animation()
    {
        PlayAnimation(spAttack2Animation);
    }

    public void PlayDefeatAnimation()
    {
        PlayAnimation(defeatAnimation);
    }
}
