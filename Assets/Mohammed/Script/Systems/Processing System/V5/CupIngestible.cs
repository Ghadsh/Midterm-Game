using UnityEngine;

public class CupIngestible : MonoBehaviour
{
    public enum Kind { Leaves, Mod }

    public Kind kind = Kind.Leaves;
    public ItemDefinition itemDef;
    public int count = 1;

    [Header("Ingest Delay")]
    public float consumeDelay = 0.35f;   // grace period after spawn
    float _spawnTime;

    void OnEnable() { _spawnTime = Time.time; }

    public bool CanBeConsumed() => Time.time >= _spawnTime + consumeDelay;
}
