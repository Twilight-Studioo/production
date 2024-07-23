using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public Transform player;

    private EnemyPresenter _presenter;

    private void Awake()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _presenter?.StopChasing();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _presenter?.StartChasing();
        }
    }

    public void SetPresenter(EnemyPresenter presenter)
    {
        _presenter = presenter;
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 GetCurrentPosition()
    {
        return transform.position;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.position;
    }
}
