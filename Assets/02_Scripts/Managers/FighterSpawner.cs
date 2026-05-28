using UnityEngine;

public class FighterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject p1Prefab;
    [SerializeField] private GameObject p2Prefab;

    public FighterController P1 { get; private set; }
    public FighterController P2 { get; private set; }

    [ContextMenu("Spawn Fighters")]
    public void SpawnFighters()
    {
        Vector3 p1Pos = new Vector3(-5f, 0f, 0f);
        Quaternion p1Rot = Quaternion.Euler(0f, 90f, 0f);
        GameObject p1GO = Instantiate(p1Prefab, p1Pos, p1Rot);
        P1 = p1GO.GetComponent<FighterController>();
        if (P1 != null && P1.inputReader != null)
        {
            P1.inputReader.playerIndex = PlayerIndex.Player1;
        }

        Vector3 p2Pos = new Vector3(5f, 0f, 0f);
        Quaternion p2Rot = Quaternion.Euler(0f, 270f, 0f);
        GameObject p2GO = Instantiate(p2Prefab, p2Pos, p2Rot);
        P2 = p2GO.GetComponent<FighterController>();
        if (P2 != null && P2.inputReader != null)
        {
            P2.inputReader.playerIndex = PlayerIndex.Player2;
        }
    }
}
