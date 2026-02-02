# Holy Cross Animation Setup Guide

This guide shows you how to set up the Holy Cross sprite sheet animation for the special attack.

## Overview

The `HolyCross_Lite.png` texture (64x384) is a sprite sheet containing 6 frames of animation (each frame is 64x64). We need to:
1. Slice the sprite sheet into 6 frames
2. Create an animated prefab
3. Integrate it with SpecialAttack

---

## Step 1: Slice the Sprite Sheet

### 1.1 Select the Texture

1. Navigate to: `Assets/Pixel Art/PixelArtRPGVFXLite/Textures/Holy/`
2. Click on **HolyCross_Lite**

### 1.2 Configure Import Settings

In the Inspector (with HolyCross_Lite selected):

1. **Texture Type**: `Sprite (2D and UI)` ✓ (already set)
2. **Sprite Mode**: Change from `Single` to **`Multiple`**
3. **Pixels Per Unit**: 64 (or leave at default)
4. **Filter Mode**: `Point (no filter)` (for crisp pixel art)
5. **Compression**: `None` (for best quality)
6. Click **Apply**

### 1.3 Open Sprite Editor

1. With HolyCross_Lite still selected
2. Click **Sprite Editor** button at top of Inspector
3. Sprite Editor window will open

### 1.4 Slice the Sprite

In Sprite Editor:

1. Click **Slice** dropdown at the top
2. **Type**: `Grid By Cell Count`
3. **Column**: `1`
4. **Row**: `6` (there are 6 frames stacked vertically)
5. Click **Slice** button
6. You should see 6 rectangles dividing the sprite
7. Click **Apply** at top-right
8. Close Sprite Editor

### 1.5 Verify the Frames

1. In Project window, expand the **HolyCross_Lite** texture (click the arrow)
2. You should now see 6 sub-sprites:
   - HolyCross_Lite_0
   - HolyCross_Lite_1
   - HolyCross_Lite_2
   - HolyCross_Lite_3
   - HolyCross_Lite_4
   - HolyCross_Lite_5

✅ **Sprite sheet is now sliced!**

---

## Step 2: Create the Animated Holy Cross Prefab

### 2.1 Create Empty GameObject

1. In Hierarchy, right-click → **Create Empty**
2. Name it: **HolyCross_Animated**
3. **Position**: (0, 0, 0)

### 2.2 Add Sprite Renderer

1. Select HolyCross_Animated
2. **Add Component** → **Sprite Renderer**
3. In Sprite Renderer:
   - **Sprite**: Drag `HolyCross_Lite_0` (first frame)
   - **Color**: White (255, 255, 255, 255)
   - **Sorting Layer**: Default
   - **Order in Layer**: 100

### 2.3 Add HolyCrossAnimator Script

1. With HolyCross_Animated still selected
2. **Add Component** → Search **"HolyCrossAnimator"**
3. Click to add it

### 2.4 Configure Animation

In **HolyCrossAnimator** component:

**Animation Settings:**
- **Animation Frames**: Click the lock icon → Set **Size**: `6`
- Drag each frame from Project to the array:
  - Element 0: `HolyCross_Lite_0`
  - Element 1: `HolyCross_Lite_1`
  - Element 2: `HolyCross_Lite_2`
  - Element 3: `HolyCross_Lite_3`
  - Element 4: `HolyCross_Lite_4`
  - Element 5: `HolyCross_Lite_5`
- **Frame Rate**: `12` (12 frames per second - adjust for speed)
- **Loop**: ✗ (unchecked - play once)
- **Play On Start**: ✓ (checked)

**Visual Settings:**
- **Scale**: `2` (makes it 2x larger)
- **Tint Color**: White (or yellow for holy effect)

### 2.5 Test the Animation

1. **Play the game**
2. The animation should play automatically
3. If it's too fast/slow, adjust **Frame Rate**:
   - Slower: 8-10 fps
   - Medium: 12 fps (default)
   - Faster: 15-20 fps

### 2.6 Create Prefab

1. Drag **HolyCross_Animated** from Hierarchy to Project
2. Put it in: `Assets/Prefabs/` (or create a Prefabs folder)
3. Delete the instance from the scene (you only need the prefab)

✅ **Animated prefab is ready!**

---

## Step 3: Integrate with Special Attack

### 3.1 Assign the New Prefab

1. Select your **Player** GameObject
2. Find **SpecialAttack** component
3. In **Holy Cross Prefab** field:
   - **Remove** the old HolyCross prefab
   - **Drag** the new `HolyCross_Animated` prefab

### 3.2 Update SpecialAttack Script

The script needs a small update to handle sprite-based animation instead of Canvas:

Open `SpecialAttack.cs` and the spawn code is already good! The sprite-based animation will work with the `else` branch (line 167-176).

