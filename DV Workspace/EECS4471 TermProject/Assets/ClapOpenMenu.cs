using UnityEngine;

public class ClapOpenMenu : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public float clapDistanceThreshold = 0.1f;
    public float cooldownTime = 1f;
    public GameObject clapMenu;  // The parent object of your Nova UI (ClapMenu)

    private float cooldownTimer;
    private bool isMenuOpen = false;

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (!isMenuOpen && cooldownTimer <= 0f && HandsAreClose())
        {
            // Open the menu
            OpenMenu();
            cooldownTimer = cooldownTime;
        }
    }

    bool HandsAreClose()
    {
        if (!leftHand || !rightHand) return false;
        float distance = Vector3.Distance(leftHand.position, rightHand.position);
        return distance < clapDistanceThreshold;
    }

    void OpenMenu()
    {
        isMenuOpen = true;
        // For now, just set active. We’ll handle animation in Step 4.
        clapMenu.SetActive(true);
    }
}
