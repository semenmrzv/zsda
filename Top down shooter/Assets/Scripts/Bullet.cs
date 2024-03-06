
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // ������, ������� ����� ���������� ��� ��������� ����
    [SerializeField] private GameObject _hitPrefab;

    // �������� ����
    [SerializeField] private float _speed = 30f;

    // ����� ����������� ���� �� ������
    [SerializeField] private float _lifeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // ��������� ����� ����������� ���� �� ������
        ReduceLifeTime();

        // ��������� ��������� � ������
        CheckHit();

        // ���������� ����
        Move();
    }

    private void ReduceLifeTime()
    {
        // ��������� ����� ����������� ���� �� �����, ��������� � ���������� �����
        _lifeTime -= Time.deltaTime;

        // ���� ����� ����������� ���� �������
        if (_lifeTime <= 0)
        {
            // ���� ��������� � ������
            DestroyBullet();
        }
    }

    private void CheckHit()
    {
        // ���� ���� ����������� � ���-��
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _speed * Time.deltaTime))
        {
            // ������������ ���������
            Hit(hit);
        }
    }

    private void Move()
    {
        // ������ ������� ���� ����� ��������� �������� � �������
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void Hit(RaycastHit hit)
    {
        // ������ ������ ��������� �� ����� ������������ ����
        Instantiate(_hitPrefab, hit.point, Quaternion.LookRotation(-transform.up, -transform.forward));

        // ���� ��������� � ������
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        // ������� ������ ����
        Destroy(gameObject);
    }
}

