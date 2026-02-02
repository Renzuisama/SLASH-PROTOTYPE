using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper tool to identify wave system conflicts
/// </summary>
public class WaveSystemChecker : EditorWindow
{
    [MenuItem("Tools/Check Wave System")]
    public static void ShowWindow()
    {
        GetWindow<WaveSystemChecker>("Wave System Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Wave System Status", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Check Scene"))
        {
            CheckScene();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Fix: Remove Old Wave System"))
        {
            RemoveOldSystem();
        }
    }

    private void CheckScene()
    {
        Debug.Log("=== WAVE SYSTEM CHECK ===");

        // Check for old system
        WaveDirector oldDirector = FindObjectOfType<WaveDirector>();
        WaveUI oldUI = FindObjectOfType<WaveUI>();

        // Check for new system
        SimpleWaveManager newManager = FindObjectOfType<SimpleWaveManager>();
        SimpleWaveUI newUI = FindObjectOfType<SimpleWaveUI>();

        Debug.Log("\n--- OLD SYSTEM (Should be removed) ---");
        if (oldDirector != null)
        {
            Debug.LogWarning($"❌ Found OLD WaveDirector on: {oldDirector.gameObject.name}", oldDirector);
        }
        else
        {
            Debug.Log("✓ No old WaveDirector found");
        }

        if (oldUI != null)
        {
            Debug.LogWarning($"❌ Found OLD WaveUI on: {oldUI.gameObject.name}", oldUI);
        }
        else
        {
            Debug.Log("✓ No old WaveUI found");
        }

        Debug.Log("\n--- NEW SYSTEM (Should exist) ---");
        if (newManager != null)
        {
            Debug.Log($"✓ Found NEW SimpleWaveManager on: {newManager.gameObject.name}", newManager);
        }
        else
        {
            Debug.LogError("❌ No SimpleWaveManager found! You need to add this.");
        }

        if (newUI != null)
        {
            Debug.Log($"✓ Found NEW SimpleWaveUI on: {newUI.gameObject.name}", newUI);
        }
        else
        {
            Debug.LogError("❌ No SimpleWaveUI found! You need to add this.");
        }

        Debug.Log("\n=== END CHECK ===");

        if (oldDirector != null || oldUI != null)
        {
            Debug.LogWarning("\n⚠️ CONFLICT DETECTED! You have both old and new systems. Click 'Fix: Remove Old Wave System' to fix.");
        }
        else if (newManager != null && newUI != null)
        {
            Debug.Log("\n✓ Everything looks good! Using new simple system.");
        }
    }

    private void RemoveOldSystem()
    {
        if (!EditorUtility.DisplayDialog("Remove Old Wave System",
            "This will remove WaveDirector and WaveUI components from your scene. Continue?",
            "Yes, Remove Them", "Cancel"))
        {
            return;
        }

        int removed = 0;

        // Remove old WaveDirector
        WaveDirector oldDirector = FindObjectOfType<WaveDirector>();
        if (oldDirector != null)
        {
            Debug.Log($"Removing WaveDirector from: {oldDirector.gameObject.name}");
            DestroyImmediate(oldDirector);
            removed++;
        }

        // Remove old WaveUI
        WaveUI oldUI = FindObjectOfType<WaveUI>();
        if (oldUI != null)
        {
            Debug.Log($"Removing WaveUI from: {oldUI.gameObject.name}");
            DestroyImmediate(oldUI);
            removed++;
        }

        if (removed > 0)
        {
            Debug.Log($"✓ Removed {removed} old component(s). Scene is now clean!");
            EditorUtility.DisplayDialog("Success", $"Removed {removed} old component(s). Your scene now uses only the new simple system.", "OK");
        }
        else
        {
            Debug.Log("No old components found to remove.");
            EditorUtility.DisplayDialog("Info", "No old components found. Your scene is already clean!", "OK");
        }
    }
}
