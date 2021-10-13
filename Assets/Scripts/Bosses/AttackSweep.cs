using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSweep : BossAttack
{
    //Fields
    private float timer;
    private float _sweepTime;
    private float _hangTime;
    private Vector2 _origin;
    private float _direction;
    private float _length;

    //Properties
    public float SweepTime
    {
        get { return _sweepTime; }
        set { _sweepTime = value; }
    }
    public float HangTime
    {
        get { return _hangTime; }
        set { _hangTime = value; }
    }
    public Vector2 Origin
    {
        //get { return _origin; }
        set { _origin = value; }
    }
    public float Direction
    {
        //get { return _direction; }
        set { _direction = value; }
    }
    public float Length
    {
        //get { return _length; }
        set { _length = value; }
    }
    

    // Start is called before the first frame update
    protected override void Start()
    {
        //Safe defaulting
        timer = 0;
        _sweepTime = 1.0f;
        _hangTime = 0.1f;
        _origin = new Vector2(transform.position.x, transform.position.y);
        _direction = 0.0f;
        _length = 1.0f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        timer += Time.deltaTime;
        //Extension period
        if (timer < _sweepTime / 2)
        {
            //Adjusts the x component of scale
            transform.localScale = new Vector3((timer / (_sweepTime / 2)) * _length, transform.localScale.y, transform.localScale.z);

            //Adjusts the x component of position
            transform.position = new Vector3(
                _origin.x + (timer / (_sweepTime / 2)) * _length / 2,
                transform.position.y,
                transform.position.z
                ); 
        }
        //Hang period
        else if (timer > _sweepTime / 2 && timer < (_sweepTime / 2) + _hangTime)
        {
            
            //Moves the sweep rectangle to the end and rotates it, so that retraction works in the correct direction
            transform.position = new Vector3(
                transform.position.x + (_length * Mathf.Cos(_direction)),
                transform.position.y + (_length * Mathf.Sin(_direction))
                );
            transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
            
        }
        //Retraction period
        else if (timer > (_sweepTime / 2) + _hangTime && timer < _sweepTime + _hangTime)
        {
            //Adjusts the x component of scale
            //transform.localScale = new Vector3((((_sweepTime / 2) + _hangTime) - timer) * _length, transform.localScale.y, transform.localScale.z);
            transform.localScale = new Vector3(
                _length * (1 - ((timer - (_sweepTime + _hangTime)) / (_sweepTime / 2))),
                transform.localScale.y, 
                transform.localScale.z);
            //Adjusts the x component of position
            transform.position = new Vector3(
                (_origin.x + _length) - (((_sweepTime / 2) + _hangTime) - timer) * _length / 2,
                transform.position.y,
                transform.position.z
                );
        }
        //End of sweep
        else
            Destroy(this.gameObject);

    }
}
