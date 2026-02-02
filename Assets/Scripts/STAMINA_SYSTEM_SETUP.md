# Stamina System Setup Guide

This guide explains how to set up the Dark Souls-style stamina system with a yellow stamina bar that allows exactly 2 dashes.

## Features

- **Dark Souls-style stamina**: Each dash costs 50 stamina (2 dashes maximum with 100 max stamina)
- **Automatic regeneration**: Stamina regenerates after a 1 second delay
- **Yellow stamina bar**: Visual feedback with smooth color gradient
- **Smooth animations**: Bar smoothly depletes and regenerates
- **Configurable**: Adjust stamina costs, max stamina, and regeneration rates
- **Souls-like feel**: Delay before regeneration, fast regen rate once started

## Quick Setup (5 Steps)

### Step 1: Add PlayerStamina to Player

1. Select your **Player** GameObject in Hierarchy
2. Click **Add Component**
3. Search for **"PlayerStamina"**
4. Click to add it

**Default settings are perfect** (100 max stamina, 50 per dash = 2 dashes):
- Max Stamina: 100
- Dash Stamina Cost: 50
- Stamina Regen Rate: 20
- Regen Delay: 1

### Step 2: Create Stamina Bar UI

#### A. Create the Slider

1. Find your **Canvas** in Hierarchy (or create one: Right-click → UI → Canvas)
2. Right-click on **Canvas** → UI → **Slider**
3. Rename it to **"StaminaBar"**

#### B. Position the Slider

1. Select **StaminaBar**
2. In **Rect Transform**:
   - **Anchor**: Bottom-left
   - **Pos X**: 150 (or position below/near your health bar)
   - **Pos Y**: 80
   - **Width**: 200
   - **Height**: 20

#### C. Configure Slider Settings

1. In **Slider** component:
   - **Min Value**: 0
   - **Max Value**: 100
   - **Whole Numbers**: ✗ (unchecked)
   - **Interactable**: ✗ (unchecked)

#### D. Remove the Handle (not needed)

1. Expand **StaminaBar** in Hierarchy
2. Find **"Handle Slide Area"** child object
3. **Delete it**
4. In Slider component, set **Handle Rect** to **None**

#### E. Style the Bar

**Background** (dark area):
1. Select **StaminaBar → Background**
2. In **Image** component, set **Color**: RGB(30, 30, 30) - dark gray/black

**Fill** (yellow bar):
1. Select **StaminaBar → Fill Area → Fill**
2. In **Image** component, set **Color**: RGB(255, 235, 4) - bright yellow
   (The script will handle the gradient automatically)

### Step 3: Add StaminaBar Script

1. Select the **StaminaBar** GameObject (the root slider)
2. Click **Add Component**
3. Search for **"StaminaBar"**
4. Click to add it

**Leave all settings at default**:
- Smooth Transition: ✓
- Transition Speed: 5
- Color Gradient: ✓
- Colors will be auto-configured

### Step 4: Link StaminaBar to PlayerStamina

1. Select your **Player** GameObject
2. Find the **PlayerStamina** component
3. In **References** section, find **"Stamina Bar"** field
4. **Drag the StaminaBar GameObject** from Hierarchy into this field

### Step 5: Test!

1. **Play the game**
2. **Press Shift** → First dash works
3. **Press Shift again** → Second dash works
4. **Press Shift third time** → Should fail (yellow bar is empty)
5. **Wait 1 second** → Yellow bar starts filling
6. **Wait ~5 seconds total** → Bar fully refills

**Success!** You now have a working stamina system.

---

## Detailed Explanation

### How It Works

**Dash Consumption:**
- Each dash costs **50 stamina**
- With **100 max stamina**, you can dash exactly **2 times**
- Third dash attempt is blocked until stamina regenerates

**Regeneration System:**
- **1 second delay** after dashing before regen starts (like Dark Souls)
- **20 stamina per second** regeneration rate
- Takes **5 seconds** to fully regenerate from 0 to 100
- Regenerates while moving (like Dark Souls 3)

**Visual Feedback:**
- **Yellow bar** shows current stamina
- **Color gradient**: Bright yellow (full) → Dark orange (empty)
- **Smooth animations**: Bar smoothly depletes and refills

---

## Configuration Guide

