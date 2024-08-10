using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rigidBody;
	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private AudioSource audioSource;

	private bool isJumping = false;
	private bool isGrounded = true;
	private float horizontalVelocity = 0f;

	[SerializeField] private float speed = 1f;
	[SerializeField] private float jumpForce = 1f;

	public bool isDead = false;
	private bool platformPriority = false;

	[SerializeField] private AudioClip jumpAudioClip;
	[SerializeField] private AudioClip takesDamageAudioClip;
	[SerializeField] private AudioClip diesAudioClip;
	[SerializeField] private AudioClip respawnsAudioClip;
	[SerializeField] private TilemapCollider2D platforms;

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
        horizontalVelocity = Input.GetAxis("Horizontal") * speed;

		if (isJumping == false) 
		{
			if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
			{
				isJumping = true;
				isGrounded = false;
				rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
				animator.SetTrigger("hasJumped");
				PlayAudioClip(jumpAudioClip);
			}
		}

		Vector2 move = transform.TransformDirection(new Vector2(horizontalVelocity, rigidBody.velocity.y));
		rigidBody.velocity = new Vector3(move.x, rigidBody.velocity.y);

		animator.SetFloat("velocity", Mathf.Abs(rigidBody.velocity.x));
		
		FlipX();
		MovingThroughPlatforms();
	}

	void MovingThroughPlatforms()
	{
			if (Input.GetKey(KeyCode.S))
		{
			platforms.enabled = false;
			if (!platformPriority)
				StartCoroutine(DropThroughPlatforms());
		}
		else if (rigidBody.velocity.y > 0.1f)
			platforms.enabled = false;
		else if (!platformPriority)
			platforms.enabled = true;	
	}

	IEnumerator DropThroughPlatforms()
	{
		platformPriority = true;
		yield return new WaitForSeconds(0.3f);
		platformPriority = false;
	}

	void FlipX()
	{
		if (rigidBody.velocity.x < -0.5f)
			spriteRenderer.flipX = true;
		if (rigidBody.velocity.x > 0.5f)
			spriteRenderer.flipX = false;
	}

	public void TakesDamage(int amount)
	{
		if (isDead)
			return;

		GameManager.Instance.gameData.hp -= amount;
		UserInterface.Instance.UpdateUI();

		if (GameManager.Instance.gameData.hp > 0)
		{
			animator.SetTrigger("takesDamage");
			Debug.LogFormat("The caterpillar is hit!\n{0} HP remaining!", GameManager.Instance.gameData.hp);
			PlayAudioClip(takesDamageAudioClip);
		}
		else
		{
			isDead = true;
			animator.SetTrigger("dies");
			Debug.Log("The caterpillar has died!");
			StartCoroutine(Respawn());
			PlayAudioClip(diesAudioClip);
		}
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		isGrounded = true;
		if (isJumping)
		{
			isJumping = false;
			animator.SetTrigger("hasLanded");
		}
	}	

	void OnTriggerExit2D(Collider2D collider)
	{
		isGrounded = false;
	}

	IEnumerator Respawn()
	{
		GameManager.Instance.gameData.deaths += 1;
		GameManager.Instance.Fade();
		enabled = false;
		rigidBody.velocity = Vector2.zero;
		yield return new WaitForSeconds(1f);
		GameManager.Instance.ResetPlayerPos();
		yield return new WaitForSeconds(0.9f);
		spriteRenderer.flipX = false;
		animator.SetTrigger("respawns");
		PlayAudioClip(respawnsAudioClip);
		yield return new WaitForSeconds(1.2f);
		GameManager.Instance.gameData.hp = 3;
		UserInterface.Instance.UpdateUI();
		enabled = true;
		isDead = false;
	}

	void PlayAudioClip(AudioClip clip)
	{
		audioSource.clip = clip;
		audioSource.Play();
	}
}
