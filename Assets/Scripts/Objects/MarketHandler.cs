using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MarketHandler : MonoBehaviour
{
    [SerializeField]
    private Transform _customerLeft;
    [SerializeField]
    private Transform _customerRight;
    [SerializeField]
    private Transform _deliLeft;
    [SerializeField]
    private Transform _deliRight;
    [SerializeField]
    private Transform[] deliPos;
    [SerializeField]
    private Transform[] cusPos;


    public List<Vector3> GetPos(int index) // random {0-3} return in order : 0 cus pos, 1 deli pos , 2 cus start, 3 cus end, 4 deli end
    {
        var p = index;
        if (p < 0 || p > 3)
        {
            return null;
        }
        var list = new List<Vector3>();
        list.Add(cusPos[p].position);
        list.Add(deliPos[p].position);
        if (p <= 1)
        {
            list.Add(_customerLeft.position);
            list.Add(_customerLeft.position);
            list.Add(_deliLeft.position);
        }
        else
        {
            list.Add(_customerRight.position);
            list.Add(_customerRight.position);
            list.Add(_deliRight.position);
        }

        return list;
    }
}
