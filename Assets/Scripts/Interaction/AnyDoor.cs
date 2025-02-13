using System.Collections;
using UnityEngine;

public class AnyDoor : Interactable
{
    [SerializeField] private Vector3 localPos, localRot;
    [SerializeField] private bool shouldMove, shouldRot;
    [SerializeField] private float speed = 1f;

    private Vector3 defaultPos, defaultRot;
    private bool isOpen, isOpenning;

    private Outline outline;

    private void Start()
    {
        if (TryGetComponent<Outline>(out outline))
            outline.enabled = false;

        defaultPos = transform.localPosition;
        defaultRot = transform.localRotation.eulerAngles;
    }

    public override void OnFocus()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public override void OnInteract()
    {
        if (isOpenning) return;

        if (isOpen)
            StartCoroutine(OpenDoor(defaultPos, Quaternion.Euler(defaultRot)));
        else
            StartCoroutine(OpenDoor(localPos, Quaternion.Euler(localRot)));
    }

    public override void OnLoseFocus()
    {
        if (outline != null)
            outline.enabled = false;
    }

    private IEnumerator OpenDoor(Vector3 targetPosition, Quaternion targetRotation)
    {
        isOpenning = true;
        isOpen = !isOpen;

        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * speed;

            if (shouldMove)
                transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);

            if (shouldRot)
                transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);

            yield return null;
        }

        if (shouldMove)
            transform.localPosition = targetPosition;

        if (shouldRot)
            transform.localRotation = targetRotation;

        isOpenning = false;
    }
}