using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class GameUpdater : MonoBehaviour
{
    [SerializeField] private int listUpdateCounter;
    [SerializeField] private int listFixedUpdateCounter;

    private LinkedList<Action> listOfUpdateEvent;
    private LinkedList<Action> listOfFixedUpdateEvent;

    private static GameUpdater instance = null;
    internal static GameUpdater GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameUpdater>();

                if (instance != null)
                {
                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }

    private void OnDisable()
    {
        if (ListOfUpdateEvent?.Count > 0)
        {
            ListOfUpdateEvent.Clear();
            listUpdateCounter = 0;
        }

        if (listOfFixedUpdateEvent?.Count > 0)
        {
            listOfFixedUpdateEvent.Clear();
            listFixedUpdateCounter = 0;
        }
    }

    private LinkedList<Action> ListOfUpdateEvent
    {
        get
        {
            if (listOfUpdateEvent == null)
            {
                listUpdateCounter = 0;
                listOfUpdateEvent = new LinkedList<Action>();
            }
            return listOfUpdateEvent;
        }
    }

    private LinkedList<Action> ListOfFixedUpdateEvent
    {
        get
        {
            if (listOfFixedUpdateEvent == null)
            {
                listFixedUpdateCounter = 0;
                listOfFixedUpdateEvent = new LinkedList<Action>();
            }
            return listOfFixedUpdateEvent;
        }
    }

    private void Awake()
    {
        if (GetInstance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (listUpdateCounter > 0)
        {
            LinkedListNode<Action> currentNode = ListOfUpdateEvent.First;
            while (currentNode != null)
            {
                currentNode.Value.Invoke();
                currentNode = currentNode.Next;
            }
        }
    }

    internal void AddToUpdateEvent(Action UpdateFunction)
    {
        ListOfUpdateEvent.AddLast(UpdateFunction);
        listUpdateCounter++;
    }

    internal void RemoveFromUpdateEvent(Action UpdateFunction)
    {
        if (listUpdateCounter > 0)
        {
            ListOfUpdateEvent.Remove(UpdateFunction);
            listUpdateCounter--;
        }
    }

    internal void FixedUpdate()
    {
        if (listFixedUpdateCounter > 0)
        {
            LinkedListNode<Action> currentNode = ListOfFixedUpdateEvent.First;
            while (currentNode != null)
            {
                currentNode.Value.Invoke();
                currentNode = currentNode.Next;
            }
        }
    }

    internal void AddToFixedUpdateEvent(Action FixedUpdateFunction)
    {
        ListOfFixedUpdateEvent.AddLast(FixedUpdateFunction);
        listFixedUpdateCounter++;
    }

    internal void RemoveFromFixedUpdateEvent(Action FixedUpdateFunction)
    {
        if (listFixedUpdateCounter > 0)
        {
            ListOfFixedUpdateEvent.Remove(FixedUpdateFunction);
            listFixedUpdateCounter--;
        }
    }
}