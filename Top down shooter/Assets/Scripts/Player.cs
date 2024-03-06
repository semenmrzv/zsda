using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : Character
{

    // ���������� ���� ������
    private Transform _aimTransform;

    // ���������� ��� ��������
    // ������������ ���� RigBuilder
    private RigBuilder _rigBuilder;

    // ������ �������� ���� WeaponAiming
    private WeaponAiming[] _weaponAimings;
    // ��������� � ������ ��������������� ��������
    // ����� ������������ � ��� ��������� ������ ����� � ����������
    private const string MovementHorizontalKey = "Horizontal";


    // ��������� � ������ ������������� ��������
    // ����� ������������ � ��� ��������� ������ ����� � ����������
    private const string MovementVerticalKey = "Vertical";

    // ��������� � ������ �����������
    // ����� ������������ � ��� ����������� � ���������
    private const string IsGroundedKey = "IsGrounded";

    // ��������� ����������
    [SerializeField] private float _gravityMultiplier = 2f;

    // �������� ��������
    [SerializeField] private float _movementSpeed = 6f;

    // �������� ������
    [SerializeField] private float _jumpSpeed = 30f;

    // ������������ ������
    [SerializeField] private float _jumpDuration = 1f;

    // ���������� ��� �����������
    [SerializeField] private float _groundCheckDistance = 0.2f;

    // �������������� ������ �������� �����
    [SerializeField] private float _groundCheckExtraUp = 0.2f;

    // �������� ��������
    [SerializeField] private float _aimingSpeed = 10f;

    // �������� �����
    private Animator _animator;

    // ���������� ��������
    private CharacterController _characterController;

    // ������� ������
    private Camera _mainCamera;

    // ������� ���������� ��� �������� �����
    private Vector3 _groundCheckBox;

    // ���� ����, ��� ����� �� �����
    private bool _isGrounded;

    // ���� ����, ��� ����� � ������
    private bool _isJumping;

    // ������ ������������ ������
    private float _jumpTimer;

    private void Start()
    {
        // �������� ����� Init()
        Init();
    }

    private void Init()
    {
        _animator = GetComponentInChildren<Animator>();
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;

        // �����: ������� ������ ���� PlayerAim
        // ���������� ��� ���������� � _aimTransform
        _aimTransform = FindAnyObjectByType<PlayerAim>().transform;

        // �����: �������� RigBuilder �� �������� ��������
        // ���������� ��� � ���������� _rigBuilder
        _rigBuilder = GetComponentInChildren<RigBuilder>();

        // �����: �������� ��� ���������� WeaponAiming
        // ���������� �� � ������ _weaponAimings
        _weaponAimings = GetComponentsInChildren<WeaponAiming>(true);

        _groundCheckBox = new Vector3(_characterController.radius, 0.0001f, _characterController.radius);

        // �����: �������� ����� InitWeaponAimings()
        // ������� ���� _weaponAimings � _aimTransform
        InitWeaponAimings(_weaponAimings, _aimTransform);
    }

    private void InitWeaponAimings(WeaponAiming[] weaponAimings, Transform aim)
    {
        // �������� �� ���� ��������� weaponAimings
        for (int i = 0; i < weaponAimings.Length; i++)
        {
            // �������� � weaponAimings[i] ����� Init()
            // � ������� ��� ��������� ���� aim
            weaponAimings[i].Init(aim);
        }
        // �������� � _rigBuilder ���������� ����� Build()
        // ����� ��������� ��������� �������� �����
        _rigBuilder.Build();
    }

    private void FixedUpdate()
    {
        // ��������� � ����� ����������
        Gravity();

        // ������� ����� ���������
        Movement();

        // ��������� ������� �����
        Jumping();

        // ������������ ����� ��� �������
        Aiming();
    }

    private void Gravity()
    {
        // ������ ���������� gravity ���� Vector3
        // ����������� �� �������� ���� ���������� �� ����������� ������ Physics
        Vector3 gravity = Physics.gravity;

        // �������� ���������� �� ��������� � ����� �����
        gravity *= _gravityMultiplier * Time.fixedDeltaTime;

        // ������ ���������� ���������� CharacterController
        _characterController.Move(gravity);
    }

    private void Movement()
    {
        // ������ ���������� movement �� ��������� (0, 0, 0)
        Vector3 movement = Vector3.zero;

        // ����� movement.x �������� ��������������� ����� � ���������� (������� A � D)
        movement.x = Input.GetAxis(MovementHorizontalKey);

        // ����� movement.z �������� ������������� ����� � ���������� (������� W � S)
        movement.z = Input.GetAxis(MovementVerticalKey);

        // ����������� ������ ����������� ������������ ������
        movement = GetMovementByCamera(movement);

        // ��������� ������ �����������
        // ����� �������� � ����� ����� �������������� �������
        movement *= _movementSpeed * Time.fixedDeltaTime;

        // ������ �������� ���������� CharacterController
        _characterController.Move(movement);

        // ��������� �������� �����
        AnimateMovement(movement);
    }

    private Vector3 GetMovementByCamera(Vector3 input)
    {
        // �������� ������ ����������� ������ �����
        Vector3 cameraForward = _mainCamera.transform.forward;

        // �������� ������ ����������� ������ ������
        Vector3 cameraRight = _mainCamera.transform.right;

        // �������� �������� ������� ����������� �����
        cameraForward.y = 0f;

        // �������� �������� ������� ����������� ������
        cameraRight.y = 0f;

        // ����������� ������ ����������� �����
        cameraForward.Normalize();

        // ����������� ������ ����������� ������
        cameraRight.Normalize();

        // ���������� ������� �������� � ������ ����������� ������
        Vector3 movement = cameraForward * input.z + cameraRight * input.x;

        // ���������� ���������� ������ ��������
        return movement;
    }

    private void AnimateMovement(Vector3 movement)
    {
        // �������� �������� ������� �������� �� ��� X
        float relatedX = Vector3.Dot(movement.normalized, transform.right);

        // �������� �������� ������� �������� �� ��� Y
        float relatedY = Vector3.Dot(movement.normalized, transform.forward);

        // ������������� �������� �������� ��������������� ��������
        _animator.SetFloat(MovementHorizontalKey, relatedX);

        // ������������� �������� �������� ������������� ��������
        _animator.SetFloat(MovementVerticalKey, relatedY);
    }

    private void Jumping()
    {
        // ��������� ������ � �����������
        RefreshIsGrounded();

        // ���� ������ ������� �������
        // � ����� ��������� �� �����
        // � ������ ������ �� �����������
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded && !_isJumping)
        {
            // ������� ��������� ��� �����
            SetIsGrounded(false);

            // ������ ���� �� ������
            _isJumping = true;

            // �������� ������ ������
            _jumpTimer = 0;
        }
        // ���� ����� � ������
        if (_isJumping)
        {
            // ����������� ������ ������
            _jumpTimer += Time.fixedDeltaTime;

            // ������������ ������������ ���� �������� ����� � ������ ������������ ������
            Vector3 motion = Vector3.up * _jumpSpeed * (1 - _jumpTimer / _jumpDuration) * Time.fixedDeltaTime;

            // ��������� ���� ����� Character Controller
            _characterController.Move(motion);

            // ���� ������������ ������ ���������
            // ��� ���� ����� �����������
            if (_jumpTimer >= _jumpDuration || _isGrounded)
            {
                // ������� ���� �� ������
                _isJumping = false;
            }
        }
    }

    private void RefreshIsGrounded()
    {
        // ������������� ��������� ����������� ����� GroundCheck()
        SetIsGrounded(GroundCheck());
    }

    private bool GroundCheck()
    {
        // ��������� ������� ���������� ��������� ��� �������� �����
        Vector3 startCheckPosition = transform.position + Vector3.up * _groundCheckExtraUp;

        // ��������� ����� ���� ��� �������� �����
        float checkDistance = _groundCheckDistance + _groundCheckExtraUp;

        // ���������� ��������� � ����������� �� ��������� ������� �� ����� ���� �����
        return Physics.BoxCast(startCheckPosition, _groundCheckBox, Vector3.down, transform.rotation, checkDistance);
    }

    private void SetIsGrounded(bool value)
    {
        // ���� ��������� ����� ����������
        // �� ��� ����� � �� ������ ��� ��������
        if (value != _isGrounded)
        {
            // ��������� �������� � ���������
            _animator.SetBool(IsGroundedKey, value);
        }
        // ����������� ����� ��� ����� ���������� ��������
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

            // �����: ������ ���������� ���� ������
            // � ����� ������������ � �������� ���������
            _aimTransform.position = Vector3.Lerp(_aimTransform.position, hitInfo.point, _aimingSpeed * Time.fixedDeltaTime);
        }
    }


}
