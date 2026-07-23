using UnityEngine;

[ExecuteAlways]
public class CharacterStateVisualizer : MonoBehaviour
{
    [SerializeField] Color MoveDirColor = Color.green;
    [SerializeField] Vector3 MoveDirection = Vector3.right;
    [SerializeField] float LineLength = 2f;

    public Vector3 MoveDirectionPropety(Vector3 _MoveDirection) => MoveDirection = _MoveDirection;

    void OnDrawGizmos()
    {
        Gizmos.color = MoveDirColor;
        Gizmos.DrawLine(transform.position, transform.position + MoveDirection.normalized * LineLength);
    }
}