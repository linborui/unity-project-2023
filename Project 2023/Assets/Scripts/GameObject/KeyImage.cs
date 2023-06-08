using UnityEngine;
using UnityEngine.UI;
using Luminosity.IO;

public class KeyImage : MonoBehaviour
{
    string textureName = "_MainTex";
    float interval = 0.5f;
    Material keyMaterial;
    float time;

    void Start()
    {
        keyMaterial = GetComponent<Image>().material;
        time = 0f;
    }

    void Update()
    {
        if (InputManager.GetButton("Interact")) {
            pressImage();
            return;
        }
        else if (InputManager.GetButtonUp("Interact")) {
            toggleImage();
            time = Time.time;
            return;
        }

        if (time + interval < Time.time) {
            toggleImage();
            time = Time.time;
        }
    }

    void toggleImage() {
        Vector2 offset = keyMaterial.GetTextureOffset(textureName);
        offset.x = 0.5f - offset.x;
        keyMaterial.SetTextureOffset(textureName, offset);
    }

    void pressImage() {
        Vector2 offset = keyMaterial.GetTextureOffset(textureName);
        offset.x = 0.5f;
        keyMaterial.SetTextureOffset(textureName, offset);
    }
}
