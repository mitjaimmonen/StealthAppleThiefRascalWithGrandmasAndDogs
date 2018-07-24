using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaypointSystem;
using AIStateStuff;

public class AI : MonoBehaviour
{

    public bool switchState = false;
    public float gameTimer;
    public int seconds = 0;
    public float _moveSpeed = 5;
    public float _turnSpeed = 8;
    public Vector3 startPosition;

    
    public Path _path;


    public float _waypointArriveDistance = 0.5f;

    public Direction _direction;

    private IMover _mover;
    public IMover Mover { get { return _mover; } }

    private StateMachine stateMachine { get; set; }

    private void Awake()
    {
        startPosition = transform.position;
    }

    public void Init()
    {
        _mover = gameObject.GetOrAddComponent<TransformMover>();
        _mover.Init(_moveSpeed, _turnSpeed);
    }

    private void Start()
    {
        stateMachine = new StateMachine(this);
        gameTimer = Time.time;

        Init();
        stateMachine.InitStates();
    }

  

    private void Update()
    {     
        stateMachine.ExecuteStateUpdate();
    }

}
