using UnityEngine;
using TheClimb.Player;

namespace TheClimb.Astral
{
    [RequireComponent(typeof(PlanetController))]
    [DisallowMultipleComponent]
    public class PlanetBootstrap : MonoBehaviour    //  後に改善予定
    {
        [SerializeField] PlanetController controller;
        [SerializeField] StageClear stageClear;

        PlayerDataProvider playerDataProvider;
        [SerializeField] Transform playerTransform;
        [SerializeField] Rigidbody playerRigidBody;

        //  --  UnityLifeCycle

        void Awake()
        {
            if(controller == null)
            {
                controller = GetComponent<PlanetController>();
                Debug.LogWarning("playerController isn't assigned in the Inspector");
            }

            if (playerTransform == null)
            {
                Debug.LogWarning("playerTransform isn't assigned in the Inspector");
            }

            if (playerRigidBody == null)
            {
                Debug.LogWarning("playerRigidBody isn't assigned in the Inspector");
            }
            playerDataProvider = new PlayerDataProvider(playerTransform, playerRigidBody);
        }
        
        private void Start()
        {
            controller.Initialize(playerDataProvider, stageClear);
        }
    }
}