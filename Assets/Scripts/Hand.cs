using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// sources: https://www.youtube.com/watch?v=HnzmnSqE-Bc https://www.youtube.com/watch?v=ryfUXr5yvKw
// Vive Pickup and Drop Object 1 and 2 from VR with Andrew
// Note that the old tutorial behavior does not work with Unity 2.0 interactions
// 2019-11-17: updated so that we can control various types of game objects via listeners

public class Hand : MonoBehaviour
{
    public AbstractHandListener[] m_Listeners;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;
    private List<Interactable> m_Interactables = new List<Interactable>();

    private void Start()
    {
        m_Listeners = FindObjectsOfType<AbstractHandListener>();
    }

    void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (AbstractHandListener listener in m_Listeners) {
            listener.HandListener(m_Pose, Drop, Pickup);
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

    public Interactable Pickup()
    {
        // get nearest interactable
        Interactable m_InContact = GetNearestInteractable();

        // null check
        if (m_InContact == null)
        {
            print("nothing in contact");
            return null;
        }

        // already held check
        if (m_InContact.m_ActiveHand != this && m_InContact.m_ActiveHand != null)
        {
            print("dropping");
            m_InContact.m_ActiveHand.Drop(m_InContact);
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

        return m_InContact;
    }

    public Interactable Drop(Interactable m_InContact)
    {
        if (m_InContact == null)
        {
            return null;
        }
        // Detach
        m_Joint.connectedBody = null;

        // Clear active hand
        m_InContact.m_ActiveHand = null;
        m_InContact = null;

        return m_InContact;
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
