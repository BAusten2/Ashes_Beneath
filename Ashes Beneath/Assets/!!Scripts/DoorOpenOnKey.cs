/* 
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorOpenOnKey : MonoBehaviour
{
    public string openBoolName = "Open"; // Animator bool parameter
    Animator _anim;
    bool _alreadyOpened;

    void Awake() => _anim = GetComponent<Animator>();

    void Update()
    {
        if (!_alreadyOpened && PlayerInventory.HasKey1)
        {
            _anim.SetBool(openBoolName, true); // transition to Door_Open
            _alreadyOpened = true;             // never close
        }
    }
}
*/