using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class EnemyAnimationBuilder
{
    private const float DefaultFrameRate = 12f;
    private const string EnemySheetPath = "Assets/png/Object/enemy.png";
    private const string DefaultAnimationFolder = "Assets/Animations/Enemies";

    [MenuItem("Tools/Enemy/Create Animation/From Selection")]
    public static void CreateAnimationFromSelection()
    {
        var sprites = Selection.GetFiltered<Sprite>(SelectionMode.Assets);
        if (sprites == null || sprites.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Create Enemy Animation",
                "Select one or more Sprite assets (enemy_0..enemy_6, etc.) in the Project window.",
                "OK");
            return;
        }

        CreateAnimationFromSprites(sprites, "EnemyIdle");
    }

    [MenuItem("Tools/Enemy/Create Animation/Enemy 0-6")]
    public static void CreateEnemy0To6()
    {
        CreateAnimationFromEnemySheetRange(0, 6, "Enemy_0_6");
    }

    [MenuItem("Tools/Enemy/Create Animation/Enemy 7-13")]
    public static void CreateEnemy7To13()
    {
        CreateAnimationFromEnemySheetRange(7, 13, "Enemy_7_13");
    }

    [MenuItem("Tools/Enemy/Create Animation/Enemy 14-20")]
    public static void CreateEnemy14To20()
    {
        CreateAnimationFromEnemySheetRange(14, 20, "Enemy_14_20");
    }

    [MenuItem("Tools/Enemy/Create Animation/Enemy 21-27")]
    public static void CreateEnemy21To27()
    {
        CreateAnimationFromEnemySheetRange(21, 27, "Enemy_21_27");
    }

    [MenuItem("Tools/Enemy/Create Animation/Enemy 28-34")]
    public static void CreateEnemy28To34()
    {
        CreateAnimationFromEnemySheetRange(28, 34, "Enemy_28_34");
    }

    [MenuItem("Tools/Enemy/Create Animation/Enemy 35-41")]
    public static void CreateEnemy35To41()
    {
        CreateAnimationFromEnemySheetRange(35, 41, "Enemy_35_41");
    }

    [MenuItem("Tools/Enemy/Create Animation/Enemy 42-44")]
    public static void CreateEnemy42To44()
    {
        CreateAnimationFromEnemySheetRange(42, 44, "Enemy_42_44");
    }

    private static void CreateAnimationFromEnemySheetRange(int start, int end, string clipName)
    {
        var sprites = LoadEnemySpritesFromSheet(start, end);
        if (sprites.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Create Enemy Animation",
                $"No sprites found in {EnemySheetPath}. Make sure the file exists and is sliced.",
                "OK");
            return;
        }

        CreateAnimationFromSprites(sprites, clipName);
    }

    private static Sprite[] LoadEnemySpritesFromSheet(int start, int end)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath(EnemySheetPath);
        return assets
            .OfType<Sprite>()
            .Where(sprite =>
            {
                var index = ExtractEnemyIndex(sprite.name);
                return index >= start && index <= end;
            })
            .OrderBy(sprite => ExtractEnemyIndex(sprite.name))
            .ThenBy(sprite => sprite.name, System.StringComparer.Ordinal)
            .ToArray();
    }

    private static void CreateAnimationFromSprites(Sprite[] sprites, string defaultClipName)
    {
        var sortedSprites = sprites
            .OrderBy(sprite => ExtractEnemyIndex(sprite.name))
            .ThenBy(sprite => sprite.name, System.StringComparer.Ordinal)
            .ToArray();

        EnsureFolderExists("Assets", "Animations");
        EnsureFolderExists("Assets/Animations", "Enemies");

        var clipPath = EditorUtility.SaveFilePanelInProject(
            "Save Animation Clip",
            defaultClipName,
            "anim",
            "Choose where to save the animation clip.",
            DefaultAnimationFolder);

        if (string.IsNullOrWhiteSpace(clipPath))
        {
            return;
        }

        var clip = new AnimationClip
        {
            frameRate = DefaultFrameRate
        };

        var keyFrames = new ObjectReferenceKeyframe[sortedSprites.Length];
        for (var i = 0; i < sortedSprites.Length; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / DefaultFrameRate,
                value = sortedSprites[i]
            };
        }

        var binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = string.Empty,
            propertyName = "m_Sprite"
        };

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyFrames);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, clipPath);
        AssetDatabase.SaveAssets();

        var controllerPath = clipPath.Replace(".anim", ".controller");
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        controller.layers[0].stateMachine.states[0].state.motion = clip;
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Create Enemy Animation",
            "Animation clip and controller created.\nAssign the controller to your Enemy's Animator.",
            "OK");
    }

    private static int ExtractEnemyIndex(string name)
    {
        if (name.StartsWith("enemy_", System.StringComparison.Ordinal) &&
            int.TryParse(name.Substring("enemy_".Length), out var index))
        {
            return index;
        }

        return int.MaxValue;
    }

    private static void EnsureFolderExists(string parent, string folder)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{folder}"))
        {
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
