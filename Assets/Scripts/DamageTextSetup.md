# Damage Text Setup Guide

## Scripts Created
1. **DamageText.cs** - Handles the individual damage text animation (movement + fade)
2. **DamageTextManager.cs** - Singleton manager to spawn damage text
3. **EnemyAnimated.cs** - Updated to show damage text when hit

## Setup Instructions

### Step 1: Import TextMeshPro
1. In Unity, go to `Window > TextMeshPro > Import TMP Essential Resources`
2. Click "Import" if you haven't already

### Step 2: Create the Damage Text Prefab
1. In the Hierarchy, right-click and select `Create Empty`
2. Name it "DamageText"
3. Right-click on "DamageText" and select `3D Object > Text - TextMeshPro`
4. In the Inspector for the TextMeshPro component:
   - Set **Font Size**: 36-48 (adjust to your preference)
   - Set **Alignment**: Center (both horizontal and vertical)
   - Set **Color**: White
   - Set **Sorting Layer**: Make sure it's above your sprites
   - **Important**: Set the sorting order high enough to appear above enemies (e.g., 10)
5. Add the **DamageText** script component to the DamageText GameObject
6. Adjust settings in DamageText component:
   - **Move Speed**: 1.5 (how fast it moves up)
   - **Fade Duration**: 1.0 (how long before it disappears)
   - **Move Distance**: 1.0 (how far up it travels)
7. Drag the DamageText GameObject to your Assets/Prefabs folder to create a prefab
8. Delete the DamageText GameObject from the Hierarchy

### Step 3: Setup the Manager
1. In the Hierarchy, create an empty GameObject and name it "DamageTextManager"
2. Add the **DamageTextManager** script component to it
3. In the Inspector:
   - Drag your DamageText prefab into the **Damage Text Prefab** slot
   - Set **Spawn Offset**: (0, 0.5, 0) - adjusts where text appears relative to enemy
   - Set **Normal Damage Color**: White
   - Set **Critical Damage Color**: Red (for future critical hits)

### Step 4: Test
1. Play the game and attack an enemy
2. You should see damage numbers float up and fade out

## Customization Options

### In DamageText.cs:
- **moveSpeed**: How fast the text moves upward
- **fadeDuration**: How long the animation lasts
- **moveDistance**: Total distance the text travels

### In DamageTextManager.cs:
- **spawnOffset**: Position offset from enemy (adjust Y to spawn higher/lower)
- **normalDamageColor**: Color for regular damage
- **criticalDamageColor**: Color for critical hits

### Additional Features You Can Add:
- Random horizontal offset for variety: `spawnOffset + new Vector3(Random.Range(-0.2f, 0.2f), 0, 0)`
- Scale animation (start big, shrink)
- Different colors for different damage types
- Floating arc motion instead of straight up
