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

    private float extendProgress;
    private float hangProgress;
    private float retractProgress;

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
        /*_sweepTime = 1.0f;
        _hangTime = 0.1f;
        _origin = new Vector2(transform.position.x, transform.position.y);
        _direction = 0.0f;
        _length = 1.0f;*/
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Update the progress in each stage to a value between 0 and 1 representing the end and beginning of that stage
        if (timer > 0 && timer < _sweepTime)
        {
            extendProgress = timer / _sweepTime;
            hangProgress = 0.0f;
            retractProgress = 0.0f;
        }
        else if (timer > _sweepTime && timer < (_sweepTime + _hangTime))
        {
            extendProgress = 0.0f;
            hangProgress = (timer - _sweepTime) / (_hangTime);
            retractProgress = 0.0f;
        }
        else if (timer > _sweepTime + _hangTime && timer < (_sweepTime * 2) + _hangTime)
        {
            extendProgress = 0.0f;
            hangProgress = 0.0f;
            retractProgress = (timer - (_sweepTime + _hangTime)) / (_sweepTime);
        }
        timer += Time.deltaTime;
        //Extension period
        if (extendProgress > 0.0f)
        {
            //Adjusts the x component of scale
            transform.localScale = new Vector3(_length * extendProgress, 1.0f, 1.0f);

            //Adjusts the x component of position
            transform.position = new Vector3(
                _origin.x + ((_length / 2) * Mathf.Cos(_direction + (Mathf.PI / 2)) * extendProgress),
                _origin.y + ((_length / 2) * Mathf.Sin(_direction + (Mathf.PI / 2)) * extendProgress),
                transform.position.z
                ); 
        }
        //Hang period
        else if (hangProgress > 0.0f)
        {
            //Adjusts the x component of scale
            transform.localScale = new Vector3(_length, 1.0f, 1.0f);

            //Moves the sweep rectangle to the end and rotates it, so that retraction works in the correct direction
            transform.position = new Vector3(
                _origin.x + (_length * Mathf.Cos(_direction)),
                _origin.y + (_length * Mathf.Sin(_direction)),
                transform.position.z
                );
            transform.rotation = Quaternion.AngleAxis((Mathf.Rad2Deg * _direction) + 180, Vector3.forward);

        }
        //Retraction period
        else if (retractProgress > 0.0f)
        {
            //Adjusts the x component of scale
            //transform.localScale = new Vector3((((_sweepTime / 2) + _hangTime) - timer) * _length, transform.localScale.y, transform.localScale.z);
            transform.localScale = new Vector3(
                _length * (1 - extendProgress),
                transform.localScale.y, 
                transform.localScale.z);
            //Adjusts the x component of position
            transform.position = new Vector3(
                (_origin.x + _length) - (_length * retractProgress),
                transform.position.y,
                transform.position.z
                );
        }
        //End of sweep
        else
            Destroy(this.gameObject);

    }
}
