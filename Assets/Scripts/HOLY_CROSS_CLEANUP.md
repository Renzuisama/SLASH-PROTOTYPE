# Holy Cross Prefab Cleanup Guide

The Holy Cross prefab includes demo UI text ("MADE BY") that blocks the actual particle effect. Here's how to fix it.

## Quick Fix (Recommended)

### Option 1: Remove Demo UI from Prefab

1. **Find the HolyCross prefab** in Project:
   - Navigate to: `Assets/Pixel Art/PixelArtRPGVFXLite/Prefabs/Holy/`
   - Find `HolyCross.prefab`

2. **Open prefab for editing**:
   - Double-click `HolyCross.prefab` to enter Prefab Mode
   - OR drag it into your scene, edit it, then apply changes back to prefab

3. **Find and delete demo UI**:
   - Look in the Hierarchy for these objects (inside HolyCross):
     - `Canvas` (or `Canvas (Environment)`)
     - Any `Text` objects
     - Any `Demo Canvas` objects
   - **Delete these UI objects** (not the particle systems!)

4. **Keep these important objects**:
   - ✓ Keep all `Particle System` objects
   - ✓ Keep `Frame01`, `Frame02`, etc. (the actual VFX)
   - ✓ Keep anything with "Holy" in the name

5. **Save the prefab**:
   - If in Prefab Mode: Click the `<` arrow at top-left to exit and save
   - If in scene: Right-click the object → Prefab → Apply All

### Option 2: Create Clean Version

1. **Drag HolyCross into your scene**

2. **Expand the hierarchy** to see all children

3. **Delete these objects**:
   - Find `Canvas (Environment)` or any Canvas
   - Delete it
   - Find any Text or UI elements
   - Delete them

4. **Keep only the particle effects**:
   - You should see objects named like:
     - `Frame01`, `Frame02`, `Frame03`, etc.
     - Various particle systems
   - Keep all of these!

5. **Create new prefab**:
   - Drag the cleaned HolyCross from scene back to `Holy` folder
   - Name it `HolyCross_Clean`
   - Delete the instance from scene

6. **Update SpecialAttack component**:
   - Select Player
   - In SpecialAttack component
   - Assign `HolyCross_Clean` instead

## If You Can't Find What to Delete

### Alternative: Scale Up the Effect

The particle effects might be too small to see. Try this:

1. Select Player in Hierarchy
2. Find SpecialAttack component
3. Add this setting at the top of the script (or just test in Inspector after spawning):

**Temporary test solution:**
- When HolyCross spawns in game, select it in Hierarchy
- Set Scale to X: 2, Y: 2, Z: 2 (or higher)
- See if the actual effect becomes visible

### Alternative: Use Different VFX

If Holy Cross is too problematic, try these other effects:

**Explosion (Fire burst):**
- Path: `Assets/Pixel Art/PixelArtRPGVFXLite/Prefabs/Explosion/Explosion_003.prefab`
- Works great for AOE attacks
- No demo UI issues

**Lightning:**
- Path: `Assets/Pixel Art/PixelArtRPGVFXLite/Prefabs/Electricity/ElectricLightning01.prefab`
- Good for special attacks
- Clean prefab

**Water Wave:**
- Path: `Assets/Pixel Art/PixelArtRPGVFXLite/Prefabs/Water/WaterWave.prefab`
- Nice ground-based AOE effect

## Testing the Fix

After cleaning up the prefab:

1. **Play the game**
2. **Get near an enemy**
3. **Press E** to use special attack
4. **Look for**:
   - ✓ Golden/yellow cross particles
   - ✓ Glowing effect at enemy location
   - ✗ NO "MADE BY" text

## If Effect Still Not Visible

### Check Sorting Layers

1. **Select the spawned HolyCross in Hierarchy** (while game is running)
2. **Expand to see particle systems**
3. **Select each Particle System**
4. **In Renderer section**:
   - Set Sorting Layer to: `Default` or your foreground layer
   - Set Order in Layer to: `10` or higher

### Check Camera Settings

Make sure your camera can see particle effects:
- Camera should be **Orthographic** (for 2D)
- Far Clipping Plane should be high enough (100+)

### Check Scale

The effect might be tiny:
1. While game is running, select spawned HolyCross
2. Check Transform Scale in Inspector
3. If it's (1, 1, 1) and still invisible, try:
   - Scale: (3, 3, 3) or even (5, 5, 5)

## Proper Prefab Structure

After cleanup, your HolyCross should look like this in Hierarchy:

```
HolyCross
├── Frame01 (Particle System)
├── Frame02 (Particle System)
├── Frame03 (Particle System)
├── Frame04 (Particle System)
├── Frame05 (Particle System)
└── Frame06 (Particle System)
```

**NO Canvas, NO Text, NO Demo UI**

## Script Solution (If Manual Cleanup is Hard)

Add this to your SpecialAttack.cs script to automatically remove UI:

```csharp
// In ExecuteSpecialAttack(), after spawning Holy Cross:
if (holyCrossPrefab != null)
{
    GameObject holyCross = Instantiate(holyCrossPrefab, targetPosition, Quaternion.identity);

    // Remove demo UI automatically
    Canvas[] canvases = holyCross.GetComponentsInChildren<Canvas>();
    foreach (Canvas canvas in canvases)
    {
        Destroy(canvas.gameObject);
    }

    // Scale up if needed
    holyCross.transform.localScale = new Vector3(2f, 2f, 2f);

    if (debugMode)
    {
        Debug.Log($"Spawned Holy Cross at {targetPosition}");
    }

    Destroy(holyCross, attackDuration);
}
```

This code:
- Automatically finds and removes any Canvas (demo UI)
- Scales the effect to 2x size
- Keeps the actual particle effects

## Recommended Solution

**Use the Script Solution above** - it's the easiest and most reliable:

1. Open `SpecialAttack.cs`
2. Find the `ExecuteSpecialAttack` method
3. Add the Canvas removal code shown above
4. Test in game - UI should be gone!

This way you don't have to manually edit the prefab, and it will work even if the prefab is updated.
