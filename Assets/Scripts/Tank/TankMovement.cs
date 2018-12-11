using UnityEngine;
using UnityEngine.Networking;
using GeekGame.Input;//触控摇杆必须要有的！！！！

public class TankMovement : NetworkBehaviour
{
    public int m_PlayerNumber = 1;                // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    public int m_LocalID = 1;
    public float m_Speed = 12f;                   // How fast the tank moves forward and back.
    public float m_TurnSpeed = 180f;              // How fast the tank turns in degrees per second.
    public float m_PitchRange = 0.2f;             // The amount by which the pitch of the engine noises can vary.
    public AudioSource m_MovementAudio;           // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip m_EngineIdling;              // Audio to play when the tank isn't moving.
    public AudioClip m_EngineDriving;             // Audio to play when the tank is moving.
    public ParticleSystem m_LeftDustTrail;        // The particle system of dust that is kicked up from the left track.
    public ParticleSystem m_RightDustTrail;       // The particle system of dust that is kicked up from the rightt track.
    public Rigidbody m_Rigidbody;              // Reference used to move the tank.
    
    private string m_MovementAxis;              // The name of the input axis for moving forward and back.
    private string m_TurnAxis;                  // The name of the input axis for turning.
    private float m_MovementInput;              // The current value of the movement input.
    private float m_TurnInput;                  // The current value of the turn input.
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void Start()//讲道理这一段应该是用来初始化的
    {
        // The axes are based on player number.==========cut了两句！
        m_MovementAxis = "Vertical" + (m_LocalID + 1);
        m_TurnAxis = "Horizontal" + (m_LocalID + 1);

        // Store the original pitch of the audio source.
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        //========================================================后面两行是非触控操作！！！！！！！！ Store the value of both input axes.
        m_MovementInput = Input.GetAxis(m_MovementAxis);
        m_TurnInput = Input.GetAxis(m_TurnAxis);
		//========================================================下面是触控操作
		//transform.Translate(new Vector3(JoystickMove.instance.H,0f,JoystickMove.instance.V)*speed*Time.deltaTime);

		//transform.LookAt(transform.position+new Vector3(JoystickRotate.instance.H,0f,JoystickRotate.instance.V));

		//if (JoystickFire.instance.Fire) {
		//	Debug.Log ("fire");
		//}
        EngineAudio();
    }


    private void EngineAudio()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs(m_MovementInput) < 0.1f && Mathf.Abs(m_TurnInput) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                // ... change the clip to idling and play it.
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            // Otherwise if the tank is moving and the idling clip is currently playing...
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                // ... change the clip to driving and playing.
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }
	/*=============================================================这里是触控输入的参考！！！！！！！！！再乱撸这段代码吃屎！！
	using UnityEngine;
using System.Collections;
using GeekGame.Input;

public class CubeControl : MonoBehaviour {

	public float speed=.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(new Vector3(JoystickMove.instance.H,0f,JoystickMove.instance.V)*speed*Time.deltaTime);


		transform.LookAt(transform.position+new Vector3(JoystickRotate.instance.H,0f,JoystickRotate.instance.V));

		if(JoystickFire.instance.Fire){
			Debug.Log("fire");
		}
	}
}
*/

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
		//这句可以删
		//Vector2 movement= JoystickMove.GetPosition("movement");
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Move();
        Turn();
    }


    private void Move()
    {
        // Create a movement vector based on the input, speed and the time between frames, in the direction the tank is facing.
        Vector3 movement = transform.forward * m_MovementInput * m_Speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }
    //=====下面是触控操作
    //transform.Translate(new Vector3(JoystickMove.instance.H,0f,JoystickMove.instance.V)*m_Speed*Time.deltaTime);
    //transform.LookAt(transform.position+new Vector3(JoystickRotate.instance.H,0f,JoystickRotate.instance.V));

   
     


    //移动摇杆结束  
    void OnJoystickMoveEnd(MovingJoystick move)
    {
        //停止时，角色恢复idle  
        if (move.joystickName == "MoveJoystick")
        {

        }
    }


    //移动摇杆中  
    void OnJoystickMove(MovingJoystick move)
    {
        if (!isLocalPlayer)
            return;
        if (move.joystickName != "MoveJoystick")
        {
            return;
        }

        //获取摇杆中心偏移的坐标  
        float joyPositionX = move.joystickAxis.x;
        float joyPositionY = move.joystickAxis.y;


        if (joyPositionY != 0 || joyPositionX != 0)
        {
            //设置角色的朝向（朝向当前坐标+摇杆偏移量）  
            transform.LookAt(new Vector3(transform.position.x + joyPositionX, transform.position.y, transform.position.z + joyPositionY));
            //移动玩家的位置（按朝向位置移动）  
            transform.Translate(Vector3.forward * Time.deltaTime * m_Speed, Space.Self);
            //播放奔跑动画  

        }
    }

    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInput * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion inputRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * inputRotation);
    }


    // This function is called at the start of each round to make sure each tank is set up correctly.
    public void SetDefaults()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;

        m_MovementInput = 0f;
        m_TurnInput = 0f;

        m_LeftDustTrail.Clear();
        m_LeftDustTrail.Stop();

        m_RightDustTrail.Clear();
        m_RightDustTrail.Stop();
    }

    public void ReEnableParticles()
    {
        m_LeftDustTrail.Play();
        m_RightDustTrail.Play();
    }

    //We freeze the rigibody when the control is disabled to avoid the tank drifting!
    protected RigidbodyConstraints m_OriginalConstrains;
    void OnDisable()
    {
        m_OriginalConstrains = m_Rigidbody.constraints;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        EasyJoystick.On_JoystickMove -= OnJoystickMove;
        EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
    }

    void OnEnable()
    {
        EasyJoystick.On_JoystickMove += OnJoystickMove;
        EasyJoystick.On_JoystickMoveEnd += OnJoystickMoveEnd;
        m_Rigidbody.constraints = m_OriginalConstrains;
    }
}
