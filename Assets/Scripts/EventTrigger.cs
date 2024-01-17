using UnityEngine;

public class Event_Trigger : MonoBehaviour
{
    private Camera playerCamera;
    public LayerMask prefabLayer; // Set this layer mask to the layer your prefabs are on

    private GameObject lastHitPrefab; // To keep track of the last prefab that was hit

    private void Start()
    {
        playerCamera = Camera.main; // Assumes the main camera is used, adjust if needed
    }

    private void Update()
    {
        RaycastHit hit;

        // Raycast from the camera to detect objects on the specified layer
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, Mathf.Infinity, prefabLayer))
        {
            Debug.Log(hit.ToString());
            GameObject newHitPrefab = hit.collider.gameObject;

            if (newHitPrefab != lastHitPrefab)
            {
                DeactivateLastPrefab();
                ActivatePrefab(newHitPrefab);
                lastHitPrefab = newHitPrefab;
            }
        }
        else
        {
            DeactivateLastPrefab(); // If no prefab is hit, deactivate the last one
            lastHitPrefab = null;
        }
    }

    private void ActivatePrefab(GameObject prefab)
    {

        if (prefab.tag != "TakeOff")
        {

            // Play the animation on the activated prefab
            Animator animator = prefab.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("go"); // Replace "YourAnimationTrigger" with your actual animation trigger name
            }
        }
        else
        {
            prefab.GetComponent<Takeoff>().isTakingOff = true;
        }
    }

    private void DeactivateLastPrefab()
    {
        // Deactivate the animation on the last prefab
        if (lastHitPrefab != null)
        {
            


            Animator lastHitAnimator = lastHitPrefab.GetComponent<Animator>();
            if (lastHitAnimator != null)
            {
                lastHitAnimator.ResetTrigger("go");
            }
        }
    }
}