### 3.3 Adjust Scale if Needed

In SpecialAttack.cs, line 170 sets scale to 1.5x. You can adjust:
- In **HolyCrossAnimator**: Scale = 2 (controls prefab default)
- In **SpecialAttack.cs** line 170: Multiplies the scale again

**Recommended:** Set HolyCrossAnimator Scale to `1`, then control size in SpecialAttack:
```csharp
holyCross.transform.localScale = new Vector3(2f, 2f, 2f); // Line 170
```

---

## Step 4: Test the Complete System

1. **Play the game**
2. **Get near enemies**
3. **Wait for full stamina** (yellow bar full)
4. **Press E**

**You should see:**
- ✓ Holy Cross sprite animation plays at enemy location
- ✓ Animation cycles through all 6 frames
- ✓ Effect appears at correct size
- ✓ Enemies within AOE take damage

---

## Troubleshooting

### Problem: Animation doesn't play

**Solutions:**
- Check all 6 frames are assigned in HolyCrossAnimator
- Verify "Play On Start" is checked
- Check Frame Rate is > 0
- Look for errors in Console

### Problem: Effect is too small/large

**Solutions:**
- Adjust **Scale** in HolyCrossAnimator (try 2-4)
- Or adjust line 170 in SpecialAttack.cs:
  ```csharp
  // Larger
  holyCross.transform.localScale = new Vector3(3f, 3f, 3f);

  // Smaller
  holyCross.transform.localScale = new Vector3(1f, 1f, 1f);
  ```

### Problem: Animation plays too fast/slow

**Solutions:**
- Slower: Frame Rate = 8-10
- Medium: Frame Rate = 12 (default)
- Faster: Frame Rate = 15-20

### Problem: Can't see the effect

**Solutions:**
- Check Sprite Renderer is enabled
- Verify Sorting Layer is correct (Default)
- Increase Order in Layer to 100+
- Check Z position is 0
- Verify effect is spawning (check Console logs)

### Problem: Sprite sheet didn't slice correctly

**Solutions:**
- Make sure Sprite Mode is "Multiple" not "Single"
- In Sprite Editor, use Grid By Cell Count
- Column = 1, Row = 6
- Each frame should be 64x64 pixels
- Click Apply in Sprite Editor

### Problem: Wrong frames or order

**Solutions:**
- Frames should go from top to bottom:
  - Frame 0 (top) = first frame
  - Frame 5 (bottom) = last frame
- Re-slice if needed
- Verify array order in HolyCrossAnimator

---

## Customization

### Change Animation Speed

In **HolyCrossAnimator**:
- Very Slow: Frame Rate = 6
- Slow: Frame Rate = 10
- Medium: Frame Rate = 12 (default)
- Fast: Frame Rate = 18
- Very Fast: Frame Rate = 24

### Make it Loop

In **HolyCrossAnimator**:
- Check **Loop** = ✓
- Animation will repeat until effect is destroyed

### Add Color Tint

In **HolyCrossAnimator**, Tint Color:
- **Yellow/Gold**: RGB(255, 220, 100) - holy effect
- **White**: RGB(255, 255, 255) - default
- **Blue**: RGB(100, 200, 255) - magic effect
- **Red**: RGB(255, 100, 100) - fire effect

### Make it Bigger

In **HolyCrossAnimator**:
- Small: Scale = 1
- Medium: Scale = 2 (default)
- Large: Scale = 3
- Huge: Scale = 5

### Use Different Sprite Sheet

If you want to use a different effect:

1. Find the sprite sheet in Textures folder
2. Slice it (same process)
3. Create new prefab with HolyCrossAnimator
4. Assign frames
5. Replace in SpecialAttack

---

## Alternative: Use Unity Animator Instead

If you prefer Unity's built-in Animation system:

### Create Animation Clip

1. Select HolyCross_Animated in scene
2. Open Animation window (Window → Animation → Animation)
3. Click "Create" button
4. Name it: `HolyCross_Animation`
5. Click "Add Property" → Sprite Renderer → Sprite
6. Drag all 6 frames into the timeline
7. Adjust timing by dragging frames
8. Save

### This gives you:
- More control over timing
- Can add easing/curves
- Can trigger events
- More complex animation features

**But the HolyCrossAnimator script is simpler for basic frame animation!**

---

## Summary

You now have a fully animated Holy Cross special attack!

**What you created:**
1. ✓ Sliced sprite sheet into 6 frames
2. ✓ Created animated prefab with HolyCrossAnimator
3. ✓ Integrated with SpecialAttack system
4. ✓ Customizable scale, speed, and color

**Result:**
- Press E → Beautiful animated Holy Cross appears
- Cycles through 6 frames smoothly
- Damages all enemies in AOE
- Looks professional and polished!

Enjoy your new animated special attack!
