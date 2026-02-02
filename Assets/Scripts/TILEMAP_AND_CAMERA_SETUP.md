# Tilemap Expansion and Camera Follow Setup Guide

This guide will help you expand your tilemap to create a larger play area and set up smooth camera following.

## Part 1: Expand the Tilemap

### Method 1: Paint More Tiles (Quick)

1. **Select Ground Tilemap** in Hierarchy
2. **Open Tile Palette** (Window → 2D → Tile Palette)
3. **Select the Tile Brush** tool
4. **Paint more tiles** to expand your map:
   - Recommended size: 50x50 tiles or larger
   - Current looks like: ~15x15 (too small)
   - Good size: 60x40 tiles minimum

**Tips:**
- Hold Shift while painting for straight lines
- Use Rectangle tool for large areas
- Use Fill tool for enclosed areas

### Method 2: Resize Grid (Faster for Large Maps)

1. Select **Grid** in Hierarchy
2. Use the Tile Palette **Rectangle tool**
3. Draw a large rectangle (60 tiles wide x 40 tiles tall)
4. This creates your floor/ground
5. Then paint walls around the edges

### Recommended Map Sizes

**Small dungeon room:**
- 30x30 tiles

**Medium arena:**
- 50x40 tiles (Recommended for your game)

**Large exploration map:**
- 80x60 tiles

**Huge world:**
- 120x100 tiles

---

## Part 2: Setup Camera Follow

### Step 1: Add CameraFollow to Main Camera

1. Select **Main Camera** in Hierarchy
2. **Add Component** → Search "CameraFollow"
3. The script will auto-find the player

### Step 2: Configure Basic Following

In **CameraFollow** component:

**Target:**
- **Target**: Leave empty (auto-finds Player)
- **Auto Find Player**: ✓ (checked)

**Follow Settings:**
- **Follow X**: ✓ (checked)
- **Follow Y**: ✓ (checked)
- **Smooth Speed**: 5 (how fast camera catches up)
- **Offset**: (0, 0, -10) - Z must be negative for 2D!

### Step 3: Set Camera Bounds (Optional but Recommended)

This prevents the camera from showing outside the tilemap.

**Option A: Manual Bounds**

In **CameraFollow** component:

**Bounds:**
- **Use Bounds**: ✓ (checked)
- **Min X**: -25 (adjust based on your map)
- **Max X**: 25
- **Min Y**: -20
- **Max Y**: 20

**How to calculate bounds:**
- Count your tiles in each direction
- If map is 50 tiles wide and each tile is 1 unit:
  - Min X = -25, Max X = 25
- Leave some margin (2-3 tiles) so camera doesn't show edges

**Option B: Auto-Calculate from Tilemap (Advanced)**

Add this to your game initialization:
```csharp
// Get tilemap bounds
Tilemap tilemap = FindFirstObjectByType<Tilemap>();
if (tilemap != null)
{
    CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
    Camera cam = Camera.main;

    float height = cam.orthographicSize * 2;
    float width = height * cam.aspect;

    camFollow.SetBoundsFromTilemap(
        tilemap.localBounds,
        height / 2f,
        width / 2f
    );
}
```

### Step 4: Test Camera Follow

1. **Play the game**
2. **Move the player** with WASD
3. **Camera should smoothly follow**
4. **Camera should stop at edges** (if bounds enabled)

---

## Part 3: Advanced Camera Features

### Smooth Speed

Adjust how fast camera catches up to player:

- **Very Slow**: 2 (cinematic, laggy feel)
- **Slow**: 3-4 (smooth, slight lag)
- **Medium**: 5 (default, balanced)
- **Fast**: 8-10 (responsive, snappy)
- **Instant**: 100 (no smoothing)

### Dead Zone (Optional)

Creates a zone where player can move without camera moving:

- **Use Dead Zone**: ✓
- **Dead Zone Radius**: 2 (units player can move freely)

Good for:
- Reducing camera motion sickness
- Letting player look around without camera moving

### Look Ahead (Optional)

Camera leads player's movement slightly:

- **Use Look Ahead**: ✓
- **Look Ahead Distance**: 2 (how far ahead to look)
- **Look Ahead Speed**: 2 (how quickly to adjust)

Good for:
- Fast-paced games
- Platformers
- Action games

---

## Part 4: Tilemap Design Tips

### Layout Suggestions

**Arena Style (Recommended for your game):**
```
50x40 tiles total
- Outer wall: 2 tiles thick
- Inner play area: 46x36 tiles
- Add obstacles/columns inside for cover
```

**Dungeon Style:**
```
Multiple connected rooms
- Each room: 20x15 tiles
- Corridors: 3-4 tiles wide
- Doors between rooms
```

