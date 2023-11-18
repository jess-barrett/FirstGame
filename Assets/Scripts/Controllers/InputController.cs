using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController : ScriptableObject
{
    public abstract bool RetrieveLeftInput();
    public abstract bool RetrieveRightInput();
    public abstract bool RetrieveJumpInput();
}
