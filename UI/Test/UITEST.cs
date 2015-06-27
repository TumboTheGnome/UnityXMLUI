using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BrainBit.UI;

[RequireComponent(typeof(UIAssets))]
public class UITEST : MonoBehaviour
{
    public TextAsset XML;
    private UI _ui;

    void Start()
    {
        this._ui = new UI(new DefaultRuleBuilder());
        this._ui.Load(this.XML.text);
        this._ui.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        this._ui.Canvas.worldCamera = Camera.main;

    }
}
