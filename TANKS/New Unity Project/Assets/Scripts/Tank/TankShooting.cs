using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
	public Image m_ShooterIcon;
	public Slider m_HealthSlider;
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;
	private bool m_ActiveShooter = false;
	private GameObject m_GameManagerObject;
	private GameManager m_GameManager;


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

		m_GameManagerObject = GameObject.Find ("GameManager");
		m_GameManager = m_GameManagerObject.GetComponent<GameManager> ();
    }
    

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
		m_AimSlider.value = m_MinLaunchForce;

		if (m_ActiveShooter) {
			if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
				m_CurrentLaunchForce = m_MaxLaunchForce;
				Fire ();
			} else if (Input.GetButtonDown (m_FireButton)) {
				m_Fired = false;
				m_CurrentLaunchForce = m_MinLaunchForce;

				m_ShootingAudio.clip = m_ChargingClip;
				m_ShootingAudio.Play ();
			} else if (Input.GetButton (m_FireButton) && !m_Fired) {
				m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

				m_AimSlider.value = m_CurrentLaunchForce;
			} else if (Input.GetButtonUp (m_FireButton) && !m_Fired) {
				Fire ();
			}
		}
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
		m_Fired = true;

		Rigidbody shellInstance = Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

		shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

		m_ShootingAudio.clip = m_FireClip;
		m_ShootingAudio.Play ();

		m_CurrentLaunchForce = m_MinLaunchForce;

		m_GameManager.SwitchActiveShooter ();
    }

	public void DeactivateShooter() {
		m_ShooterIcon.enabled = false;
		m_HealthSlider.gameObject.SetActive (false);
		m_ActiveShooter = false;
	}

	public void ActivateShooter() {
		m_ShooterIcon.enabled = true;
		m_HealthSlider.gameObject.SetActive (true);
		m_ActiveShooter = true;
	}
}