# Special Attack Cooldown UI Setup Guide

This guide shows you how to create a cooldown indicator for the special attack in the bottom-left corner of the screen.

## Overview

The cooldown UI shows:
- ✓ Holy Cross icon
- ✓ Radial or vertical cooldown fill (360° or vertical bar)
- ✓ Cooldown timer text ("5.2s" or "READY!")
- ✓ Color changes (ready = white, cooldown = gray, no stamina = red)
- ✓ Pulse animation when ready

---

## Quick Setup (5 Minutes)

### Step 1: Create the UI Container

1. Find your **Canvas** in Hierarchy (or create one: Right-click → UI → Canvas)
2. Right-click on **Canvas** → UI → **Empty**
3. Name it: **SpecialAttackCooldown**
4. Set **Anchor** to: Bottom-Left
   - Click the Anchor preset box → Hold Alt+Shift → Click bottom-left preset
5. Set **Position**:
   - Pos X: 80
   - Pos Y: 80
   - Width: 64
   - Height: 64

### Step 2: Add Background/Icon Image

1. Right-click **SpecialAttackCooldown** → UI → **Image**
2. Name it: **Icon**
3. Configure:
   - **Rect Transform**: Stretch to fill parent (0, 0, 0, 0)
   - **Source Image**: Drag `HolyCross_3` sprite (the cross icon)
   - **Color**: White (255, 255, 255, 255)
   - **Preserve Aspect**: ✓ (optional)

### Step 3: Add Cooldown Fill Image

1. Right-click **SpecialAttackCooldown** → UI → **Image**
2. Name it: **CooldownFill**
3. Configure:
   - **Rect Transform**: Stretch to fill parent (0, 0, 0, 0)
   - **Source Image**: Same as Icon (`HolyCross_3`)
   - **Color**: White (will be controlled by script)
   - **Image Type**: Filled (will be set by script)

### Step 4: Add Cooldown Text

1. Right-click **SpecialAttackCooldown** → UI → **Text - TextMeshPro**
   - If prompted, import TMP Essentials
2. Name it: **CooldownText**
3. Configure:
   - **Rect Transform**:
     - Anchor: Center
     - Pos X: 0, Pos Y: -50 (below the icon)
     - Width: 100, Height: 30
   - **Text**: "READY!"
   - **Font Size**: 18
   - **Alignment**: Center (Horizontal and Vertical)
   - **Color**: White

### Step 5: Add the Script

1. Select **SpecialAttackCooldown** (the parent)
2. **Add Component** → **SpecialAttackCooldownUI**
3. Configure in Inspector:

**References:**
- **Special Attack**: Drag your **Player** GameObject here
- **Cooldown Icon**: Drag the **Icon** child
- **Cooldown Fill**: Drag the **CooldownFill** child
- **Cooldown Text**: Drag the **CooldownText** child

**Cooldown Display Type:**
- **Cooldown Type**: Radial (or Vertical/Horizontal)

**Visual Settings:**
- **Ready Color**: White (255, 255, 255)
- **Cooldown Color**: Gray (150, 150, 150)
- **Not Enough Stamina Color**: Light Red (255, 128, 128)
- **Show Cooldown Text**: ✓
- **Show Ready Text**: ✓
- **Ready Text Message**: "READY!"

**Animation:**
- **Pulse When Ready**: ✓
- **Pulse Speed**: 2
- **Pulse Scale**: 1.1

### Step 6: Test!

1. **Play the game**
2. **Press E** to use special attack
3. **Watch the UI**:
   - ✓ Icon turns gray
   - ✓ Fill drains (radial or vertical)
   - ✓ Text shows countdown "10.0s... 5.2s..."
   - ✓ When ready, icon turns white
   - ✓ Text shows "READY!"
   - ✓ Icon pulses gently

✅ **Done!**

---

## Cooldown Display Types

### Radial (360° - Recommended)

**Best for**: Circular icons, modern UI
```
Cooldown Type: Radial
Result: Fills clockwise from top (like a pie chart)
```

**How it looks:**
- Full circle = Ready
- Empty circle = On cooldown
- Gradually fills as cooldown progresses

### Vertical (Bottom-to-Top)

**Best for**: Rectangular icons, traditional UI
```
Cooldown Type: Vertical
Result: Fills from bottom to top
```

**How it looks:**
- Full bar = Ready
- Empty bar = On cooldown
- Bar rises as cooldown progresses

### Horizontal (Left-to-Right)

**Best for**: Wide icons
```
Cooldown Type: Horizontal
Result: Fills from left to right
```

**How it looks:**
- Full bar = Ready
- Empty bar = On cooldown
- Bar fills left to right

---

## Customization

### Change Size

Select **SpecialAttackCooldown**:
- Small: Width/Height = 48
- Medium: Width/Height = 64 (default)
- Large: Width/Height = 80
- Extra Large: Width/Height = 100

### Change Position

Select **SpecialAttackCooldown**:

**Bottom-Left (default):**
- Pos X: 80, Pos Y: 80

**Bottom-Right:**
- Anchor: Bottom-Right
- Pos X: -80, Pos Y: 80

**Top-Left:**
- Anchor: Top-Left
- Pos X: 80, Pos Y: -80

**Bottom-Center:**
- Anchor: Bottom-Center
- Pos X: 0, Pos Y: 80

### Change Colors

