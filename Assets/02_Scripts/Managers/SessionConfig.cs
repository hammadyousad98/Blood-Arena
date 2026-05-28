using UnityEngine;

[CreateAssetMenu(fileName = "NewSessionConfig", menuName = "BloodArena/Session Config")]
public class SessionConfig : ScriptableObject
{
    public int totalRounds = 3;
    public float roundDuration = 120f;
    public int parryWindowMin = 6;
    public int parryWindowMax = 8;
    public float suddenDeathTimeout = 30f;
}
