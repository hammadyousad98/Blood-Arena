using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateFighterPrefabs
{
    [MenuItem("Tools/BloodArena/Create Fighter Prefabs")]
    public static void CreatePrefabs()
    {
        string prefabsFolder = "Assets/03_Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabsFolder))
        {
            Directory.CreateDirectory(Application.dataPath + "/03_Assets/Prefabs");
            AssetDatabase.Refresh();
        }

        GameObject ragnarPrefab = CreateFighterPrefab("Ragnar", PlayerIndex.Player1, 1.8f);
        GameObject dariusPrefab = CreateFighterPrefab("Darius", PlayerIndex.Player2, 1.7f);

        CreateGameManager(ragnarPrefab, dariusPrefab);
        
        Debug.Log("Fighter prefabs and GameManager created successfully!");
    }

    private static GameObject CreateFighterPrefab(string fighterName, PlayerIndex playerIndex, float height)
    {
        string prefabPath = $"Assets/03_Assets/Prefabs/{fighterName}.prefab";

        GameObject root = new GameObject(fighterName);
        
        FighterController controller = root.AddComponent<FighterController>();
        InputReader inputReader = root.AddComponent<InputReader>();
        Rigidbody rb = root.AddComponent<Rigidbody>(); // RequiredComponent on FighterController might add this automatically, but we can explicitly set it or use GetComponent
        CapsuleCollider col = root.AddComponent<CapsuleCollider>();

        // Since FighterController has [RequireComponent(typeof(Rigidbody))], adding it might create Rigidbody first.
        // GetComponent is safer to use here.
        if (rb == null) rb = root.GetComponent<Rigidbody>();

        rb.mass = 1f;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        col.height = height;
        col.radius = 0.35f;
        col.center = new Vector3(0, height / 2f, 0); // 0.9 for 1.8, 0.85 for 1.7

        inputReader.playerIndex = playerIndex;
        
        // Link references on controller
        controller.inputReader = inputReader;

        // Children
        new GameObject("Model").transform.SetParent(root.transform);
        new GameObject("HitboxRoot").transform.SetParent(root.transform);
        new GameObject("HurtboxRoot").transform.SetParent(root.transform);

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        GameObject.DestroyImmediate(root);

        return savedPrefab;
    }

    private static void CreateGameManager(GameObject p1Prefab, GameObject p2Prefab)
    {
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager == null)
        {
            gameManager = new GameObject("GameManager");
        }
        
        FighterSpawner spawner = gameManager.GetComponent<FighterSpawner>();
        if (spawner == null)
        {
            spawner = gameManager.AddComponent<FighterSpawner>();
        }

        // Set private fields using SerializedObject
        SerializedObject so = new SerializedObject(spawner);
        so.FindProperty("p1Prefab").objectReferenceValue = p1Prefab;
        so.FindProperty("p2Prefab").objectReferenceValue = p2Prefab;
        so.ApplyModifiedProperties();
    }
}
