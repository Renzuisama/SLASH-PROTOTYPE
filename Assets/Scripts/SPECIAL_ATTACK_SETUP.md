# Holy Cross Special Attack Setup Guide

This guide explains how to set up the Holy Cross special AOE attack that damages all enemies in an area.

## Features

- **E button activation**: Press E to activate special attack
- **AOE damage**: Damages all enemies within radius (default: 5 units)
- **Auto-targeting**: Automatically targets closest enemy and spawns there
- **Stamina cost**: Uses full stamina bar (100 stamina)
- **Cooldown system**: 10 second cooldown between uses
- **Holy Cross VFX**: Beautiful yellow cross effect spawns on target
- **Visual feedback**: Yellow damage numbers for special attack

## Quick Setup (5 Steps)

### Step 1: Add SpecialAttack Component to Player

1. Select your **Player** GameObject in Hierarchy
2. Click **Add Component**
3. Search for **"SpecialAttack"**
4. Click to add it

### Step 2: Assign Holy Cross Prefab

1. In **SpecialAttack** component, find **"Holy Cross Prefab"** field
2. Navigate to: `Assets/Pixel Art/PixelArtRPGVFXLite/Prefabs/Holy/`
3. **Drag "HolyCross" prefab** into the field

### Step 3: Configure Attack Settings

In **SpecialAttack** component:

**Holy Cross Special Attack:**
- **Holy Cross Prefab**: HolyCross (assigned in Step 2)
- **Special Damage**: 50 (damage to each enemy hit)
- **AOE Radius**: 5 (damage radius in units)
- **Attack Duration**: 2 (how long VFX stays)

**Stamina/Cooldown:**
- **Special Stamina Cost**: 100 (full stamina bar)
- **Cooldown Time**: 10 (seconds between uses)
- **Use Stamina**: ✓ (checked)
- **Use Cooldown**: ✓ (checked)

**Targeting:**
- **Enemy Layer**: Select your enemy layer (e.g., "Enemy")
- **Detection Range**: 15 (how far to search for enemies)
- **Target Closest Enemy**: ✓ (checked)

### Step 4: Verify Enemy Layer Setup

Make sure your enemies are on the correct layer:

