using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteLoad : MonoBehaviour {


    public Image image;
    public Sprite a;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("atlasNameatlasNameatlasNameatlasNameatlasNameatlasNameatlasNameatlasNameatlasNameatlasNameatlasNameatlasName");
    
    }

    private SpriteAtlas LoadSpriteAtlas(string atlasName)
    {
        if (string.IsNullOrEmpty(atlasName))
            return null;

        SpriteAtlas res;

        res = Resources.Load(atlasName) as SpriteAtlas;
        return res;
    }

    public Sprite LoadSprite(string atlasName, string spriteName)
    {
        SpriteAtlas atlas = LoadSpriteAtlas(atlasName);
        if (atlas != null)
            return atlas.GetSprite(spriteName);
        else
            return null;
    }

}
