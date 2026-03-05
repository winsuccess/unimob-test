using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TransferManager : MonoBehaviour
{
    private static TransferManager _instance;

    public static TransferManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TransferManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("TransferManager");
                    _instance = obj.AddComponent<TransferManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private MarketHandler _market;

    private List<ConstructionTransfer> _consTransferList = new List<ConstructionTransfer>();
    private List<CustomerTransfer> _cusTransferList = new List<CustomerTransfer>();
    private int[] seats = { 0, 0, 0, 0 }; // 0 empty, 1 in
    private Queue<ConstructionTransfer> _deliveryTransferQueue = new Queue<ConstructionTransfer>();
    private Queue<CustomerTransfer> _customerTransferQueue = new Queue<CustomerTransfer>();

    public void AddConstruction(ConstructionHandler c)
    {
        var transfer = new ConstructionTransfer
        {
            ID = _consTransferList.Count + 1,
            constructionHandler = c,
            State = ConstructionTransfer.TransferState.None
        };
        _consTransferList.Add(transfer);
    }

    public void CheckDelivery(ConstructionTransfer transfer)
    {

    }

    public void AddCustomer(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var transfer = new CustomerTransfer
            {
                ID = _cusTransferList.Count + 1,
                State = CustomerTransfer.TransferState2.None,
            };
            _cusTransferList.Add(transfer);
        }
    }

    public int CheckSeat()
    {
        List<int> availableSeats = new List<int>();
        for (int i = 0; i < seats.Length; i++)
        {
            if (seats[i] == 0)
            {
                availableSeats.Add(i);
            }
        }

        if (availableSeats.Count > 0)
        {
            int randomIndex = availableSeats[UnityEngine.Random.Range(0, availableSeats.Count)];
            seats[randomIndex] = 1;
            return randomIndex;
        }
        return -1; // No available seats
    }

    public void Start()
    {
        ResourceManager.Instance.AddCustomer(1);
    }

    private void Update()
    {
        for (int i = 0; i < _consTransferList.Count; i++)
        {
            var transfer = _consTransferList[i];
            switch (transfer.State)
            {
                case ConstructionTransfer.TransferState.None:
                    transfer.State = ConstructionTransfer.TransferState.Collecting;
                    transfer.Delivery = HumanManager.Instance.SpawnDelivery(transfer.constructionHandler.PositionStart());
                    HumanManager.Instance.PlayMove(transfer.Delivery);
                    transfer.Delivery.transform.rotation = Quaternion.Euler(0f, transfer.constructionHandler.GetSide() == 1 ? -90f : 90f, 0f);
                    break;
                case ConstructionTransfer.TransferState.Collecting:
                    if (transfer.Delivery != null)
                    {
                        transfer.Delivery.transform.position
                            = Vector3.MoveTowards(transfer.Delivery.transform.position, transfer.constructionHandler.PositionEnd(), speed * Time.deltaTime);
                        if (Vector3.Distance(transfer.Delivery.transform.position, transfer.constructionHandler.PositionEnd()) < 0.01f)
                        {
                            transfer.State = ConstructionTransfer.TransferState.Collected;
                            transfer.Collected = transfer.constructionHandler.GetCurrentProfit();
                            HumanManager.Instance.PlayIdle(transfer.Delivery);
                            transfer.Delivery.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
                        }
                    }
                    break;
                case ConstructionTransfer.TransferState.Collected:
                    if (transfer.Delivery != null)
                    {
                        if (transfer.collectTime < .5f)
                        {
                            transfer.collectTime += Time.deltaTime;
                        }
                        else
                        {
                            HumanManager.Instance.PlayCarryIdle(transfer.Delivery);
                            transfer.State = ConstructionTransfer.TransferState.WaitingDelivery;
                            _deliveryTransferQueue.Enqueue(transfer);
                        }
                    }
                    break;
                case ConstructionTransfer.TransferState.WaitingDelivery:

                    //if (_customerTransferQueue.Count == 0)
                    //{
                    //    break; // No customers waiting, skip this transfer
                    //}
                    //var cus = _customerTransferQueue.Dequeue();
                    //if (cus != null)
                    //{
                    //    transfer.Customer = cus;
                    //    transfer.State = ConstructionTransfer.TransferState.Delivering;
                    //    HumanManager.Instance.PlayMove(transfer.Delivery);
                    //}
                    break;
                case ConstructionTransfer.TransferState.Delivering:
                    if (transfer.Delivery != null && transfer.Customer != null)
                    {
                        transfer.Delivery.transform.position
                            = Vector3.MoveTowards(transfer.Delivery.transform.position, transfer.PosData[1], speed * Time.deltaTime);
                        if (Vector3.Distance(transfer.Delivery.transform.position, transfer.PosData[1]) < 0.01f)
                        {
                            HumanManager.Instance.PlayIdle(transfer.Delivery);
                            transfer.State = ConstructionTransfer.TransferState.Transfering;
                            transfer.Customer.State = CustomerTransfer.TransferState2.Transfering;
                            EffectManager.Instance.SpawnPayEffect(transfer.Customer.Customer.transform.position);
                        }
                    }
                    break;
                case ConstructionTransfer.TransferState.Transfering:
                    if (transfer.Delivery != null && transfer.Customer != null)
                    {
                        if (transfer.transferTime < .5f)
                        {
                            transfer.transferTime += Time.deltaTime;
                        }
                        else
                        {
                            transfer.State = ConstructionTransfer.TransferState.End;
                            HumanManager.Instance.PlayMove(transfer.Delivery);
                            ResourceManager.Instance.AddGold(transfer.Collected);
                            transfer.Delivery.transform.rotation = Quaternion.Euler(0f, transfer.Seat <= 1 ? -90f : 90f, 0f);

                        }
                    }
                    break;
                case ConstructionTransfer.TransferState.End:
                    if (transfer.Delivery != null && transfer.Customer != null)
                    {
                        transfer.Delivery.transform.position
                             = Vector3.MoveTowards(transfer.Delivery.transform.position, transfer.PosData[4], speed * Time.deltaTime);
                        if (Vector3.Distance(transfer.Delivery.transform.position, transfer.PosData[4]) < 0.01f)
                        {
                            HumanManager.Instance.PlayIdle(transfer.Delivery);
                            transfer.State = ConstructionTransfer.TransferState.None;
                            //Return
                            HumanManager.Instance.ReturnToPool(transfer.Delivery, true);
                            transfer.Collected = 0;
                            transfer.collectTime = 0f;
                            transfer.transferTime = 0f;
                            transfer.Delivery = null;
                            transfer.Customer = null;
                        }
                    }
                    break;
            }
        }

        for (int i = 0; i < _cusTransferList.Count; i++)
        {
            var transfer = _cusTransferList[i];
            switch (transfer.State)
            {
                case CustomerTransfer.TransferState2.None:
                    var seat = CheckSeat();
                    if (seat == -1)
                    {
                        Debug.Log("No available seats for customer " + transfer.ID);
                        continue; // No available seats, skip this customer
                    }
                    transfer.State = CustomerTransfer.TransferState2.Positioning;
                    transfer.Seat = seat;
                    transfer.PosData = _market.GetPos(seat);
                    transfer.Customer = HumanManager.Instance.SpawnCustomer(transfer.PosData[2]);
                    transfer.Customer.transform.rotation = Quaternion.Euler(0f, transfer.Seat <= 1 ? 90f : -90f, 0f);
                    HumanManager.Instance.PlayMove(transfer.Customer);
                    break;
                case CustomerTransfer.TransferState2.Positioning:
                    if (transfer.Customer != null)
                    {
                        transfer.Customer.transform.position
                            = Vector3.MoveTowards(transfer.Customer.transform.position, transfer.PosData[0], speed * Time.deltaTime);
                        if (Vector3.Distance(transfer.Customer.transform.position, transfer.PosData[0]) < 0.01f)
                        {
                            transfer.Customer.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
                            HumanManager.Instance.PlayIdle(transfer.Customer);
                            transfer.State = CustomerTransfer.TransferState2.Waiting;
                            _customerTransferQueue.Enqueue(transfer);
                        }
                    }
                    break;
                case CustomerTransfer.TransferState2.Waiting:
                    break;
                case CustomerTransfer.TransferState2.Transfering:
                    if (transfer.transferTime < .5f)
                    {
                        transfer.transferTime += Time.deltaTime;
                    }
                    else
                    {
                        transfer.State = CustomerTransfer.TransferState2.End;
                        seats[transfer.Seat] = 0;
                        HumanManager.Instance.PlayCarryMove(transfer.Customer);
                        transfer.Customer.transform.rotation = Quaternion.Euler(0f, transfer.Seat <= 1 ? -90f : 90f, 0f);
                    }
                    break;
                case CustomerTransfer.TransferState2.End:
                    if (transfer.Customer != null)
                    {

                        transfer.Customer.transform.position
                            = Vector3.MoveTowards(transfer.Customer.transform.position, transfer.PosData[3], speed * Time.deltaTime);
                        if (Vector3.Distance(transfer.Customer.transform.position, transfer.PosData[3]) < 0.01f)
                        {
                            HumanManager.Instance.PlayIdle(transfer.Customer);
                            transfer.State = CustomerTransfer.TransferState2.None;
                            HumanManager.Instance.ReturnToPool(transfer.Customer, false);
                            transfer.transferTime = 0f;
                            transfer.Customer = null;
                        }
                    }
                    break;
            }
        }

        //Check queue transfer
        if (_deliveryTransferQueue.Count > 0 && _customerTransferQueue.Count > 0)
        {
            var transfer = _deliveryTransferQueue.Dequeue();
            var cus = _customerTransferQueue.Dequeue();
            if (transfer != null && cus != null)
            {
                transfer.Customer = cus;
                transfer.Seat = cus.Seat;
                transfer.PosData = new List<Vector3>(cus.PosData);
                transfer.State = ConstructionTransfer.TransferState.Delivering;
                HumanManager.Instance.PlayCarryMove(transfer.Delivery);
                transfer.Delivery.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }
}

[Serializable]
public class ConstructionTransfer
{
    public int ID;
    public ConstructionHandler constructionHandler;
    public TransferState State;
    public GameObject Delivery;
    public CustomerTransfer Customer;
    public int Seat;
    public List<Vector3> PosData;
    public double Collected;
    public float collectTime; //.5s collect
    public float transferTime; //.5s transfer

    public enum TransferState
    {
        None,
        Collecting,
        Collected,
        WaitingDelivery,
        Delivering,
        Transfering,
        End,
    }
}

[Serializable]
public class CustomerTransfer
{
    public int ID;
    public GameObject customer;
    public int Seat; // 0-3
    public TransferState2 State;
    public GameObject Customer;
    public List<Vector3> PosData;
    public float transferTime; //.5s transfer
    public enum TransferState2
    {
        None,
        Positioning,
        Waiting,
        Transfering,
        End,
    }

    public CustomerTransfer() { }

}