All settings are in the **PlayerStamina** component on your Player GameObject.

### Stamina Settings

**Max Stamina** (default: 100)
- How much stamina the player can have
- Example: 150 max = 3 dashes at 50 cost each

**Dash Stamina Cost** (default: 50)
- How much stamina one dash consumes
- Example: 33 cost = 3 dashes at 100 max stamina

**Attack Stamina Cost** (default: 0)
- Optional stamina cost for attacking
- Set to 0 to disable (no stamina for attacks)
- Set to 15-20 for Dark Souls-style combat

### Regeneration Settings

**Stamina Regen Rate** (default: 20)
- How much stamina regenerates per second
- Higher = faster regeneration
- Example: 30 = regenerates in ~3.3 seconds

**Regen Delay** (default: 1.0)
- Seconds to wait after action before regen starts
- Dark Souls: ~1 second
- Bloodborne: ~0.5 seconds
- Sekiro: ~0.3 seconds

**Can Regen While Moving** (default: ✓)
- If checked, stamina regens while walking
- Dark Souls 3: Yes
- Dark Souls 1: No (only while standing still)

---

## Configuration Examples

### Default (2 Dashes) - Recommended
```
Max Stamina: 100
Dash Cost: 50
Regen Rate: 20/sec
Regen Delay: 1 sec
Result: 2 dashes, 5 second recovery
```

### Aggressive (3 Dashes)
```
Max Stamina: 150
Dash Cost: 50
Regen Rate: 30/sec
Regen Delay: 0.8 sec
Result: 3 dashes, fast recovery
```

### Conservative (1 Dash + Partial)
```
Max Stamina: 100
Dash Cost: 70
Regen Rate: 15/sec
Regen Delay: 1.5 sec
Result: 1 full dash + 1 partial, slow recovery
```

### Classic Dark Souls
```
Max Stamina: 100
Dash Cost: 35
Attack Cost: 15
Regen Rate: 25/sec
Regen Delay: 1 sec
Result: 2-3 dashes, stamina-based attacks
```

### Bloodborne Style (Fast-Paced)
```
Max Stamina: 120
Dash Cost: 30
Regen Rate: 40/sec
Regen Delay: 0.5 sec
Result: 4 dashes, very fast recovery
```

### Hard Mode (Limited Dashes)
```
Max Stamina: 100
Dash Cost: 100
Regen Rate: 10/sec
Regen Delay: 2 sec
Result: 1 dash only, 10 second recovery
```

---

## UI Layout Suggestions

### Option 1: Stacked Below Health
```
[Health Bar - Red]
████████████████████
HP: 80/100

[Stamina Bar - Yellow]
████████████
```
Position: X: 150, Y: 80

### Option 2: Side by Side
```
[HP ████████████] [SP ████████]
```
Position Health: X: 100, Y: 50
Position Stamina: X: 320, Y: 50

### Option 3: Minimal Corner HUD
```
HP ████████████████
SP ████████████
```
Position: Top-left or bottom-left corner, compact

### Option 4: Center Bottom (Like Dark Souls)
```
        [HP Bar]
        [SP Bar]
```
Position: Center-bottom of screen

---

## Customization Tips

### Change Number of Dashes

**For 3 dashes:**
- Max Stamina: 150
- Dash Cost: 50

**For 4 dashes:**
- Max Stamina: 200
- Dash Cost: 50

**For 1.5 dashes:**
- Max Stamina: 100
- Dash Cost: 65

### Change Regeneration Speed

**Faster (3 seconds to full):**
- Regen Rate: 33

**Slower (10 seconds to full):**
- Regen Rate: 10

**Instant (testing):**
- Regen Rate: 1000
- Regen Delay: 0

### Change Bar Colors

In **StaminaBar** component:

**Green stamina bar:**
- Full: RGB(0, 255, 0)
- Low: RGB(0, 180, 0)
- Empty: RGB(0, 100, 0)

**Blue stamina bar:**
- Full: RGB(0, 200, 255)
- Low: RGB(0, 150, 200)
- Empty: RGB(0, 100, 150)

**Keep yellow (default):**
- Full: RGB(255, 235, 4)
- Low: RGB(204, 153, 0)
- Empty: RGB(127, 76, 0)

---

## Advanced Features

### Add Stamina Cost to Attacks

