using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* 
 * Author: Cal cwoodruf@sfu.ca (or gmail.com)
 * refs: 
 * https://docs.unity3d.com/ScriptReference/Resources.LoadAll.html
 * https://answers.unity.com/questions/254798/check-an-objects-rotation.html
 * https://docs.unity3d.com/Manual/ControllingGameObjectsComponents.html
 * https://docs.unity3d.com/ScriptReference/Material.SetTextureScale.html
 */

public class FlashCardRandomChange : MonoBehaviour
{
    public float PeakAngle;
    public string FlashCardStack;
    private GameObject floor;
    private static Texture[] textures;
    private static int texture;
    private static Rigidbody card;
    private bool tipped = false;
    private static Material mat;
    // this was being used to change the state of the card
    // but it seemed easier to use buttons to control static methods instead
    private float tipAngle = 0.0f;

    void Awake()
    {
        card = GetComponent<Rigidbody>();
        mat = card.GetComponent<Renderer>().material;
        floor = GameObject.FindGameObjectWithTag("Floor");
        tipped = false;
        tipAngle = Vector3.Angle(floor.transform.up, card.transform.up);
        textures = Resources.LoadAll("flashcards/images/"+FlashCardStack, typeof(Texture2D)).Cast<Texture2D>().ToArray();
        texture = 0;
        foreach (var t in textures)
            Debug.Log(t.name);
    }

    // make card use new image based on outside input
    public static void ChangeCard(int increment)
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

    public static void ShowAnswer()
    {
        var mat = card.GetComponent<Renderer>().material;
        mat.SetTextureOffset("_MainTex", new Vector2(0.5f, 0f));
        mat.mainTexture = textures[texture];
    }

    public static void ShowQuestion()
    {
        var mat = card.GetComponent<Renderer>().material;
        mat.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
        mat.mainTexture = textures[texture];
    }
}
