using System.Linq;
using UnityEngine;
using Valve.VR;

/* 
 * Author: Cal cwoodruf@sfu.ca (or gmail.com)
 * refs: 
 * https://docs.unity3d.com/ScriptReference/Resources.LoadAll.html
 * https://answers.unity.com/questions/254798/check-an-objects-rotation.html
 * https://docs.unity3d.com/Manual/ControllingGameObjectsComponents.html
 * https://docs.unity3d.com/ScriptReference/Material.SetTextureScale.html
 * 
 * This handles basic manipulations of flashcards. 
 * Flashcards are really just a series of images. 
 * The left side of the image is the "term"
 * and the right side of the image is the "definition".
 * 
 * There are probably different ways to implement interactions that would work.
 * In the initial test these were:
 * m_GrabAction -> \actions\default\in\GrabPinch (trigger)
 * m_NextAction -> \actions\default\in\SnapTurnLeft (click right trackpad)
 * m_PrevAction -> \actions\default\in\SnapTurnRight (click left trackpad)
 * m_RevealAction -> \actions\default\int\GrabGrip (squeeze controller handle)
 * 
 * The grab action is sticky to make it easier to do other actions.
 * The reveal action simply moves the starting pixel for the image display.
 * By default we only ever show half of the image.
 * 
 * Created the images from examples from a website called flashcardmachine.com.
 * Used imagemagick scripts to split images of the flashcards grabbed with 
 * firefox's *** > take a screenshot widget. This is ok for testing but won't
 * work if we are to produce large numbers and types of cards.
 * 
 * We would also probably want a more sophisticated interface for jumping ahead
 * and going back. For these simply define different actions. Doing this in a more 
 * ExNovo way may require some work.
 */

public class FlashCardChanger : AbstractHandListener
{
    public string FlashCardStack;
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_NextAction = null;
    public SteamVR_Action_Boolean m_PrevAction = null;
    public SteamVR_Action_Boolean m_RevealAction = null;

    private Interactable m_InContact = null;
    private Texture[] textures;
    private int texture;
    private Rigidbody card;
    private Material mat;

    void Awake()
    {
        card = GetComponent<Rigidbody>();
        mat = card.GetComponent<Renderer>().material;
        textures = Resources.LoadAll("flashcards/images/" + FlashCardStack, typeof(Texture2D)).Cast<Texture2D>().ToArray();
        texture = 0;
        foreach (var t in textures)
            Debug.Log(t.name);
    }

    public override void HandListener(
            SteamVR_Behaviour_Pose m_Pose, 
            DropDelegate Drop,
            GrabDelegate Pickup
    ) {
        // Down
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            // print(m_Pose.inputSource + " Trigger down");
            if (m_InContact)
            {
                m_InContact = Drop(m_InContact);
            }
            else
            {
                m_InContact = Pickup();
            }
        }

        // Use track pad right and left to change image seen
        // Use menu button or track pad north to see answer
        if (m_InContact)
        {
            if (m_NextAction.GetStateDown(m_Pose.inputSource))
            {
                ChangeCard(-1);
            }
            if (m_PrevAction.GetStateDown(m_Pose.inputSource))
            {
                ChangeCard(1);
            }
            // answer reveal
            if (m_RevealAction.GetStateDown(m_Pose.inputSource))
            {
                ShowAnswer();
            }
            if (m_RevealAction.GetStateUp(m_Pose.inputSource))
            {
                ShowQuestion();
            }
        }
    }

    // make card use new image based on outside input
    public void ChangeCard(int increment)
    {
        texture += increment;
        if (texture > textures.Length - 1)
        {
            texture = 0;
        }
        else if (texture < 0)
        {
            texture = textures.Length - 1;
        }
        mat.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
        mat.mainTexture = textures[texture];
    }

    public void ShowAnswer()
    {
        var mat = card.GetComponent<Renderer>().material;
        mat.SetTextureOffset("_MainTex", new Vector2(0.5f, 0f));
        mat.mainTexture = textures[texture];
    }

    public void ShowQuestion()
    {
        var mat = card.GetComponent<Renderer>().material;
        mat.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
        mat.mainTexture = textures[texture];
    }
}
