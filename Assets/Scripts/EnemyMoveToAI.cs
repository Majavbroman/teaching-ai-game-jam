using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyMoveToAI : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    //The AI core/center the enemy will move towards.
    //SerializeField us see it in the inspector and drag the object that will be the AI into this field.
    //[SerializeField] private GameObject aiCore;
    private AiCenter aiCore;

    //How fast the enemy moves, can also be change in the inspector
    [SerializeField] private float speed = 1.5f;

    //Decides which movement pattern this enemy will use.
    //0 = straight, 1 = normal wobble, 2 = slow wide wobble and 3 = fast zigzag.
    private int movementType;

    //Ramdom value used so enemies of the same movementType do not wobble in sync.
    private float randomOffset;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //Pick a random movement type when the enemy spawnes in.
        //Random.Range(0, 4) gives 0, 1, 2, or 3.
        movementType = Random.Range(0, 4);

        //Makes the wobble happen at different times for each enemy.
        randomOffset = Random.Range(0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        //If no AI core/center has been assigned, stop the code here to avoid errors.
        aiCore = AiCenter.Instance;
        if (aiCore == null)
            return;

        //Fine the direction from the enemy to the AI.
        //normalized makes the direction lenght 1, so the speed stays consistent.
        Vector2 direction = (aiCore.transform.position - transform.position).normalized;

        //Creates a sideways direction based on the direction toward the AI.
        //This is used to make the wobble/zigzag movement.
        Vector2 sideDirection = new Vector2(-direction.y, direction.x);

        //Start by assuming the enmy moves straight toward the AI.
        Vector2 finalDirection = direction;

        //MovementType 0 = move in a straight line toward the AI.
        if (movementType == 0)
        {
            finalDirection = direction;
        }

        //MovementType 1 = normal wobble toward the AI.
        else if (movementType == 1)
        {
            float wobble = Mathf.Sin(Time.time * 3f + randomOffset) * 0.5f;
            finalDirection = direction + sideDirection * wobble;
        }

        //MovementType 2 = slower, wider wobble toward the AI.
        else if (movementType == 2)
        {
            float wobble = Mathf.Sin(Time.time * 1.5f + randomOffset) * 0.8f;
            finalDirection = direction + sideDirection * wobble;
        }

        //MovementType 3 = faster zigzag toward the AI.
        else if (movementType == 3)
        {
            float wobble = Mathf.Sin(Time.time * 7f + randomOffset) * 0.6f;
            finalDirection = direction + sideDirection * wobble;
        }

        //Move the enemy in the chosen direction.
        //Time.deltaTime makes movement smooth and independent of frame rate.
        transform.position += (Vector3)(finalDirection.normalized * speed * Time.deltaTime);

        spriteRenderer.flipX = finalDirection.x > 0;
    }
}
