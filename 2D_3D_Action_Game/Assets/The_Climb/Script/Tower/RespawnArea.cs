using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnArea : MonoBehaviour
{
    public Vector3 respawnPosition = Vector3.zero;
    private Transform respawnObject;
    private Vector3 rayStart = Vector3.zero;

    void Start()
    {
        //if (respawnPosition == new Vector3(0, 0, 0))
        //{
        //    Debug.LogError(this.gameObject + "のリスポーン位置が設定されてません。");
        //}

        rayStart = new Vector3(GetComponent<BoxCollider>().bounds.min.x, GetComponent<BoxCollider>().bounds.max.y, 0f);
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.up, out hit, 5f, GameLayer.ToMask(GameLayers.GROUND)))
        {
            do
            {
                respawnObject = hit.collider.transform;

                rayStart = new Vector3(respawnObject.transform.position.x, respawnObject.GetComponent<BoxCollider>().bounds.max.y, 0f);
            } while (Physics.Raycast(rayStart, Vector3.up, out hit, 1f, GameLayer.ToMask(GameLayers.GROUND)));

            Debug.Log(respawnObject.name);
        }
        else
        {
            Debug.LogWarning(this.gameObject.name + "のリスポーン位置見つかんない");
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //SceneManager.LoadScene("");

        }
    }
}
