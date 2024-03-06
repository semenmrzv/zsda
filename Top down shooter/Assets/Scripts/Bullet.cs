
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Префаб, который будет появляться при попадании пули
    [SerializeField] private GameObject _hitPrefab;

    // Скорость пули
    [SerializeField] private float _speed = 30f;

    // Время отображения пули на экране
    [SerializeField] private float _lifeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // Уменьшаем время отображения пули на экране
        ReduceLifeTime();

        // Проверяем попадание в объект
        CheckHit();

        // Перемещаем пулю
        Move();
    }

    private void ReduceLifeTime()
    {
        // Сокращаем время отображения пули на время, прошедшее с последнего кадра
        _lifeTime -= Time.deltaTime;

        // Если время отображения пули истекло
        if (_lifeTime <= 0)
        {
            // Пуля пропадает с экрана
            DestroyBullet();
        }
    }

    private void CheckHit()
    {
        // Если пуля столкнулась с чем-то
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _speed * Time.deltaTime))
        {
            // Обрабатываем попадание
            Hit(hit);
        }
    }

    private void Move()
    {
        // Меняем позицию пули через изменения скорости и времени
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void Hit(RaycastHit hit)
    {
        // Создаём эффект попадания на месте столкновения пули
        Instantiate(_hitPrefab, hit.point, Quaternion.LookRotation(-transform.up, -transform.forward));

        // Пуля пропадает с экрана
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        // Убираем объект пули
        Destroy(gameObject);
    }
}

