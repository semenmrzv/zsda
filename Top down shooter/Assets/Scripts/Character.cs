using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    // Движение персонажа
    private CharacterMovement _movement;

    // Прицеливание персонажа
    private CharacterAiming _aiming;

    // Части персонажа
    private CharacterPart[] _parts;

    // Стрельба персонажа
    private CharacterShooting _shooting;

    private void Start()
    {
        // Вызываем метод Init()
        Init();
    }

    private void Init()
    {
        _movement = GetComponent<CharacterMovement>();
        _aiming = GetComponent<CharacterAiming>();

        // НОВОЕ: Получаем компонент стрельбы персонажа
        _shooting = GetComponent<CharacterShooting>();

        _parts = new CharacterPart[]
        {
        _movement,

        // Здесь добавили запятую
        _aiming,

        // НОВОЕ: Элемент массива «Стрельба»
        _shooting
        };

        // Оставшаяся часть метода
    

        // Проходим по всем элементам массива
        for (int i = 0; i < _parts.Length; i++)
        {
            // Проверяем, существует ли текущий элемент
            if (_parts[i])
            {
                // Вызываем метод Init() для текущего элемента
                _parts[i].Init();
            }
        }
    }
    
}