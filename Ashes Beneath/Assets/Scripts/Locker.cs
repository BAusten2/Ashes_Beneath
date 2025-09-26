// Import Libraries
using UnityEngine;

public class Locker : MonoBehaviour
{
    // Where the player snaps to when hiding
    public Transform entryPoint;
    // Where the enemy stands to attack
    public Transform attackPoint;

    private PlayerController occupant;

    public bool TryEnter(PlayerController p)
    {
        if (occupant != null) return false;
        occupant = p;

        // Snap player into locker
        if (entryPoint)
        {
            p.transform.SetPositionAndRotation(entryPoint.position, entryPoint.rotation);
        }

        // Disable movement while hidden
        var cc = p.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        return true;
    }

    public bool TryExit(PlayerController p)
    {
        if (occupant != p) return false;

        var cc = p.GetComponent<CharacterController>();
        if (cc) cc.enabled = true;

        occupant = null;
        return true;
    }

    // Called by Antagonist when it reaches the locker (On LoS) !DEPRECIATED!
    public void Attack()
    {
        Debug.Log("Locker TODO: damage / game over.");
    }
}
