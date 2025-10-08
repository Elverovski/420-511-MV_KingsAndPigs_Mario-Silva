
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyDamageOnContact : MonoBehaviour
{
    public GameObject Player;
    public string playerTag = "Player";

    public int damage = 1;


    // Délai minimal entre deux coups sur le même joueur (en secondes).
    // Évite d'infliger plusieurs hits dans le même instant lors d'entrées/sorties rapides.

    public float hitCooldown = 0.9f;  // évite le spam


    float lastHitTime = -999f;

    void Reset()
    {

        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag(playerTag)) return;

        var hp = root.GetComponent<PlayerHealth>();
        if (!hp) return;


        if (Time.time - lastHitTime < hitCooldown) return;

        hp.TakeDamage(damage);


    
        lastHitTime = Time.time;
    }

}