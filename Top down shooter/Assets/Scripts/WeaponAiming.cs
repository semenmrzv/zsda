using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponAiming : MonoBehaviour
{

    // Массив ограничителей на несколько целей
    private MultiAimConstraint[] _constraints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Transform aim)
    {
        // Создаём объект ограничителя цели constraintSourceObject
        // С помощью метода CreateConstraintSourceObject()
        WeightedTransformArray constraintSourceObject = CreateConstraintSourceObject(aim);

        // Присваиваем _constraints компоненты MultiAimConstraint из дочерних объектов
        _constraints = GetComponentsInChildren<MultiAimConstraint>(true);

        // Проходим по всем элементам массива _constraints
        for (int i = 0; i < _constraints.Length; i++)
        {
            // Устанавливаем источник объекта constraintSourceObject
            // В свойство sourceObjects каждого элемента _constraints
            _constraints[i].data.sourceObjects = constraintSourceObject;
        }
    }

    public void SetActive(bool value)
    {
        // Делаем оружие активным или неактивным
        gameObject.SetActive(value);
    }

    private WeightedTransformArray CreateConstraintSourceObject(Transform aim)
    {
        // Создаём переменную-массив constraintAimArray
        // Типа WeightedTransformArray со значением 1
        var constraintAimArray = new WeightedTransformArray(1);

        // Присваиваем первому элементу constraintAimArray значение нового объекта
        // Класса WeightedTransform с параметрами aim и 1
        constraintAimArray[0] = new WeightedTransform(aim, 1);

        // Возвращаем значение constraintAimArray
        return constraintAimArray;
    }
}
