using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_GridScrolling : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    [SerializeField] private float _scrollSpeed = 0.1f;

    private float _xScroll;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        Time.timeScale = 1.0f;
    }


    private void Update()
    {
        Scroll();
    }

    private void Scroll()
    {
        _xScroll = Time.time * _scrollSpeed;
        Vector2 offset = new Vector2(0f, _xScroll);
        _meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