In **SpecialAttackCooldownUI** component:

**Ready (white - default):**
- RGB(255, 255, 255)

**Ready (yellow):**
- RGB(255, 235, 0)

**Ready (green):**
- RGB(0, 255, 0)

**Cooldown (dark gray):**
- RGB(100, 100, 100)

**No Stamina (red):**
- RGB(255, 100, 100)

### Add Background

1. Right-click **SpecialAttackCooldown** → UI → **Image**
2. Name it: **Background**
3. Move it to top of hierarchy (first child)
4. Configure:
   - Color: Dark (0, 0, 0, 180) - semi-transparent black
   - Rect: Slightly larger than icon (10px padding)

### Add Border/Frame

1. Create another Image as child
2. Name: **Border**
3. Use a border sprite or colored outline
4. Place it above the icon

### Add Glow Effect

1. Select **Icon** or **CooldownFill**
2. Add Component → **Outline** (Unity UI component)
3. Or use a sprite with glow built-in

---

## Advanced Features

### Change Icon at Runtime

```csharp
SpecialAttackCooldownUI cooldownUI = FindFirstObjectByType<SpecialAttackCooldownUI>();
Sprite newIcon = Resources.Load<Sprite>("Icons/NewIcon");
cooldownUI.SetIconSprite(newIcon);
```

### Change Cooldown Type at Runtime

```csharp
SpecialAttackCooldownUI cooldownUI = FindFirstObjectByType<SpecialAttackCooldownUI>();
cooldownUI.SetCooldownType(SpecialAttackCooldownUI.CooldownType.Vertical);
```

### Multiple Special Attacks

Create multiple cooldown UIs:
1. Duplicate the SpecialAttackCooldown GameObject
2. Position them side by side
3. Assign different SpecialAttack references
4. Different icons for each

### Add Key Binding Display

1. Add another Text child to SpecialAttackCooldown
2. Set text to "E"
3. Position above or below the icon
4. Style to show the key to press

---

## Troubleshooting

### Problem: UI doesn't update

**Solutions:**
- Check SpecialAttack reference is assigned
- Verify Player has SpecialAttack component
- Check all image/text references are assigned
- Look for errors in Console

### Problem: No icon visible

**Solutions:**
- Check Icon image has a sprite assigned
- Verify sprite is not transparent
- Check Canvas Render Mode (should be Screen Space - Overlay)
- Increase icon color alpha to 255

### Problem: Fill doesn't animate

**Solutions:**
- Verify CooldownFill reference is assigned
- Check Image Type is "Filled" (script sets this)
- Try changing Cooldown Type
- Check fillAmount is changing (add Debug.Log)

### Problem: Text doesn't show

**Solutions:**
- Check CooldownText reference is assigned
- Verify font is not missing
- Check text color is not transparent
- Make sure text is large enough to read

### Problem: Colors don't change

**Solutions:**
- Check color values in Inspector
- Verify Ready/Cooldown colors are different enough
- Make sure alpha is 255 (opaque)

### Problem: No pulse animation

**Solutions:**
- Check "Pulse When Ready" is enabled
- Increase Pulse Scale (try 1.3)
- Increase Pulse Speed (try 3)
- Verify icon is ready (white color)

---

## Layout Examples

### Example 1: Bottom-Left Corner
```
Position: (80, 80)
Size: 64x64
Anchor: Bottom-Left
Type: Radial
```

### Example 2: Near Stamina Bar
```
Position: Below stamina bar
Size: 48x48
Anchor: Bottom-Left
Type: Radial
```

### Example 3: Action Bar Style
```
Position: Bottom-Center
Size: 80x80
Multiple icons side by side
Type: Radial for each
```

### Example 4: Minimalist
```
Position: Bottom-Left
Size: 40x40
No text, only icon + radial
Type: Radial
```

---

## Integration with Other Systems

### Show/Hide Based on Unlock

```csharp
// Hide until player unlocks special attack
SpecialAttackCooldownUI cooldownUI = FindFirstObjectByType<SpecialAttackCooldownUI>();
cooldownUI.gameObject.SetActive(false);

// Show when unlocked
cooldownUI.gameObject.SetActive(true);
```

### Disable During Cutscenes

```csharp
SpecialAttackCooldownUI cooldownUI = FindFirstObjectByType<SpecialAttackCooldownUI>();
cooldownUI.enabled = false; // Stops updating

// Re-enable
cooldownUI.enabled = true;
```

### Flash on Cooldown Complete

Add to SpecialAttackCooldownUI.cs Update():
```csharp
if (!wasReady && isReady)
{
    // Just became ready - flash effect
    StartCoroutine(FlashEffect());
}
```

---

## Performance Notes

- Very lightweight (just UI updates)
- Updates every frame but minimal cost
- No performance concerns
- Can have multiple cooldown UIs

---

## Summary

You now have a professional cooldown indicator!

**What you have:**
- ✓ Radial or vertical cooldown display
- ✓ Holy Cross icon
- ✓ Countdown timer text
- ✓ Color coding (ready/cooldown/no stamina)
- ✓ Pulse animation when ready
- ✓ Automatic stamina checking
- ✓ Fully customizable

**Result:**
- Press E → Cooldown starts
- Icon turns gray, fill drains
- Text shows remaining time
- When ready, icon pulses white
- Shows "READY!" or "NO STAMINA"

Professional and polished UI!
