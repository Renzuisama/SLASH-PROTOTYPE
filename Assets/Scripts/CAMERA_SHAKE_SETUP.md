# Camera Shake Setup Guide

This guide shows you how to add camera shake to your special attack.

## Quick Setup (2 Steps)

### Step 1: Add CameraShake to Main Camera

1. Select **Main Camera** in Hierarchy
2. Click **Add Component**
3. Search for **"CameraShake"**
4. Click to add it

**Configure settings (optional):**
- **Default Duration**: 0.5 seconds (how long shake lasts)
- **Default Magnitude**: 0.3 (shake intensity, 0.1-0.5 recommended)

### Step 2: Configure Special Attack Shake

The SpecialAttack component already has camera shake settings!

1. Select **Player** GameObject
2. Find **SpecialAttack** component
3. Expand **Camera Shake** section:
   - **Enable Camera Shake**: ✓ (checked)
   - **Shake Duration**: 0.5 (seconds)
   - **Shake Magnitude**: 0.3 (intensity)

### Step 3: Test!

1. **Play the game**
2. **Press E** to use special attack
3. **You should see**:
   - ✓ Camera shakes when Holy Cross appears
   - ✓ Smooth shake with gradual fade-out
   - ✓ Returns to normal position

✅ **Done!**

---

## Customization

### Shake Intensity

In **SpecialAttack** → Camera Shake:

**Light shake (subtle):**
- Shake Magnitude: 0.1-0.15

**Medium shake (noticeable - default):**
- Shake Magnitude: 0.3

**Heavy shake (intense):**
- Shake Magnitude: 0.5-0.7

**Extreme shake (earthquake):**
- Shake Magnitude: 1.0+

### Shake Duration

In **SpecialAttack** → Camera Shake:

**Quick shake:**
- Shake Duration: 0.2-0.3 seconds

**Medium shake (default):**
- Shake Duration: 0.5 seconds

**Long shake:**
- Shake Duration: 0.8-1.0 seconds

**Very long shake:**
- Shake Duration: 1.5+ seconds

---

## Configuration Examples

### Default (Balanced)
```
Duration: 0.5s
Magnitude: 0.3
Result: Noticeable but not jarring
```

### Subtle (Minimal)
```
Duration: 0.3s
Magnitude: 0.15
Result: Gentle vibration
```

### Powerful (Impactful)
```
Duration: 0.7s
Magnitude: 0.5
Result: Strong shake, feels powerful
```

### Explosive (Screen Rumble)
```
Duration: 1.0s
Magnitude: 0.8
Result: Intense earthquake effect
```

---

## Using Camera Shake Elsewhere

The CameraShake component can be used for other effects too!

### In Your Scripts

```csharp
// Get reference to camera shake
CameraShake shake = Camera.main.GetComponent<CameraShake>();

// Trigger shake with custom values
if (shake != null)
{
    shake.Shake(0.5f, 0.3f); // duration, magnitude
}
```

### Preset Shake Types

The CameraShake component has built-in presets:

```csharp
CameraShake shake = Camera.main.GetComponent<CameraShake>();

// Light shake (quick, subtle)
shake.ShakeLight(); // 0.3s, 0.1 magnitude

// Medium shake (balanced)
shake.ShakeMedium(); // 0.5s, 0.2 magnitude

// Heavy shake (strong impact)
shake.ShakeHeavy(); // 0.7s, 0.4 magnitude

// Explosion shake (extreme)
shake.ShakeExplosion(); // 0.8s, 0.5 magnitude
```

### Example Uses

**On enemy death:**
```csharp
// In EnemyAnimated.cs HandleDeath()
CameraShake shake = Camera.main.GetComponent<CameraShake>();
if (shake != null)
{
    shake.ShakeLight(); // Small shake on death
}
```

**On player hit:**
```csharp
// In PlayerHealth.cs TakeDamage()
CameraShake shake = Camera.main.GetComponent<CameraShake>();
if (shake != null)
{
    shake.ShakeMedium(); // Medium shake when hurt
}
```

**On dash through enemies:**
```csharp
// In DashHitbox.cs when hitting multiple enemies
if (hitEnemies.Count > 3) // If hit 3+ enemies
{
    CameraShake shake = Camera.main.GetComponent<CameraShake>();
    if (shake != null)
    {
        shake.ShakeHeavy(); // Heavy shake for combo
    }
}
```

**On wave complete:**
```csharp
// In SimpleWaveManager.cs
CameraShake shake = Camera.main.GetComponent<CameraShake>();
if (shake != null)
{
    shake.Shake(0.3f, 0.2f); // Quick celebratory shake
}
```

---

## Troubleshooting

### Problem: No shake happens

**Solutions:**
- Check CameraShake component is on Main Camera
- Verify "Enable Camera Shake" is checked in SpecialAttack
- Check Console for "CameraShake component not found" warning
- Make sure Camera has tag "MainCamera"

### Problem: Shake is too weak

**Solutions:**
- Increase Shake Magnitude (try 0.5 or 0.7)
- Increase Shake Duration (try 0.8)
- Check camera is Orthographic (2D games)

### Problem: Shake is too strong/jarring

**Solutions:**
- Decrease Shake Magnitude (try 0.15 or 0.2)
- Decrease Shake Duration (try 0.3)

### Problem: Shake doesn't return to center

**Solutions:**
- Check no other scripts are moving the camera
- Verify camera's local position is (0, 0, 0) initially
- Try calling `shake.StopShake()` manually

### Problem: Shake continues forever

**Solutions:**
- Check Shake Duration is not 0
- Call `shake.StopShake()` to force stop
- Look for errors in Console

---

## Advanced Features

### Gradual Fade-Out

The shake automatically fades out over time for a smoother ending. This is built-in and doesn't need configuration.

### Stop Shake Early

```csharp
CameraShake shake = Camera.main.GetComponent<CameraShake>();
shake.StopShake(); // Immediately stop and return to center
```

### Check if Shaking

```csharp
CameraShake shake = Camera.main.GetComponent<CameraShake>();
if (shake.IsShaking())
{
    Debug.Log("Camera is currently shaking");
}
```

### Chain Multiple Shakes

```csharp
// First shake
shake.Shake(0.3f, 0.2f);

// Wait a bit, then second shake
yield return new WaitForSeconds(0.5f);
shake.Shake(0.4f, 0.3f);
```

### Shake Based on Damage

```csharp
public void ShakeBasedOnDamage(int damage)
{
    CameraShake shake = Camera.main.GetComponent<CameraShake>();
    if (shake != null)
    {
        float magnitude = Mathf.Clamp(damage / 100f, 0.1f, 0.8f);
        shake.Shake(0.5f, magnitude);
    }
}
```

---

## Performance Notes

- Camera shake is very lightweight
- No performance impact even with frequent use
- Safe to use multiple times per second
- Automatically stops and cleans up

---

## Summary

You now have camera shake on your special attack!

**What you have:**
- ✓ Smooth camera shake effect
- ✓ Configurable duration and intensity
- ✓ Gradual fade-out for smooth ending
- ✓ Can be used for other effects too
- ✓ Multiple preset shake types
- ✓ Fully customizable

**Result:**
- Press E → Holy Cross appears with satisfying camera shake
- Makes the special attack feel more powerful
- Professional polish and game feel

Enjoy your enhanced special attack!
