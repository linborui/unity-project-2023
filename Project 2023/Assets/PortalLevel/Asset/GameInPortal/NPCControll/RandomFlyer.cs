using UnityEngine;

public class RandomFlyer : MonoBehaviour,IEndGameObserver
{
    public PortalPlaceSet Setting;
    public float speed = 10f;
    public float minHeight = 2f;
    public float maxHeight = 6f;
    public float randomRange = 2f;
    public float maxOffsetAngle = 15f;
    protected GameObject attackTarget;
    public Transform startPointTran;
    private Vector3 startPoint;
    public Transform endPointTran;
    private Vector3 endPoint;
    private float targetHeight;
    private Vector3 targetPoint;
    private Vector3 targetDirection;
    private float sightRadius = 100f;
    int countSec;

    public Transform launchPoint;
    public GameObject projectile;
    private float launchVelocity = 50f;
    int countsec = 2;
    void Awake(){
        Setting = FindObjectOfType<PortalPlaceSet>();
    }
    private void Start()
    {
        countSec = 0;
        startPoint = startPointTran.position;
        endPoint = endPointTran.position;//new Vector3(Random.Range(-20f, 20f), Random.Range(5f, 10f), Random.Range(-20f, 20f));
        targetPoint = endPoint;
        InvokeRepeating("count",0,15);
        
        //targetHeight = Random.Range(minHeight, maxHeight);
        //targetPoint.y = targetHeight;
        targetDirection = (targetPoint - transform.position).normalized;

        GameManager.Instance.AddObserver(this);
    }
    void OnEnable(){
        InvokeRepeating("Counting",1f,1f);
    }
    void Counting(){
        countsec--;
    }
    void OnDisable(){//銷毀時調用
        if(!GameManager.isInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }
    void count(){
        if (targetPoint == endPoint)
        {
            targetPoint = startPoint;
        }
        else
        {
            targetPoint = endPoint;
        }
        targetDirection = (targetPoint - transform.position).normalized;
    }
    private void Update()
    {
        if(Setting.state !=3 )return;
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

        if (distanceToTarget < 0.1f)
        {
            if (targetPoint == endPoint)
            {
                targetPoint = startPoint;
            }
            else
            {
                targetPoint = endPoint;
            }
            //targetHeight = Random.Range(minHeight, maxHeight);
            //targetPoint.y = targetHeight;
        }
        else
        {
            Vector3 randomOffset = new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
            targetPoint += randomOffset;
            targetHeight = Random.Range(minHeight, maxHeight);
            targetPoint.y = targetHeight;
            targetDirection = (targetPoint - transform.position).normalized;
            float rotationSpeed = Mathf.Min(1f, Vector3.Angle(transform.forward, targetDirection) / 45f);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion offsetRotation = Quaternion.Euler(Random.Range(-maxOffsetAngle, maxOffsetAngle), Random.Range(-maxOffsetAngle, maxOffsetAngle), Random.Range(-maxOffsetAngle, maxOffsetAngle));
            targetRotation *= offsetRotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 180f);

            transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
        }
        if(FoundPlayer()&&countsec<0){
            var _projectile = Instantiate(projectile, 
                                        launchPoint.position,
                                        launchPoint.rotation);
            _projectile.GetComponent<Rigidbody>().velocity = (attackTarget.transform.position - transform.position).normalized * launchVelocity;
            countsec = 5;
        }
    }
    bool FoundPlayer(){ //找到玩家
        
        var colliders = Physics.OverlapSphere(transform.position,sightRadius); //物理球體範圍是否有碰撞體
        foreach(var target in colliders){
            if(target.CompareTag("Player")){
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    public void EndNotify()
    {

    }
}
