using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    // �������� ���������
    private CharacterMovement _movement;

    // ������������ ���������
    private CharacterAiming _aiming;

    // ����� ���������
    private CharacterPart[] _parts;

    // �������� ���������
    private CharacterShooting _shooting;

    private void Start()
    {
        // �������� ����� Init()
        Init();
    }

    private void Init()
    {
        _movement = GetComponent<CharacterMovement>();
        _aiming = GetComponent<CharacterAiming>();

        // �����: �������� ��������� �������� ���������
        _shooting = GetComponent<CharacterShooting>();

        _parts = new CharacterPart[]
        {
        _movement,

        // ����� �������� �������
        _aiming,

        // �����: ������� ������� ���������
        _shooting
        };

        // ���������� ����� ������
    

        // �������� �� ���� ��������� �������
        for (int i = 0; i < _parts.Length; i++)
        {
            // ���������, ���������� �� ������� �������
            if (_parts[i])
            {
                // �������� ����� Init() ��� �������� ��������
                _parts[i].Init();
            }
        }
    }
    
}