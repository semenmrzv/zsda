using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponAiming : MonoBehaviour
{

    // ������ ������������� �� ��������� �����
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
        // ������ ������ ������������ ���� constraintSourceObject
        // � ������� ������ CreateConstraintSourceObject()
        WeightedTransformArray constraintSourceObject = CreateConstraintSourceObject(aim);

        // ����������� _constraints ���������� MultiAimConstraint �� �������� ��������
        _constraints = GetComponentsInChildren<MultiAimConstraint>(true);

        // �������� �� ���� ��������� ������� _constraints
        for (int i = 0; i < _constraints.Length; i++)
        {
            // ������������� �������� ������� constraintSourceObject
            // � �������� sourceObjects ������� �������� _constraints
            _constraints[i].data.sourceObjects = constraintSourceObject;
        }
    }

    public void SetActive(bool value)
    {
        // ������ ������ �������� ��� ����������
        gameObject.SetActive(value);
    }

    private WeightedTransformArray CreateConstraintSourceObject(Transform aim)
    {
        // ������ ����������-������ constraintAimArray
        // ���� WeightedTransformArray �� ��������� 1
        var constraintAimArray = new WeightedTransformArray(1);

        // ����������� ������� �������� constraintAimArray �������� ������ �������
        // ������ WeightedTransform � ����������� aim � 1
        constraintAimArray[0] = new WeightedTransform(aim, 1);

        // ���������� �������� constraintAimArray
        return constraintAimArray;
    }
}
