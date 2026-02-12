using UnityEngine;
using System.Collections;

public class SmoothAppearance : MonoBehaviour
{
    public float targetY;
    public float startYOffset = -2f;
    public float smoothSpeed = 5f;

    private void Awake()
    {
        targetY = transform.localPosition.y;
    }

    private void OnEnable()
    {
        // Коли об'єкт вмикається оптимізатором, він починає рух знизу вгору
        Vector3 pos = transform.localPosition;
        pos.y = targetY + startYOffset;
        transform.localPosition = pos;

        StartCoroutine(AppearRoutine());
    }

    IEnumerator AppearRoutine()
    {
        while (Mathf.Abs(transform.localPosition.y - targetY) > 0.01f)
        {
            Vector3 pos = transform.localPosition;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * smoothSpeed);
            transform.localPosition = pos;
            yield return null;
        }

        Vector3 finalPos = transform.localPosition;
        finalPos.y = targetY;
        transform.localPosition = finalPos;
    }
}