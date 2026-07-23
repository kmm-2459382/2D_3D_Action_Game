using UnityEngine;

public class FlashlightRevealer : MonoBehaviour
{
    public float revealDistance = 10f; // 光の届く距離
    public LayerMask revealLayer;      // 光で反応するレイヤー

    void Update()
    {
        // プレイヤーの前方向にRayを飛ばす
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, revealDistance, revealLayer))
        {
            RevealByLight reveal = hit.collider.GetComponent<RevealByLight>();
            if (reveal != null)
            {
                reveal.Reveal();
            }
        }
    }
}