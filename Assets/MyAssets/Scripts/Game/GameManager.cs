using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameController gameController;
    void Awake()
    {

    }

    void Start()
    {
        gameController = GetComponent<GameController>();
        gameController.OnStart();
    }


    void Update()
    {
        gameController.OnUpdate();
    }
}
