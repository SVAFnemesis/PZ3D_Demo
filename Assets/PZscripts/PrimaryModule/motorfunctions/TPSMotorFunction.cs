using UnityEngine;

public class TPSMotorFunction : MonoBehaviour
{
    public ObstacleDetector ObstacleDetector;
    public float characterRadius = 0.2f;
    public float characterHeight = 1.6f;

    public float baseMovementSpeed;
    public float walkFactor;
    public float strafFactor;
    public float backFactor;
    public float sprintFactor;//sprint is a global factor
    public float jumpFactor;
    public float crouchFactor;
    public float proneFactor;
    public float spinFactor;

    public ScreenDebugger sdb;
    //----------//
    [Header("Following are Utility Flags")]
    [Header("Public Flags")]
    public bool AllMovementDisabled;
    //public GameObject marker;
    [Header("Jump Crouch Flags")]
    public float jumpTick;
    public bool isJumping;
    public float crouchTick;
    [Header("Obstacle Flags")]
    //---obstacle flags---//
    public bool ObstDetected;
    public bool ObstInProgress;
    public string ObstLoggedType;
    public Transform ObstLoggedWallRoot;
    public Vector3 ObstVectorCache1;
    public Vector3 ObstVectorCache2;
    public bool ObstIsSnapped;
    public bool ObstIsSnapping;
    public float ObstScalingTick;
    public bool ObstIsFlopping;
    public int ObstSide;
    public bool ObstIsVaulting;
    public Vector3 ObstVaultStart;
    public Vector3 ObstVaultDestiny;
    public float ObstVaultingTick;

    //---private section---//
    private CharacterController CharCon;
    private BipedCalculator BipCal;
    private ObstacleHandler ObstHandler;
    private Transform TPSNode;


    private void Awake()
    {
        TPSNode = gameObject.transform;
        AllMovementDisabled = false;
        jumpTick = 0f;
        crouchTick = 1f;
        isJumping = false;
        ObstDetected = false;
        ObstInProgress = false;
        ObstIsSnapped = false;
        ObstIsFlopping = false;
        ObstIsVaulting = false;
        ObstScalingTick = 0f;
        ObstVaultingTick = 0f;
    }

    private void Start()//dependent loadup
    {
        CharCon = gameObject.GetComponent<CharacterController>();
        BipCal = new BipedCalculator();
        ObstHandler = new ObstacleHandler();
    }

    #region MOTOR FUCNTION
    public void RunMotorFunction(PlayerControlModule PCM)
    {
        if (AllMovementDisabled == false)//any condition that should disbable main motorfucntion, set the allmovementdisabled flag
        {
            // primary movement input
            Vector3 movement = BipCal.BipedMovementHandler(this, CharCon, PCM.m_walkState, PCM.m_strafState, PCM.m_standState, PCM.m_forwardState, sdb);
            CharCon.SimpleMove(movement);

            // jump input
            //Vector3 jump = BipCal.BipedJumpHandler(this, CharCon, PCM.m_walkState, PCM.m_strafState, PCM.m_standState);
            //TPSNode.position += jump;

            // crouch input
            BipCal.CrouchHandler(this, CharCon, PCM.m_standState);

            //face the camera direction at need
            Quaternion faceToward = BipCal.Forwardhanlder(this, PCM.m_forwardState);
            TPSNode.rotation = faceToward;
        }
    }
    #endregion
    /// <summary>
    /// This is in MCU's update
    /// </summary>
    /// <param name="PCM"></param>
    public void ObstacleInitiator(PlayerControlModule PCM)
    {
        if (ObstacleDetector.loggedWall)//This is a public flag queried by other functions. Inevitably we need to know in runtime if an obstacle is within reach
        {
            ObstDetected = true;
        }
        else
        {
            ObstDetected = false;
        }

        if (ObstacleDetector.loggedWall && AllMovementDisabled == false && PCM.m_standState == 1)//obstacle in progress main flag: there's a logged wall, not movement impaired, jump issued
        {// upon obstacle successfully triggered, log the current wall and save it
            ObstInProgress = true;//MAIN FLAG!!!
            ObstLoggedType = ObstacleDetector.type;//logging wall type for the following sequence
            ObstLoggedWallRoot = ObstacleDetector.wallRoot;//logging the wall info
            isJumping = false;// stop any in progress jump
            jumpTick = 0;
        }//preparing for obstacle course

        if (ObstInProgress == true)//this will loop by progress flag
        {
            AllMovementDisabled = true;
            if (ObstLoggedType == "scale")//which type of obstacle course?
            {
                ObstHandler.ScalingSequence(TPSNode, ObstLoggedWallRoot, CharCon, PCM.m_standState, PCM.m_walkState);
            }
            if (ObstLoggedType == "vault")//which type of obstacle course?
            {
                ObstHandler.VaultSequence(TPSNode, ObstLoggedWallRoot);
            }
            //if no match, bypass, no flag has been touched
        }

        if (ObstInProgress == false)//make sure movement is enable when obstacle isn't in progress
        {
            AllMovementDisabled = false;
        }
    }

    public Transform GetTPS()
    {
        return gameObject.transform;
    }
}