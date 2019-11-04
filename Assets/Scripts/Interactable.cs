using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sources: https://www.youtube.com/watch?v=HnzmnSqE-Bc https://www.youtube.com/watch?v=ryfUXr5yvKw
// Vive Pickup and Drop Object from VR with Andrew


[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public Hand m_ActiveHand = null;
}
