using UnityEngine;
using Valve.VR;

/**
 * See FlashCardChanger as an example of an implementation of this 
 * The callbacks for the hand controllers are incorporated in Hand.cs
 * which handles finding a nearby object and grabbing it via the GrabDelegate.
 * The DropDelegate handles detaching from the current hand controller.
 */
public abstract class AbstractHandListener: MonoBehaviour
{
    // see Hand.cs for implementations of these
    public delegate Interactable GrabDelegate(); 
    public delegate Interactable DropDelegate(Interactable I); 

    public abstract void HandListener(
        SteamVR_Behaviour_Pose p,
        DropDelegate d,
        GrabDelegate g
    );
}