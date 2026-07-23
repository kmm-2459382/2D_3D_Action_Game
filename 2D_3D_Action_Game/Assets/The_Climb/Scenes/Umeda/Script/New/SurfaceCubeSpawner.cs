using UnityEngine;

public class SurfaceCubeSpawner : MonoBehaviour
{
    [Header("生成するCubeのPrefab")]
    public GameObject cubePrefab;

    [Header("Groundレイヤー")]
    public LayerMask groundLayer;

    private GameObject cubeFloor;
    private GameObject cubeCeiling;
    private GameObject cubeLeft;
    private GameObject cubeRight;

    void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) == 0) return;

        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            // 床（上向き）
            if (Vector3.Dot(normal, Vector3.up) > 0.5f)
            {
                if (cubeFloor == null && cubePrefab != null)
                {
                    cubeFloor = Instantiate(cubePrefab, contact.point, Quaternion.identity);
                    cubeFloor.transform.SetParent(transform);
                }
            }
            // 天井（下向き）
            else if (Vector3.Dot(normal, Vector3.down) > 0.5f)
            {
                if (cubeCeiling == null && cubePrefab != null)
                {
                    cubeCeiling = Instantiate(cubePrefab, contact.point, Quaternion.identity);
                    cubeCeiling.transform.SetParent(transform);
                }
            }
            // 右（法線が左向き → 接触面が右）
            else if (Vector3.Dot(normal, Vector3.left) > 0.5f)
            {
                if (cubeRight == null && cubePrefab != null)
                {
                    cubeRight = Instantiate(cubePrefab, contact.point, Quaternion.identity);
                    cubeRight.transform.SetParent(transform);
                }
            }
            // 左（法線が右向き → 接触面が左）
            else if (Vector3.Dot(normal, Vector3.right) > 0.5f)
            {
                if (cubeLeft == null && cubePrefab != null)
                {
                    cubeLeft = Instantiate(cubePrefab, contact.point, Quaternion.identity);
                    cubeLeft.transform.SetParent(transform);
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) == 0) return;

        // 接触がなくなったら削除
        DestroyIfExists(ref cubeFloor);
        DestroyIfExists(ref cubeCeiling);
        DestroyIfExists(ref cubeLeft);
        DestroyIfExists(ref cubeRight);
    }

    private void DestroyIfExists(ref GameObject cube)
    {
        if (cube != null)
        {
            Destroy(cube);
            cube = null;
        }
    }
}