1. Select **Player** → **PlayerStamina** component
2. Set **Attack Stamina Cost**: 15-20
3. Update your attack code (if needed)

This makes attacks consume stamina like Dark Souls!

### Add Low Stamina Warning

When stamina is low, the bar can flash red. This is already built into StaminaBar - it will flash when empty and you try to dash.

### Stamina Restore Pickups

Create items that restore stamina (like Estus Flask):

```csharp
// Example usage:
PlayerStamina stamina = player.GetComponent<PlayerStamina>();
stamina.RestoreStamina(50); // Restore 50 stamina
```

### Disable Regeneration While Attacking

Prevents stamina from regenerating during attack animations (more challenging):

1. In **PlayerStamina**, uncheck **Can Regen While Moving**
2. This stops regen during all actions

---

## Troubleshooting

### Problem: Stamina bar not visible

**Solutions:**
- Check Canvas exists and is active
- Verify StaminaBar is child of Canvas
- Check Fill image color is not transparent
- Set Canvas to "Screen Space - Overlay"

### Problem: Can dash more than 2 times

**Solutions:**
- Check PlayerStamina component is on Player
- Verify Dash Stamina Cost is 50 (not 0)
- Make sure Max Stamina is 100 (not higher)
- Check Console for stamina messages

### Problem: Stamina not regenerating

**Solutions:**
- Check Regen Rate is > 0 (should be 20)
- Verify Regen Delay is not too high
- Make sure PlayerStamina component is enabled
- Check for errors in Console

### Problem: Bar not yellow

**Solutions:**
- Select StaminaBar → Fill Area → Fill
- Set Image color to RGB(255, 235, 4)
- Check Color Gradient is enabled in StaminaBar component
- Verify Fill Image reference is assigned

### Problem: Dash still works with no stamina

**Solutions:**
- Verify PlayerStamina component is on Player
- Check PlayerMovement is finding PlayerStamina (check Awake)
- Look for "Not enough stamina to dash!" message in Console
- Ensure TryConsumeDashStamina() returns false when empty

### Problem: Bar doesn't move smoothly

**Solutions:**
- In StaminaBar component, check Smooth Transition is ✓
- Increase Transition Speed (try 10)
- Verify Update() is running (no errors in Console)

### Problem: Regeneration too fast/slow

**Solutions:**
- Adjust Stamina Regen Rate in PlayerStamina
- Default 20/sec = full in 5 seconds
- Try 30/sec for faster (3.3 seconds)
- Try 10/sec for slower (10 seconds)

---

## Integration Examples

### With Health System

Position stamina bar near health bar for a unified HUD. Both bars should have similar width and style.

### With Wave System

Optional: Fully restore stamina between waves:

```csharp
// In SimpleWaveManager.cs
PlayerStamina stamina = FindFirstObjectByType<PlayerStamina>();
if (stamina != null)
{
    stamina.FullyRestoreStamina();
}
```

### With Upgrade System

Allow players to upgrade max stamina:

```csharp
public void UpgradeMaxStamina(float amount)
{
    maxStamina += amount;
    currentStamina = maxStamina; // Also refill
    if (staminaBar != null)
    {
        staminaBar.SetMaxStamina(maxStamina);
    }
}
```

### With Difficulty Settings

Adjust stamina costs based on difficulty:

```csharp
// Easy mode
dashStaminaCost = 40; // 2.5 dashes

// Normal mode
dashStaminaCost = 50; // 2 dashes

// Hard mode
dashStaminaCost = 65; // 1.5 dashes
```

---

## Performance Tips

- Stamina system is very lightweight
- Uses simple float math and timer checks
- No performance concerns even with many enemies
- UI updates smoothly without lag

---

## Summary

You've implemented a complete Dark Souls-style stamina system!

**What you have:**
- ✓ Yellow stamina bar UI
- ✓ 2 dashes maximum (50 stamina each)
- ✓ 1 second delay before regeneration
- ✓ 5 second full recovery time
- ✓ Smooth visual animations
- ✓ Color gradient feedback
- ✓ Fully configurable system

**Next steps:**
- Adjust stamina costs to your preference
- Position UI to match your game's style
- Optionally add stamina cost to attacks
- Test and balance for your gameplay

Enjoy your new stamina system!
