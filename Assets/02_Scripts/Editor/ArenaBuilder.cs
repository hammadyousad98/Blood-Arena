using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ArenaBuilder : EditorWindow
{
    [MenuItem("Tools/BloodArena/Build Arena 1 (Dragon Courtyard)")]
    public static void BuildArena1()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildArenaBase();
        
        // ARENA 1 specifics
        SetFloorMaterial(ParseColor("#3D2B1F"));
        
        RenderSettings.ambientLight = ParseColor("#2B1500");
        RenderSettings.fogColor = ParseColor("#4A2800");
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.04f;
        RenderSettings.fog = true;

        CreateDirectionalLight(new Vector3(35, -25, 0), ParseColor("#FF8C42"), 1.0f);
        SetupPostProcessing(0.8f, 0.4f);

        Vector3[] lightPositions = { new Vector3(-9, 2, -5), new Vector3(9, 2, -5), new Vector3(-9, 2, 5), new Vector3(9, 2, 5) };
        foreach (var pos in lightPositions)
        {
            CreatePointLight(pos, ParseColor("#FF8C42"), 4f, 1.5f, LightShadows.None);
        }

        Debug.Log("Arena 1 Built successfully. Please save the scene manually or via Editor tool if needed.");
    }

    [MenuItem("Tools/BloodArena/Build Arena 2 (Cursed Crypt)")]
    public static void BuildArena2()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildArenaBase();
        
        // ARENA 2 specifics
        SetFloorMaterial(ParseColor("#1A2020"));
        
        RenderSettings.ambientLight = ParseColor("#0A0014");
        RenderSettings.fogColor = ParseColor("#0D0020");
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 8f;
        RenderSettings.fogEndDistance = 18f;
        RenderSettings.fog = true;

        CreateDirectionalLight(new Vector3(60, 10, 0), ParseColor("#9B59FF"), 0.5f);
        SetupPostProcessing(1.2f, 0.6f);

        Vector3[] lightPositions = { 
            new Vector3(-8.5f, 1, -4), new Vector3(8.5f, 1, -4), 
            new Vector3(-8.5f, 1, 4), new Vector3(8.5f, 1, 4), 
            new Vector3(-7, 1, -3), new Vector3(7, 1, -3) 
        };
        foreach (var pos in lightPositions)
        {
            GameObject lightObj = CreatePointLight(pos, ParseColor("#7B68EE"), 2.5f, 0.8f, LightShadows.None);
            FlickerLight flicker = lightObj.AddComponent<FlickerLight>();
            flicker.minIntensity = 0.4f;
            flicker.maxIntensity = 1.2f;
        }

        Debug.Log("Arena 2 Built successfully.");
    }

    [MenuItem("Tools/BloodArena/Build Arena 3 (Infernal Forge)")]
    public static void BuildArena3()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildArenaBase();
        
        // ARENA 3 specifics
        SetFloorMaterial(ParseColor("#1A0A00"), ParseColor("#FF4500"), 0.4f);
        
        RenderSettings.ambientLight = ParseColor("#1A0000");
        RenderSettings.fogColor = ParseColor("#3D0000");
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.06f;
        RenderSettings.fog = true;

        CreateDirectionalLight(new Vector3(20, 0, 0), ParseColor("#FF3300"), 1.3f);
        SetupPostProcessing(2.0f, 0.5f);

        Vector3[] lightPositions = { new Vector3(-9, 0.3f, 0), new Vector3(9, 0.3f, 0), new Vector3(0, 0.3f, -6), new Vector3(0, 0.3f, 6) };
        foreach (var pos in lightPositions)
        {
            CreatePointLight(pos, ParseColor("#FF4400"), 5f, 2.0f, LightShadows.None);
        }

        Debug.Log("Arena 3 Built successfully.");
    }

    private static void BuildArenaBase()
    {
        // 1. Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Arena_Floor";
        floor.transform.position = Vector3.zero;
        // Unity plane is 10x10. So scale x=2 -> 20 width, z=1.2 -> 12 depth
        floor.transform.localScale = new Vector3(2f, 1f, 1.2f);

        // 2. Boundary walls
        CreateWall("Wall_North", new Vector3(0, 1.5f, 6), new Vector3(20, 3, 0.2f));
        CreateWall("Wall_South", new Vector3(0, 1.5f, -6), new Vector3(20, 3, 0.2f));
        CreateWall("Wall_East", new Vector3(10, 1.5f, 0), new Vector3(0.2f, 3, 12));
        CreateWall("Wall_West", new Vector3(-10, 1.5f, 0), new Vector3(0.2f, 3, 12));

        // 3. Spawn point markers
        CreateSpawnPoint("SpawnP1", new Vector3(-5, 0, 0), Quaternion.Euler(0, 90, 0));
        CreateSpawnPoint("SpawnP2", new Vector3(5, 0, 0), Quaternion.Euler(0, 270, 0));

        // 4. Main Camera
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.tag = "MainCamera";
        Camera cam = cameraObj.AddComponent<Camera>();
        cameraObj.transform.position = new Vector3(0, 6.0f, -7.0f);
        cameraObj.transform.rotation = Quaternion.Euler(40, 0, 0);
        cam.fieldOfView = 65f;
    }

    private static void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = new GameObject(name);
        wall.transform.position = position;
        
        BoxCollider bc = wall.AddComponent<BoxCollider>();
        bc.size = scale; // Using BoxCollider size instead of scale is often cleaner, but prompt asked for scale.
        wall.transform.localScale = scale;
        bc.size = Vector3.one; // Ensure collider size matches scale.

        int boundaryLayer = LayerMask.NameToLayer("Boundary");
        if (boundaryLayer != -1)
        {
            wall.layer = boundaryLayer;
        }
        else
        {
            Debug.LogWarning("Layer 'Boundary' does not exist! Please create it in Project Settings -> Tags and Layers.");
        }
    }

    private static void CreateSpawnPoint(string name, Vector3 pos, Quaternion rot)
    {
        GameObject spawn = new GameObject(name);
        spawn.transform.position = pos;
        spawn.transform.rotation = rot;
        spawn.tag = "SpawnPoint"; // Ensure this tag exists in Project Settings
    }

    private static void SetFloorMaterial(Color baseColor, Color emColor = default, float emIntensity = 0f)
    {
        GameObject floor = GameObject.Find("Arena_Floor");
        if (floor != null)
        {
            Renderer renderer = floor.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = baseColor;
            
            if (emIntensity > 0)
            {
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                mat.SetColor("_EmissionColor", emColor * emIntensity);
            }
            
            renderer.sharedMaterial = mat;
        }
    }

    private static void CreateDirectionalLight(Vector3 rotation, Color color, float intensity)
    {
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        lightObj.transform.rotation = Quaternion.Euler(rotation);
        light.color = color;
        light.intensity = intensity;
    }

    private static GameObject CreatePointLight(Vector3 pos, Color color, float range, float intensity, LightShadows shadows)
    {
        GameObject lightObj = new GameObject("Point Light");
        lightObj.transform.position = pos;
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.range = range;
        light.intensity = intensity;
        light.shadows = shadows;
        return lightObj;
    }

    private static void SetupPostProcessing(float bloomIntensity, float vignetteIntensity)
    {
        GameObject volumeObj = new GameObject("Global Volume");
        Volume volume = volumeObj.AddComponent<Volume>();
        volume.isGlobal = true;

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = "Arena_VolumeProfile";
        volume.profile = profile;

        if (!profile.Has<Bloom>())
        {
            Bloom bloom = profile.Add<Bloom>(true);
            bloom.intensity.Override(bloomIntensity);
        }

        if (!profile.Has<Vignette>())
        {
            Vignette vignette = profile.Add<Vignette>(true);
            vignette.intensity.Override(vignetteIntensity);
        }
    }

    private static Color ParseColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
            return color;
        return Color.white;
    }
}
