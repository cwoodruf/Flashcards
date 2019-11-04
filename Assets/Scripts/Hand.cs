using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// sources: https://www.youtube.com/watch?v=HnzmnSqE-Bc https://www.youtube.com/watch?v=ryfUXr5yvKw
// Vive Pickup and Drop Object 1 and 2 from VR with Andrew
// Note that the old tutorial behavior does not work with Unity 2.0 interactions

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_NextAction = null;
    public SteamVR_Action_Boolean m_PrevAction = null;
    public SteamVR_Action_Boolean m_RevealAction = null;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;
    public Interactable m_InContact = null;
    public List<Interactable> m_Interactables = new List<Interactable>();

    // This replaced the Start method
    void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        // Down
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + " Trigger down");
            if (m_InContact)
            {
                Drop();
            }
            else
            {
                Pickup();
            }
        }
        /*
         * make interaction based solely on toggling trigger
         * i.e. card is "sticky" 
        // Up
        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + " Trigger up");
            Drop();
        }
        */
        // Use track pad right and left to change image seen
        // Use menu button or track pad north to see answer
        if (m_InContact)
        {
            if (m_NextAction.GetStateDown(m_Pose.inputSource))
            {
                FlashCardRandomChange.ChangeCard(-1);
            }
            if (m_PrevAction.GetStateDown(m_Pose.inputSource))
            {
                FlashCardRandomChange.ChangeCard(1);
            }
            // answer reveal
            if (m_RevealAction.GetStateDown(m_Pose.inputSource))
            {
                FlashCardRandomChange.ShowAnswer();
            }
            if (m_RevealAction.GetStateUp(m_Pose.inputSource))
            {
                FlashCardRandomChange.ShowQuestion();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            m_Interactables.Add(other.gameObject.GetComponent<Interactable>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            m_Interactables.Remove(other.gameObject.GetComponent<Interactable>());
        }
    }

    public void Pickup()
    {
        // get nearest interactable
        m_InContact = GetNearestInteractable();

        // null check
        if (m_InContact == null)
        {
            print("nothing in contact");
            return;
        }

        // already held check
        if (m_InContact.m_ActiveHand != this && m_InContact.m_ActiveHand != null)
        {
            print("dropping");
            m_InContact.m_ActiveHand.Drop();
        }

        // position to our controller
        // m_InContact.transform.position = transform.position;

        // attach to fixed joint
        Rigidbody target = m_InContact.GetComponent<Rigidbody>();
        m_Joint.connectedBody = target;
        print("we are connected to " + m_Joint.connectedBody);

        // set active hand
        m_InContact.m_ActiveHand = this;
        print("active hand is " + m_InContact.m_ActiveHand);
    }

    public void Drop()
    {
        // Null check
        if (m_InContact == null)
        {
            return;
        }
        // Apply velocity
        Rigidbody target = m_Joint.connectedBody;
        // target.velocity = m_Pose.GetVelocity();
        // target.angularVelocity = m_Pose.GetAngularVelocity();

        // Detach
        m_Joint.connectedBody = null;

        // Clear active hand
        m_InContact.m_ActiveHand = null;
        m_InContact = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDist = float.MaxValue;
        float distance = 0.0f;

        foreach(Interactable i in m_Interactables)
        {
            distance = (i.transform.position - transform.position).sqrMagnitude;
            if (distance < minDist)
            {
                minDist = distance;
                nearest = i;
            }
        }
        return nearest;
    }
}