**Open Field:**
```
80x60 tiles
- Sparse walls/obstacles
- More exploration focused
- Enemies can approach from any direction
```

### Adding Variety

**Obstacles:**
- Place rocks, pillars, crates
- Creates tactical gameplay
- Breaks up sight lines

**Zones:**
- Safe zone (no enemies spawn)
- Danger zones (more enemies)
- Loot zones (health pickups)

**Layers:**
- Ground (floor tiles)
- Walls (collision)
- Decor (non-collision decorations)
- Foreground (appears above player)

---

## Part 5: Performance Tips

### For Large Tilemaps

1. **Use Tilemap Collider Composite:**
   - Reduces collider count
   - Better performance

2. **Chunk Loading (Advanced):**
   - Load/unload distant tiles
   - Only needed for very large maps (200+ tiles)

3. **Occlusion Culling:**
   - Unity automatically culls off-screen tiles
   - No action needed

### Current Map Size Analysis

Your current map appears to be ~15x15 tiles (very small):
- Players feel cramped
- Not much room for combat
- Wave spawning limited

**Recommended:** Expand to at least 50x40 tiles

---

## Part 6: Quick Expansion Steps

### Fastest Way to Expand Your Map

1. **Open Tile Palette** (Window → 2D → Tile Palette)
2. **Select Rectangle Tool** (third icon)
3. **Select Ground tile**
4. **Click and drag** in Scene view to create a 50x40 rectangle
5. **Select Wall tile**
6. **Paint walls** around the outer edge (2 tiles thick)
7. **Add decorations** inside (pillars, obstacles)

### Testing Different Sizes

1. Start with 40x30 - test combat feel
2. If too small, expand to 50x40
3. If still cramped, go to 60x50
4. Find the sweet spot for your game

---

## Part 7: Camera + Tilemap Integration

### Perfect Camera Setup for Expanded Map

Once you've expanded the tilemap:

1. **Calculate camera view size:**
   - Your camera's Orthographic Size: 5
   - This shows: 10 units vertically
   - Aspect ratio 16:9 = ~17.8 units horizontally

2. **Set camera bounds:**
   - If map is 50 tiles wide (-25 to +25)
   - Camera bounds: Min X = -25 + 9 = -16, Max X = 25 - 9 = 16
   - This prevents seeing past map edges

3. **Enable bounds in CameraFollow:**
   - Use Bounds: ✓
   - Set calculated bounds

---

## Part 8: Troubleshooting

### Problem: Camera doesn't follow

**Solutions:**
- Check Player has tag "Player"
- Verify CameraFollow component is on Main Camera
- Check Auto Find Player is enabled
- Look for errors in Console

### Problem: Camera is jerky/stuttering

**Solutions:**
- Move camera logic to LateUpdate() (already done in script)
- Increase Smooth Speed (try 8-10)
- Check player movement is in FixedUpdate()
- Disable Use Dead Zone

### Problem: Can see outside map edges

**Solutions:**
- Enable Use Bounds
- Calculate correct bounds based on your tilemap size
- Make map bigger
- Add decorative tiles outside play area

### Problem: Camera too slow/fast

**Solutions:**
- Too slow: Increase Smooth Speed (try 8)
- Too fast: Decrease Smooth Speed (try 3)
- Instant: Set Smooth Speed to 100

### Problem: Map feels too small

**Solutions:**
- Expand tilemap to 50x40 or larger
- Use Rectangle tool in Tile Palette
- Paint more floor tiles
- Adjust wave spawn bounds in SimpleWaveManager

---

## Part 9: Wave System Integration

After expanding the map, update your wave spawner:

1. Select **WaveManager** in Hierarchy
2. In **SimpleWaveManager** component:
3. Update **Spawn Bounds**:
   - **Use Spawn Bounds**: ✓
   - **Min World X**: -20 (adjust to new map size)
   - **Max World X**: 20
   - **Min World Y**: -15
   - **Max World Y**: 15

This ensures enemies spawn within the new larger map.

---

## Summary

### Quick Checklist

- [ ] Expand tilemap to 50x40 tiles (or larger)
- [ ] Paint floor with Rectangle tool
- [ ] Add walls around edges (2 tiles thick)
- [ ] Add CameraFollow to Main Camera
- [ ] Configure: Smooth Speed = 5, Follow X/Y = ✓
- [ ] Enable Use Bounds
- [ ] Set bounds to match tilemap size
- [ ] Test: Play and move player
- [ ] Update wave spawn bounds to match new map
- [ ] Add obstacles/decorations for variety

### Result

You'll have:
- ✓ Large playable area
- ✓ Smooth camera following player
- ✓ Camera bounded to map edges
- ✓ Professional game feel
- ✓ Room for tactical combat
- ✓ Better enemy spawning

Your game will feel much more polished and spacious!
