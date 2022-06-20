
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public float speed;
    public float health;
    public float damage;
    public float defense = 0;
    public int scoreYield;
}
