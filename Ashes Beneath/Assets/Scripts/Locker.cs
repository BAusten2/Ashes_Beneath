using UnityEngine;

public class Locker : MonoBehaviour
{
    // Where the player snaps to when hiding (optional)
    public Transform entryPoint;
    // Where the enemy stands to attack (optional)
    public Transform attackPoint;

    private PlayerController occupant;

    public bool TryEnter(PlayerController p)
    {
        if (occupant != null) return false;
        occupant = p;

        // snap player into locker
        if (entryPoint)
        {
            p.transform.SetPositionAndRotation(entryPoint.position, entryPoint.rotation);
        }

        // disable movement while hidden
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

    // Called by AntagonistAI when it reaches the locker after seeing the player enter
    public void Attack()
    {
        Debug.Log("Locker attacked! TODO: kill player / apply damage / game over.");
    }
}
