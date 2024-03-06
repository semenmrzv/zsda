using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : Character
{

    // Трансформа цели игрока
    private Transform _aimTransform;

    // Переменная для анимаций
    // Специального типа RigBuilder
    private RigBuilder _rigBuilder;

    // Массив объектов типа WeaponAiming
    private WeaponAiming[] _weaponAimings;
    // Константа с ключом горизонтального движения
    // Будем использовать её для получения данных ввода с клавиатуры
    private const string MovementHorizontalKey = "Horizontal";


    // Константа с ключом вертикального движения
    // Будем использовать её для получения данных ввода с клавиатуры
    private const string MovementVerticalKey = "Vertical";

    // Константа с ключом приземления
    // Будем использовать её для выставления в аниматоре
    private const string IsGroundedKey = "IsGrounded";

    // Множитель гравитации
    [SerializeField] private float _gravityMultiplier = 2f;

    // Скорость движения
    [SerializeField] private float _movementSpeed = 6f;

    // Скорость прыжка
    [SerializeField] private float _jumpSpeed = 30f;

    // Длительность прыжка
    [SerializeField] private float _jumpDuration = 1f;

    // Расстояние для приземления
    [SerializeField] private float _groundCheckDistance = 0.2f;

    // Дополнительная высота проверки земли
    [SerializeField] private float _groundCheckExtraUp = 0.2f;

    // Скорость вращения
    [SerializeField] private float _aimingSpeed = 10f;

    // Анимация героя
    private Animator _animator;

    // Контроллер движения
    private CharacterController _characterController;

    // Главная камера
    private Camera _mainCamera;

    // Размеры коллайдера для проверки земли
    private Vector3 _groundCheckBox;

    // Флаг того, что герой на земле
    private bool _isGrounded;

    // Флаг того, что герой в прыжке
    private bool _isJumping;

    // Таймер длительности прыжка
    private float _jumpTimer;

    private void Start()
    {
        // Вызываем метод Init()
        Init();
    }

    private void Init()
    {
        _animator = GetComponentInChildren<Animator>();
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;

        // НОВОЕ: Находим объект типа PlayerAim
        // Записываем его трансформу в _aimTransform
        _aimTransform = FindAnyObjectByType<PlayerAim>().transform;

        // НОВОЕ: Получаем RigBuilder из дочерних объектов
        // Записываем его в переменную _rigBuilder
        _rigBuilder = GetComponentInChildren<RigBuilder>();

        // НОВОЕ: Получаем все компоненты WeaponAiming
        // Записываем их в массив _weaponAimings
        _weaponAimings = GetComponentsInChildren<WeaponAiming>(true);

        _groundCheckBox = new Vector3(_characterController.radius, 0.0001f, _characterController.radius);

        // НОВОЕ: Вызываем метод InitWeaponAimings()
        // Передаём туда _weaponAimings и _aimTransform
        InitWeaponAimings(_weaponAimings, _aimTransform);
    }

    private void InitWeaponAimings(WeaponAiming[] weaponAimings, Transform aim)
    {
        // Проходим по всем элементам weaponAimings
        for (int i = 0; i < weaponAimings.Length; i++)
        {
            // Вызываем у weaponAimings[i] метод Init()
            // И передаём ему компонент цели aim
            weaponAimings[i].Init(aim);
        }
        // Вызываем у _rigBuilder встроенный метод Build()
        // Чтобы построить скелетную анимацию героя
        _rigBuilder.Build();
    }

    private void FixedUpdate()
    {
        // Применяем к герою гравитацию
        Gravity();

        // Двигаем героя клавишами
        Movement();

        // Управляем прыжком героя
        Jumping();

        // Поворачиваем героя для прицела
        Aiming();
    }

    private void Gravity()
    {
        // Создаём переменную gravity типа Vector3
        // Присваиваем ей значение силы гравитации из физического движка Physics
        Vector3 gravity = Physics.gravity;

        // Умножаем гравитацию на множитель и время кадра
        gravity *= _gravityMultiplier * Time.fixedDeltaTime;

        // Придаём гравитацию компоненту CharacterController
        _characterController.Move(gravity);
    }

    private void Movement()
    {
        // Создаём переменную movement со значением (0, 0, 0)
        Vector3 movement = Vector3.zero;

        // Задаём movement.x значение горизонтального ввода с клавиатуры (клавиши A и D)
        movement.x = Input.GetAxis(MovementHorizontalKey);

        // Задаём movement.z значение вертикального ввода с клавиатуры (клавиши W и S)
        movement.z = Input.GetAxis(MovementVerticalKey);

        // Преобразуем вектор перемещения относительно камеры
        movement = GetMovementByCamera(movement);

        // Вычисляем вектор перемещения
        // Через скорость и время между фиксированными кадрами
        movement *= _movementSpeed * Time.fixedDeltaTime;

        // Придаём движение компоненту CharacterController
        _characterController.Move(movement);

        // Анимируем движение героя
        AnimateMovement(movement);
    }

    private Vector3 GetMovementByCamera(Vector3 input)
    {
        // Получаем вектор направления камеры вперёд
        Vector3 cameraForward = _mainCamera.transform.forward;

        // Получаем вектор направления камеры вправо
        Vector3 cameraRight = _mainCamera.transform.right;

        // Обнуляем значение вектора направления вперёд
        cameraForward.y = 0f;

        // Обнуляем значение вектора направления вправо
        cameraRight.y = 0f;

        // Нормализуем вектор направления вперёд
        cameraForward.Normalize();

        // Нормализуем вектор направления вправо
        cameraRight.Normalize();

        // Складываем векторы движения с учётом направления камеры
        Vector3 movement = cameraForward * input.z + cameraRight * input.x;

        // Возвращаем полученный вектор движения
        return movement;
    }

    private void AnimateMovement(Vector3 movement)
    {
        // Получаем проекцию вектора движения на ось X
        float relatedX = Vector3.Dot(movement.normalized, transform.right);

        // Получаем проекцию вектора движения на ось Y
        float relatedY = Vector3.Dot(movement.normalized, transform.forward);

        // Устанавливаем значение анимации горизонтального движения
        _animator.SetFloat(MovementHorizontalKey, relatedX);

        // Устанавливаем значение анимации вертикального движения
        _animator.SetFloat(MovementVerticalKey, relatedY);
    }

    private void Jumping()
    {
        // Обновляем данные о приземлении
        RefreshIsGrounded();

        // Если нажата клавиша «пробел»
        // И герой находится на земле
        // И прыжок сейчас не выполняется
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded && !_isJumping)
        {
            // Убираем состояние «на земле»
            SetIsGrounded(false);

            // Ставим флаг «в прыжке»
            _isJumping = true;

            // Обнуляем таймер прыжка
            _jumpTimer = 0;
        }
        // Если герой в прыжке
        if (_isJumping)
        {
            // Увеличиваем таймер прыжка
            _jumpTimer += Time.fixedDeltaTime;

            // Рассчитываем вертикальную силу движения вверх с учётом длительности прыжка
            Vector3 motion = Vector3.up * _jumpSpeed * (1 - _jumpTimer / _jumpDuration) * Time.fixedDeltaTime;

            // Применяем силу через Character Controller
            _characterController.Move(motion);

            // Если длительность прыжка превышена
            // Или если герой приземлился
            if (_jumpTimer >= _jumpDuration || _isGrounded)
            {
                // Убираем флаг «в прыжке»
                _isJumping = false;
            }
        }
    }

    private void RefreshIsGrounded()
    {
        // Устанавливаем состояние приземления через GroundCheck()
        SetIsGrounded(GroundCheck());
    }

    private bool GroundCheck()
    {
        // Вычисляем позицию начального положения для проверки земли
        Vector3 startCheckPosition = transform.position + Vector3.up * _groundCheckExtraUp;

        // Вычисляем длину луча для проверки земли
        float checkDistance = _groundCheckDistance + _groundCheckExtraUp;

        // Возвращаем результат с расстоянием от начальной позиции до точки ниже героя
        return Physics.BoxCast(startCheckPosition, _groundCheckBox, Vector3.down, transform.rotation, checkDistance);
    }

    private void SetIsGrounded(bool value)
    {
        // Если состояние героя изменяется
        // От «на земле» к «в прыжке» или наоборот
        if (value != _isGrounded)
        {
            // Обновляем значение в аниматоре
            _animator.SetBool(IsGroundedKey, value);
        }
        // Присваиваем флагу «на земле» переданное значение
        _isGrounded = value;
    }

    private void Aiming()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Ray findTargetRay = _mainCamera.ScreenPointToRay(mouseScreenPosition);

        if (Physics.Raycast(findTargetRay, out RaycastHit hitInfo))
        {
            Vector3 lookDirection = (hitInfo.point - transform.position).normalized;
            lookDirection.y = 0;
            var newRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, _aimingSpeed * Time.fixedDeltaTime);

            // НОВОЕ: Плавно перемещаем цель игрока
            // В точку столкновения с заданной скоростью
            _aimTransform.position = Vector3.Lerp(_aimTransform.position, hitInfo.point, _aimingSpeed * Time.fixedDeltaTime);
        }
    }


}
