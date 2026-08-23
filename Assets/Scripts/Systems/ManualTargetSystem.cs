using UnityEngine;

public class ManualTargetSystem : Singleton<ManualTargetSystem>
{
    [SerializeField] private ArrowView arrowView;
    [SerializeField] private LayerMask targetLayerMask;
    public void StartTargetting(Vector3 startPosition)
    {
        arrowView.gameObject.SetActive(true);
        arrowView.SetupArrow(startPosition);
    }
    public EnemyView EndTargetting(Vector3 endPosition)
    {
        arrowView.gameObject.SetActive(false);
        if (Physics.Raycast(endPosition, Vector3.forward, out RaycastHit hit, 10f, targetLayerMask) && hit.collider!= null && hit.collider.TryGetComponent(out EnemyView enemyView)) { return enemyView; }

        return null;
    }
}
