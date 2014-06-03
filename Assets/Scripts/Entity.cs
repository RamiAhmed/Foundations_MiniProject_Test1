using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {

	#region PUBLIC_VARIABLES

	public string Name = "Entity",
				 Class = "Entity";
	
	[Range(0, 100)]
	public float MinimumDamage = 0f;
	
	[Range(1, 100)]
	public float MaximumDamage = 1f;
	
	[Range(0, 100)]
	public float MinimumHealPower = 0f;
	
	[Range(1, 100)]
	public float MaximumHealPower = 0f;
	
	[Range(0f, 1f)]
	public float Accuracy = 0.5f;
	
	[Range(0f, 1f)]
	public float Evasion = 0.5f;
	
	[Range(0, 100)]
	public float Armor = 1f;
	
	[Range(10, 1000)]
	public float MaxHitPoints = 100f;
	
	[HideInInspector]
	public float CurrentHitPoints = 100f;
	
	[Range(0, 1000)]
	public float MovementSpeed = 2f;
	
	[Range(0, 100)]
	public float PerceptionRange = 10f;
	
	[Range(0, 100)]
	public float AttackingRange = 2f;
	
	[Range(0f, 10f)]
	public float AttacksPerSecond = 1f;
	
	[Range(0f, 1f)]
	public float FleeThreshold = 0.1f;
	
	[Range(0f, 100f)]
	public float MoralePointsPerSecond = 0.01f; 

	[Range(0f, 10f)]
	public float HitPointsPerSecond = 0.05f;
	
	public string WalkAnimation = "", 
				  AttackAnimation = "";
	
	public List<AudioClip> 
		AttackSounds = new List<AudioClip>(), 
		DeathSounds = new List<AudioClip>(), 
		BeingHitSounds = new List<AudioClip>();

	public Texture2D HealthbarTexture = null;
	public Texture2D MoraleBarTexture = null;
	
	[HideInInspector]
	public bool Selected = false,
				IsDead = false;
	
	[HideInInspector]
	public Entity lastAttacker = null,
				  attackTarget = null;

	#endregion

	#region PRIVATE_AND_PROTECTED_VARIABLES	
	
	protected float lastAttack = 0f;
	protected float meleeDistance = 5f;
	protected float moraleLevel = 100f;
	protected GameController _gameController;
	protected Camera _camRef = null;

	//private Vector3 targetPosition = Vector3.zero;
	private Animation animation;	
	private Dictionary<string, AudioSource> audioSources;
	private bool isMoving = false;	
	private float lastMoraleRegenerate = 0f;
	private float lastHPRegenerate = 0f;
	private float maxMoraleLevel = 0f;

	#endregion

	#region PRIVATE_METHODS 
	
	void OnGUI() {
		if (_gameController.CurrentState == GameController.GameState.PLAYING || _gameController.CurrentState == GameController.GameState.PAUSED) {
			if (HealthbarTexture != null) {
				if (!this.IsDead) {
					float width = 75f, height = 25f;
					
					Vector3 healthBarPos = _camRef.WorldToScreenPoint(new Vector3(0f, 1.4f, 0f) + this.transform.position);
					float barWidth = width * (this.CurrentHitPoints / this.MaxHitPoints);
					
					GUI.BeginGroup(new Rect(healthBarPos.x - (width/2f), Screen.height - healthBarPos.y - (height/2f), barWidth, height));
					GUI.DrawTexture(new Rect(0f, 0f, width, height), HealthbarTexture, ScaleMode.StretchToFill);
					GUI.EndGroup();
				}
			}
			else {
				Debug.LogWarning("Healthbar texture has not been added to " + this.Name);
			}

			if (MoraleBarTexture != null) {
				if (!this.IsDead) {
					float width = 75f, height = 25f;
					
					Vector3 moraleBarPos = _camRef.WorldToScreenPoint(new Vector3(0f, 2.25f, 0f) + this.transform.position);
					float barWidth = width * (this.moraleLevel / this.maxMoraleLevel);
					
					GUI.BeginGroup(new Rect(moraleBarPos.x - (width/2f), Screen.height - moraleBarPos.y - (height/2f), barWidth, height));
					GUI.DrawTexture(new Rect(0f, 0f, width, height), MoraleBarTexture, ScaleMode.StretchToFill);
					GUI.EndGroup();
				}
			}

			float nameWidth = 75f, nameHeight = 25f;
			Vector3 textPos = _camRef.WorldToScreenPoint(new Vector3(0f, 3f, 0f) + this.transform.position);
			GUI.Label(new Rect(textPos.x - (nameWidth/2f), Screen.height - textPos.y - (nameHeight/2f), nameWidth, nameHeight), this.Name);
		}
	}

	#endregion

	void Awake() {
		_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

		animation = this.GetComponent<Animation>();
		if (animation == null) {
			animation = this.GetComponentInChildren<Animation>();
		}
		
		audioSources = new Dictionary<string, AudioSource>();

		if (AttackSounds.Count > 0)
			addAudioSource("Attack", AttackSounds);
		if (BeingHitSounds.Count > 0) 
			addAudioSource("BeingHit", BeingHitSounds);

		_camRef = Camera.main;
		if (_camRef == null)
			_camRef = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

		if (_camRef == null)
			Debug.LogWarning("Could not identify camera.");

		this.CurrentHitPoints = this.MaxHitPoints;
		this.maxMoraleLevel = this.MaxHitPoints;
		this.moraleLevel = this.maxMoraleLevel;
	}

	#region VIRTUAL_METHODS 

	// Use Start for initialization
	protected virtual void Start() {}
	
	// Update is called once per frame
	protected virtual void Update() {
		if (animation != null) {
			if (isMoving) {
				if (!animation.IsPlaying(GetWalkAnimation())) {
					animation.Play(GetWalkAnimation());
				}
			}
		}

		if (_gameController.CurrentState == GameController.GameState.PLAYING) {
			if (_gameController.GameTime - this.lastMoraleRegenerate > 1f) {
				this.lastMoraleRegenerate = _gameController.GameTime;

				if (this.moraleLevel + this.MoralePointsPerSecond <= this.maxMoraleLevel)
					this.moraleLevel += this.MoralePointsPerSecond;

				if (this.CurrentHitPoints + this.HitPointsPerSecond <= this.MaxHitPoints) 
					this.CurrentHitPoints += this.HitPointsPerSecond;
			}
		}

	}

	// FixedUpdate is called once per time step
	protected virtual void FixedUpdate() {}


	#endregion
	

	#region PUBLIC_METHODS
	
	public bool GetIsWithinPerceptionRange(Entity target) {
		return GetIsWithinRange(target, PerceptionRange);	
	}
	
	public bool GetIsWithinAttackingRange(Entity target) {
		return GetIsWithinRange(target, AttackingRange);
	}
	
	public bool GetIsWithinRange(Entity target, float range) {
		return target != null && Vector3.Distance(target.transform.position, this.transform.position) < range;
	}

	/*public float GetDamage(bool bAverage) {
		return bAverage ? (MaximumDamage - MinimumDamage)/2f + MinimumDamage : GetDamage();
	}*/
	
	public float GetDamage() {
		return Random.Range(MinimumDamage, MaximumDamage);
	}

	public void MoveTo(Transform target) {
		MoveTo(target.position);
	}

	public void MoveTo(Vector3 position) {
		if (position.sqrMagnitude > 0f) {

			Vector3 direction = (position - this.transform.position).normalized;
			Vector3 speed = direction * Time.deltaTime * MovementSpeed;

			//this.transform.Translate(speed);
			this.transform.position = speed + this.transform.position;
			this.transform.LookAt(position);

			if (!isMoving)
				isMoving = true;
		}
	}

	public void Flee() {
		if (this.attackTarget != null && !this.attackTarget.IsDead) {
			Vector3 direction = (-(this.attackTarget.transform.position - this.transform.position)).normalized;
			Vector3 speed = direction * Time.deltaTime * MovementSpeed * 2f;
			
			//this.transform.Translate(speed);
			this.transform.position = speed + this.transform.position;
			this.transform.LookAt(direction);
			
			if (!isMoving)
				isMoving = true;
		}
	}

	public void ReceiveDamage(float damage) {
		if (damage <= 0f) {
			Debug.LogWarning("Receive Damage cannot damage 0 or less");
			return;
		}
		
		//float moraleDamage = (this.GetD20()/20f) < FleeThreshold ? damage : 0f;
		float moraleDamage = damage;
		if (this.moraleLevel - moraleDamage >= 0f) {
			this.moraleLevel -= moraleDamage;
		}
		
		this.CurrentHitPoints -= damage;
		if (this.CurrentHitPoints <= 0f) {
			PlayRandomDeathSound();
			this.IsDead = true;
		}
	}

	public bool GetShouldFlee() {
		return this.moraleLevel / this.maxMoraleLevel < FleeThreshold;
	}

	public void StopAllAnimations() {
		if (animation != null) {
			animation.Stop();
			// TODO Find nicer solution
		}
	}

	public void StopMoving() {
		if (isMoving) 
			isMoving = false;
	}

	#endregion

	#region PROTECTED_METHODS

	protected virtual bool Attack(Entity opponent) {
		bool hitResult = false;
		
		if (opponent.IsDead || opponent == null) {
			attackTarget = null;
		}
		/*else if (GetIsAlly(opponent)) {
			attackTarget = null;
			Debug.LogWarning(this.Name + " tried to attack ally " + opponent);
		}*/
		else {
			StopMoving();
			//lookAtTarget(opponent.transform.position);
			this.transform.LookAt(opponent.transform.position);
			
			float currentTime = Time.time;
			if (currentTime - lastAttack > 1f/AttacksPerSecond) {
				lastAttack = currentTime;
				
				/*if (Bullet != null) {
					ShootBullet(opponent);
				}*/
				
				float accuracy = this.Accuracy + Random.value;
				accuracy = accuracy > 1f ? 1f : accuracy;
				
				float evasion = opponent.Evasion + Random.value;
				evasion = evasion > 1f ? 1f : evasion;
				
				if (accuracy > evasion) {
					float damage = (GetDamage() - opponent.Armor);
					damage = damage < 1f ? 1f : damage;
					opponent.ReceiveDamage(damage);
					opponent.PlayRandomBeingHitSound();
					hitResult = true;
					Debug.Log(_gameController.GameTime + ": " + this.Name + " hit " + opponent.Name + " with " + damage.ToString() + " damage");
				}
				else {
					Debug.Log(_gameController.GameTime + ": " + this.Name + " missed " + opponent.Name);
				}
				
				//this.attackCount++;
				//opponent.attackedCount++;
				
				if (opponent.lastAttacker == null) {
					opponent.lastAttacker = this;
				}

				if (this.attackTarget != opponent) 
					this.attackTarget = opponent;
				
				if (animation != null) {
					animation.Play(GetAttackAnimation());
				}		
				
				PlayRandomAttackSound();
			}
		}
		return hitResult;
	}

	protected string GetWalkAnimation() {
		return WalkAnimation;
	}
	
	protected string GetAttackAnimation() {
		return AttackAnimation;	
	}

	protected int GetD20() {
		return Random.Range(1, 21);
	}
	
	protected float fGetD20() {
		return Random.Range(1f, 20f);
	}

	#endregion

	#region SOUND_METHODS

	private void addAudioSource(string type, List<AudioClip> sounds) {
		if (sounds.Count > 0) {
			audioSources.Add(type, this.gameObject.AddComponent<AudioSource>());
			audioSources[type].playOnAwake = false;
		}
	}

	public void PlayRandomAttackSound() {
		playRandomSound(AttackSounds, "Attack");
	}
	
	public void PlayRandomBeingHitSound() {
		playRandomSound(BeingHitSounds, "BeingHit");
	}
	
	public void PlayRandomDeathSound() {
		if (DeathSounds.Count > 0) {
			AudioClip sound = DeathSounds.Count > 1 ? DeathSounds[Random.Range(0, DeathSounds.Count)] : DeathSounds[0];
			AudioSource.PlayClipAtPoint(sound, this.transform.position);
		}
	}
	
	private void playRandomSound(List<AudioClip> sounds, string type) {
		if (audioSources.ContainsKey(type)) {
			if (sounds.Count > 0) {
				if (!audioSources[type].isPlaying) {
					AudioClip sound = sounds.Count > 1 ? sounds[Random.Range(0, sounds.Count)] : sounds[0];
					audioSources[type].clip = sound;
					audioSources[type].Play();
				}
			}
		}
	}

	#endregion
}
