using UnityEngine;
/// <summary>
/// check target inside threat zone:
/// zombies will check for player which distance is shorter than 15
/// check target in range:
/// zombies will check for player which distance is shorter than 3
/// check friends near:
/// zombies will check for other which distance is shorter than 2
/// check spawn point:
/// zombies will move to spawn point if they are too far from their spawn point with distance longer than 6
/// check hitpoints:
/// when zombies have only 1 hitpoint, they are low HP statu.
/// </summary>
public class ZombieAI : MonoBehaviour {


    public float move_Speed;
    public float rotate_Speed = 50f;
    public int startingHitPoint = 3;
    public int currentHitPoint;
    public Vector3 mySpawnPoint;

    private AudioSource audioSource;
    private GameObject target;

    bool isActive = true;

    DecisionTree root;

    void Awake()
    {
        currentHitPoint = startingHitPoint;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Use this for initialization
    void Start () {

        audioSource = GetComponent<AudioSource>();
        BuildDecisionTree();
    }


    // Update is called once per frame
    void Update () {
        if (isActive)
            root.Search();
    }
    bool checkTargetInsideThreatZone()
    {
        return Vector2.Distance(this.transform.position, target.transform.position) < 15;
    }

    bool checkPlayerDistance()
    {
        return Vector2.Distance(this.transform.position, target.transform.position) < 3;
    }


    bool checkFriendsNear()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach(GameObject z in zombies)
        {
            if((z.transform.position!=this.transform.position)&& Vector2.Distance(this.transform.position, z.transform.position) < 2)
            {
                return true;
            }
        }
        return false;
    }

    bool checkHP()
    {
        if (currentHitPoint > 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool checkSpawnDistance()
    {
        return Vector2.Distance(this.gameObject.transform.position, mySpawnPoint) > 6; 
    }
    
    bool randomAct()
    {
        int temp = Random.Range(0, 10000) % 2;
        return temp == 0;
    }

    void Attack()
    {
        move_Speed = 2f;
        UpdateHeading();
    }

    void Retreat()
    {
        move_Speed = 1f;
        UpdateRetreating();
    }

    void Advance()
    {
        move_Speed = 0.5f;
        UpdateHeading();
    }

    void MoveBack()
    {
        move_Speed = 0.5f;
        MoveToSpawn();
    }

    void RandomWalk()
    {
        Debug.Log("Walk");
        RandomWalking();
    }

    void IdleAnimation()
    {
        Debug.Log("Idle");
        //gameObject.GetComponent<Animator>().SetBool("Idle",true);
    }

    void BuildDecisionTree()
    {
        /******  Decision Nodes  ******/
        DecisionTree isTargetInsideThreatZoneNode = new DecisionTree();
        isTargetInsideThreatZoneNode.SetDecision(checkTargetInsideThreatZone);

        DecisionTree isInRangeNode = new DecisionTree();
        isInRangeNode.SetDecision(checkPlayerDistance);

        DecisionTree isFriendsNearNode = new DecisionTree();
        isFriendsNearNode.SetDecision(checkFriendsNear);

        DecisionTree isHPLowNode1 = new DecisionTree();
        isHPLowNode1.SetDecision(checkHP);

        DecisionTree isHPLowNode2 = new DecisionTree();
        isHPLowNode2.SetDecision(checkHP);

        DecisionTree isTooFarNode = new DecisionTree();
        isTooFarNode.SetDecision(checkSpawnDistance);

        DecisionTree randomNode = new DecisionTree();
        randomNode.SetDecision(randomAct);


        /******  Action Nodes  ******/
        DecisionTree actAttackNode = new DecisionTree();
        actAttackNode.SetAction(Attack);

        DecisionTree actRetreatNode = new DecisionTree();
        actRetreatNode.SetAction(Retreat);

        DecisionTree actAdvanceNode = new DecisionTree();
        actAdvanceNode.SetAction(Advance);

        DecisionTree actMoveBackNode = new DecisionTree();
        actMoveBackNode.SetAction(MoveBack);

        DecisionTree actRandomWalkNode = new DecisionTree();
        actRandomWalkNode.SetAction(RandomWalk);

        DecisionTree actIdleNode = new DecisionTree();
        actIdleNode.SetAction(IdleAnimation);


        /******  Assemble Tree  ******/

        // Is target inside threat zone?
        isTargetInsideThreatZoneNode.SetRight(isInRangeNode);
        isTargetInsideThreatZoneNode.SetLeft(isTooFarNode);

        // Is target in range?
        isInRangeNode.SetRight(isFriendsNearNode);
        isInRangeNode.SetLeft(isHPLowNode1);

        // Is too far from spawn
        isTooFarNode.SetRight(actMoveBackNode);
        isTooFarNode.SetLeft(randomNode);

        // Is friends near?
        isFriendsNearNode.SetRight(actAttackNode);
        isFriendsNearNode.SetLeft(isHPLowNode2);

        // Is HP low?
        isHPLowNode1.SetRight(actRetreatNode);
        isHPLowNode1.SetLeft(actAdvanceNode);

        // A random node
        randomNode.SetRight(actRandomWalkNode);
        randomNode.SetLeft(actIdleNode);

        // Is HP low?
        isHPLowNode2.SetRight(actRetreatNode);
        isHPLowNode2.SetLeft(actAttackNode);

        // tree root 
        root = isTargetInsideThreatZoneNode;
    }

    // follow the player
    void UpdateHeading()
    {
        if (target != null)
        {
            this.transform.position = Vector3.MoveTowards(
                this.transform.position,
                target.transform.position,
                move_Speed * Time.deltaTime
                );

            Vector3 vectorToTarget = target.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotate_Speed);
        }
    }

    // retreating if HP low
    void UpdateRetreating()
    {
        if (target != null)
        {
            this.transform.position =  Vector3.MoveTowards(
                this.transform.position,
                target.transform.position,
                -move_Speed * Time.deltaTime
                );

            Vector3 vectorToTarget = target.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotate_Speed);
        }
    }

    // moving back to Spwan
    void MoveToSpawn()
    {
        this.transform.position = Vector3.MoveTowards(
            this.transform.position,
            mySpawnPoint,
            move_Speed * Time.deltaTime
            );

        Vector3 vectorToTarget = mySpawnPoint - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotate_Speed);

    }

    private float timeToChangeDirection = 0f;
    // random walk
    void RandomWalking()
    {
        Vector3 randomPoint = new Vector3();
        timeToChangeDirection -= Time.deltaTime;
            randomPoint = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);
            timeToChangeDirection = 1.5f;
        randomPoint += this.transform.position;
        this.transform.position = Vector3.MoveTowards(
            this.transform.position,
            randomPoint
            , move_Speed * Time.deltaTime
            );
        
        Vector3 vectorToTarget = randomPoint - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotate_Speed);
    }

    // Hit by bullet
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Bullet")
        {
            currentHitPoint--;
            if (currentHitPoint == 0)
            {
                audioSource.Play();
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
                Destroy(this.gameObject, 3);
                GameObject.Find("UIManager").SendMessage("CountKills");
            }
        }
    }
}