1. Select an enemy prefab or instance
2. Check the **Layer** dropdown at the top of Inspector
3. Should be set to "Enemy" (or whatever layer you're using)
4. Set **Enemy Layer** in SpecialAttack to match this

### Step 5: Test!

1. **Play the game**
2. **Get near enemies**
3. **Make sure stamina is full** (wait for it to regenerate)
4. **Press E** → Holy Cross spawns on closest enemy!
5. **Watch enemies take damage** within the AOE radius
6. **Try pressing E again** → Should fail (cooldown)
7. **Wait 10 seconds** → Can use special attack again

**Success!** You now have a powerful AOE special attack.

---

## How It Works

### Activation Sequence

1. **Press E** → Check if special is ready
2. **Check stamina** → Need 100 stamina (full bar)
3. **Check cooldown** → Must wait 10 seconds between uses
4. **Find target** → Automatically finds closest enemy
5. **Spawn VFX** → Holy Cross appears at enemy location
6. **Deal damage** → All enemies in 5 unit radius take damage
7. **Start cooldown** → Can't use again for 10 seconds

### Stamina System Integration

- **Cost**: 100 stamina (exactly 2 dashes worth)
- **Requires full bar**: Must have full stamina to use
- **Regeneration delay**: After using, 1 second delay before regen starts
- **Full recovery**: Takes 5 seconds to regenerate fully

### Targeting System

**Closest Enemy (default):**
- Searches within 15 unit radius
- Finds closest enemy to player
- Spawns Holy Cross at that enemy's position

**Random Enemy (optional):**
- Uncheck "Target Closest Enemy"
- Will pick random enemy in range

---

## Configuration Guide

### Change Damage

**Low damage (chip damage):**
- Special Damage: 25-35

**Medium damage (default):**
- Special Damage: 50

**High damage (mini-nuke):**
- Special Damage: 80-100

**One-shot (boss killer):**
- Special Damage: 200+

### Change AOE Radius

**Small AOE (single target focused):**
- AOE Radius: 2-3

**Medium AOE (default):**
- AOE Radius: 5

**Large AOE (screen clear):**
- AOE Radius: 8-10

**Massive AOE (room clear):**
- AOE Radius: 15+

### Change Stamina Cost

**Cheap (3 uses before empty):**
- Special Stamina Cost: 33

**Moderate (2 uses before empty):**
- Special Stamina Cost: 50

**Expensive (1 use only - default):**
- Special Stamina Cost: 100

**Very expensive (requires extra stamina):**
- Increase Max Stamina to 150
- Special Stamina Cost: 150

### Change Cooldown

**No cooldown (stamina-only):**
- Use Cooldown: ✗ (unchecked)

**Short cooldown:**
- Cooldown Time: 5 seconds

**Medium cooldown (default):**
- Cooldown Time: 10 seconds

**Long cooldown (ultimate ability):**
- Cooldown Time: 30 seconds

**Very long cooldown (once per wave):**
- Cooldown Time: 60 seconds

### Change Detection Range

**Short range (enemies must be close):**
- Detection Range: 8-10

**Medium range (default):**
- Detection Range: 15

**Long range (anywhere on screen):**
- Detection Range: 25-30

**Unlimited range:**
- Detection Range: 999

---

## Configuration Examples

### Default (Powerful Ultimate)
```
Damage: 50
AOE Radius: 5
Stamina Cost: 100 (full bar)
Cooldown: 10 seconds
Detection Range: 15
Result: Strong AOE, used carefully
```

### Frequent Use (Spam Mode)
```
Damage: 30
AOE Radius: 4
Stamina Cost: 50 (2 uses)
Cooldown: 5 seconds
Detection Range: 15
Result: Can use often, lower impact
```

### Boss Killer (Nuke)
```
Damage: 150
AOE Radius: 8
Stamina Cost: 100
Cooldown: 30 seconds
Detection Range: 20
Result: Massive damage, rare use
```

### Screen Clear (Emergency)
```
Damage: 40
AOE Radius: 12
Stamina Cost: 100
Cooldown: 15 seconds
Detection Range: 20
Result: Large area, moderate damage
```

### Balanced (Recommended)
```
Damage: 60
AOE Radius: 6
Stamina Cost: 100
Cooldown: 8 seconds
Detection Range: 15
Result: Good damage, reasonable cooldown
```

---

## Advanced Features

### Add Cooldown UI Indicator

Create a UI element to show cooldown:

```csharp
// In your UI script:
SpecialAttack special = player.GetComponent<SpecialAttack>();

if (special != null)
{
    float cooldownPercent = special.GetCooldownPercentage();
    cooldownImage.fillAmount = cooldownPercent;

    if (special.IsSpecialReady())
    {
        cooldownText.text = "READY!";
    }
    else
    {
        float remaining = special.GetRemainingCooldown();
        cooldownText.text = $"{remaining:F1}s";
    }
}
```

### Add Stamina Requirement Indicator

Show if player has enough stamina:

```csharp
// Change color based on readiness
if (special.IsSpecialReady())
{
    specialButton.color = Color.yellow; // Ready
}
else
{
    specialButton.color = Color.gray; // Not ready
}
```

### Multiple Special Attacks

Create different special attacks for different keys:

1. Duplicate SpecialAttack script
2. Rename to SpecialAttack_Holy, SpecialAttack_Fire, etc.
3. Change the key check in Update():
```csharp
if (Keyboard.current.eKey.wasPressedThisFrame) // Holy
if (Keyboard.current.rKey.wasPressedThisFrame) // Fire
if (Keyboard.current.qKey.wasPressedThisFrame) // Lightning
```

### Knockback on Special Attack

Add knockback to the special attack in DealAOEDamage():

```csharp
// After dealing damage:
Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
if (enemyRb != null)
{
    Vector2 knockbackDir = (enemy.transform.position - center).normalized;
    enemyRb.AddForce(knockbackDir * 10f, ForceMode2D.Impulse);
}
```

### Slow Motion Effect

Add slow-mo when special is used:

```csharp
// In ExecuteSpecialAttack(), after spawning VFX:
StartCoroutine(SlowMotionEffect());

private IEnumerator SlowMotionEffect()
{
    Time.timeScale = 0.3f; // 30% speed
    yield return new WaitForSecondsRealtime(0.5f); // Real-time seconds
    Time.timeScale = 1f; // Back to normal
}
```

### Screen Shake

Add camera shake on impact:

```csharp
// Create CameraShake script, then in ExecuteSpecialAttack():
CameraShake shake = Camera.main.GetComponent<CameraShake>();
if (shake != null)
{
    shake.Shake(0.5f, 0.3f); // Duration, intensity
}
```

---

## Troubleshooting

### Problem: E key does nothing

**Solutions:**
- Check SpecialAttack component is on Player
- Verify Holy Cross Prefab is assigned
- Check Console for error messages
- Make sure stamina is full (100/100)
- Verify cooldown has passed
- Check enemies are in detection range (15 units)

### Problem: Holy Cross spawns but no damage

**Solutions:**
- Verify Enemy Layer is correctly set
- Check enemies have IDamageable component
- Make sure AOE Radius is large enough (try 8)
- Enable Debug Mode and check Console logs
- Verify enemies are within AOE radius of spawn point

### Problem: Can't see Holy Cross VFX

**Solutions:**
- Check Holy Cross Prefab is assigned
- Verify VFX package imported correctly
- Check Z position (should be 0 for 2D)
- Look at Scene view, not just Game view
- Increase Attack Duration to 5 seconds for testing

### Problem: Special uses even with no enemies

**Solutions:**
- This is by design - it checks for enemies first
- If it uses with no enemies, check detection code
- Enable Debug Mode to see what's happening

### Problem: Always says "not enough stamina"

**Solutions:**
- Check Special Stamina Cost (should be 100 or less)
- Verify Max Stamina in PlayerStamina (should be 100+)
- Wait for stamina to fully regenerate
- Check PlayerStamina component is on Player
- Try reducing Special Stamina Cost to 50 for testing

### Problem: Cooldown not working

**Solutions:**
- Check "Use Cooldown" is enabled
- Verify Cooldown Time is > 0
- Enable Debug Mode to see cooldown messages
- Check system time isn't frozen (Time.timeScale = 1)

### Problem: VFX looks wrong or glitchy

**Solutions:**
- Check Particle System settings in prefab
- Verify sorting layers are correct
- Check camera is Orthographic for 2D
- Try reimporting the VFX package

---

## Integration with Other Systems

### With Wave System

Optional: Reset cooldown between waves:

```csharp
// In SimpleWaveManager, between waves:
SpecialAttack special = FindFirstObjectByType<SpecialAttack>();
if (special != null)
{
    // Reset cooldown
    special.ResetCooldown(); // Need to add this method
}
```

### With Upgrade System

Allow players to upgrade special attack:

```csharp
// Upgrade damage
public void UpgradeSpecialDamage(int bonus)
{
    specialDamage += bonus;
}

// Reduce cooldown
public void ReduceCooldown(float reduction)
{
    cooldownTime = Mathf.Max(cooldownTime - reduction, 2f);
}

// Increase AOE
public void IncreaseAOE(float bonus)
{
    aoeRadius += bonus;
}
```

### With Difficulty Scaling

Adjust special attack for difficulty:

```csharp
// Easy mode
specialStaminaCost = 75; // More frequent
cooldownTime = 8;

// Normal mode
specialStaminaCost = 100;
cooldownTime = 10;

// Hard mode
specialStaminaCost = 100;
cooldownTime = 15; // Less frequent
```

---

## Visual Customization

### Change VFX Color

The Holy Cross VFX uses particle systems. To change colors:

1. Find HolyCross prefab in Project
2. Expand hierarchy to find Particle Systems
3. Select each particle system
4. In Particle System component → Color over Lifetime
5. Change gradient colors

### Use Different VFX

Try other effects from the package:

**Lightning** (Electric effect):
- Use: `ElectricLightning01.prefab`
- Good for: Stun-based special attacks

**Explosion** (Fire burst):
- Use: `Explosion_003.prefab`
- Good for: Damage-focused attacks

**Wind** (Ground effect):
- Use: `WindGround.prefab`
- Good for: Knockback attacks

**Void** (Dark shield):
- Use: `VoidShield.prefab`
- Good for: Defensive abilities

---

## Balancing Tips

### Early Game
- Damage: 30-40
- AOE: 4-5
- Cost: 75 stamina
- Cooldown: 8 seconds

### Mid Game
- Damage: 50-60
- AOE: 5-6
- Cost: 100 stamina
- Cooldown: 10 seconds

### Late Game
- Damage: 80-100
- AOE: 7-8
- Cost: 100 stamina
- Cooldown: 8 seconds

### Boss Fights
- Consider reducing cost
- Or reducing cooldown
- Allows more aggressive play

---

## Summary

You've implemented a powerful Holy Cross special attack!

**What you have:**
- ✓ E button activation
- ✓ Auto-targeting system
- ✓ AOE damage (5 unit radius)
- ✓ 50 damage per enemy
- ✓ Full stamina cost (100)
- ✓ 10 second cooldown
- ✓ Beautiful Holy Cross VFX
- ✓ Fully configurable

**Next steps:**
- Adjust damage/AOE to your preference
- Test with different enemy groups
- Consider adding UI indicators
- Balance stamina cost vs cooldown

Enjoy your new special attack system!
