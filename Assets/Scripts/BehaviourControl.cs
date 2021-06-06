using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourControl : MonoBehaviour
{
    private enum MovimentType { Random, GoToFood }

    public float speedModifier;
    public float movimentModifier;
    public float distanceModifier;
    public int energyModifier;
    public float visionModifier;

    const int baseEnergy = 10;
    const float baseSpeed = 1f;
    const float baseDistance = 1f;
    const float baseMoviment = 4f;
    const float baseVision = 2f;

    private float timeToMove = 1f;
    private float heartBeat;
    private float speed;
    private int energy;
    private bool isMoving;
    private Vector3 positionToGo;
    private GameObject food = null;
    private MovimentType currentMoviment = MovimentType.Random;
    private CircleCollider2D vision;

    private void Start()
    {
        InvokeRepeating("HandleSecond", 1f, 1f);

        positionToGo = GetNewMovimentDestination();

        energy = baseEnergy + energyModifier;
        
        vision = GetComponent<CircleCollider2D>();
        vision.radius = baseVision * visionModifier;

        speed = baseSpeed * Time.deltaTime * speedModifier;

        currentMoviment = MovimentType.Random;
    }

    void Update()
    {
        heartBeat += Time.deltaTime * 1f;
        HandleMoviment();
        UpdateProps();
    }

    void UpdateProps()
    {
        vision.radius = baseVision * visionModifier;
    }

    void HandleSecond()
    {
        Debug.Log(isMoving);
        HandleAlive();
    }

    void HandleMoviment()
    {
        switch (currentMoviment)
        {
            case MovimentType.Random:
                RandomMoviment();
                break;
            case MovimentType.GoToFood:
                GoToFood();
                break;
            default:
                ResetMoviment();
                RandomMoviment();
                break;
        }
    }

    void GoToFood()
    {
        isMoving = true;
        positionToGo = food.transform.position;
        transform.position = Vector3.Lerp(transform.position, food.transform.position, speed);
        if (Vector3.Distance(transform.position, food.transform.position) <= 0.001f)
        {
            currentMoviment = MovimentType.Random;
            Destroy(food.gameObject);
            food = null;
            ResetMoviment();
        }
    }

    void RandomMoviment()
    {
        if (heartBeat >= timeToMove)
        {
            isMoving = true;
            float speed = baseSpeed * Time.deltaTime * speedModifier;
            transform.position = Vector3.Lerp(transform.position, positionToGo, speed);
            if (Vector3.Distance(transform.position, positionToGo) <= 0.01f)
            {
                ResetMoviment();
            }
        }
    }

    void ResetMoviment()
    {
        isMoving = false;
        positionToGo = GetNewMovimentDestination();
        timeToMove = heartBeat + Random.Range(3f, baseMoviment + movimentModifier);
    }

    Vector3 GetNewMovimentDestination()
    {
        return new Vector3(Random.Range(-(baseDistance + distanceModifier), (baseDistance + distanceModifier)), Random.Range(-(baseDistance + distanceModifier), (baseDistance + distanceModifier)), 0);
    }

    void HandleAlive()
    {
        if( energy == 0)
        {
            Kill();
        }
        else
        {
            energy -= 1;
        }
    }

    void Kill()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Food")
        {
            food = collision.gameObject;
            currentMoviment = MovimentType.GoToFood;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Showing next moviment
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(positionToGo, 0.2f);
    }

    void OnValidate()
    {
        if (visionModifier < 1f)
        {
            visionModifier = 1f;
        }
    }

    void ApplyModifiers(Modifiers modifiers)
    {
        speedModifier = modifiers.speed;
        distanceModifier = modifiers.distanceModifier;
        movimentModifier = modifiers.movimentModifier;
        energyModifier = modifiers.energyModifier;
        visionModifier = modifiers.visionModifier;
    }
}

[System.Serializable]
public struct Modifiers
{
    public int speed;
    public float distanceModifier;
    public float movimentModifier;
    public int energyModifier;
    public float visionModifier;
}