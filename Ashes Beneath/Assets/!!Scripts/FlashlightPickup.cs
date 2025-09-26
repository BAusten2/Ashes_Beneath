using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashlightPickup : MonoBehaviour {
	private Transform myTransform;
	private bool playerInRange = false;
	private bool isLookingAtItem = false;
	
	public GameObject FlashLight;
	public GameObject UIPanel;
	public KeyCode PickupKey = KeyCode.E; // Customizable input key with E as default
	
	// Interaction Prompt Settings
	[Header("Interaction Prompt")]
	public GameObject InteractionPrompt; // UI Text object for prompt
	public string PromptText = "[E] To pick up this item";
	public float InteractionRange = 3f;
	public LayerMask ItemLayerMask = -1;
	
	public AudioClip pickupSound;
	public bool removeOnUse = true;
	
	[Header("Pickup Message")]
	public bool PickupMessage;
	public GameObject MessageLabel; // Changed from private to public
	public string PickupTEXT = "You have picked up a Flashlight";
	public Color PickupTextColor = Color.white;	
	
	void Start () {
		myTransform = transform;
		UIPanel.SetActive(false);
		
		// Hide interaction prompt at start
		if (InteractionPrompt != null) {
			InteractionPrompt.SetActive(false);
		}
	}

	void Update() {
		// Check if player is looking at the flashlight
		CheckPlayerLookingAtItem();
		
		// Check if player is in range and presses the pickup key
		if (isLookingAtItem && Input.GetKeyDown(PickupKey)) {
			UseObject();
		}
	}
	
	void CheckPlayerLookingAtItem() {
		// Find the player's camera (assuming it's tagged as MainCamera or find FirstPersonController camera)
		Camera playerCamera = Camera.main;
		if (playerCamera == null) {
			// Try to find camera in FirstPersonController
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null) {
				playerCamera = player.GetComponentInChildren<Camera>();
			}
		}
		
		if (playerCamera != null) {
			Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			RaycastHit hit;
			
			// Cast ray and check if we hit this specific object
			if (Physics.Raycast(ray, out hit, InteractionRange)) {
				if (hit.collider.gameObject == this.gameObject) {
					// Player is looking at this flashlight
					if (!isLookingAtItem) {
						isLookingAtItem = true;
						ShowInteractionPrompt();
					}
				} else {
					// Player is looking at something else
					if (isLookingAtItem) {
						isLookingAtItem = false;
						HideInteractionPrompt();
					}
				}
			} else {
				// No hit or out of range
				if (isLookingAtItem) {
					isLookingAtItem = false;
					HideInteractionPrompt();
				}
			}
		}
	}
	
	void ShowInteractionPrompt() {
		if (InteractionPrompt != null) {
			InteractionPrompt.SetActive(true);
			Text promptText = InteractionPrompt.GetComponent<Text>();
			if (promptText != null) {
				promptText.text = PromptText;
			}
		}
	}
	
	void HideInteractionPrompt() {
		if (InteractionPrompt != null) {
			InteractionPrompt.SetActive(false);
		}
	}

    public void UseObject (){
		FlashlightScript FlashlightComponent = FlashLight.GetComponent<FlashlightScript>();
		UIPanel.SetActive(true);
		FlashlightComponent.PickedFlashlight = true;
		
		// Hide interaction prompt
		HideInteractionPrompt();
		
		this.GetComponent<Renderer>().enabled = false;
		this.GetComponent<Collider>().enabled = false;
		
         if(pickupSound){AudioSource.PlayClipAtPoint(pickupSound, myTransform.position, 0.75f);}//Main Audio		 
		 if(PickupMessage){StartCoroutine(SendMessage());}
	}
	
 	public IEnumerator SendMessage (){
		if (MessageLabel == null) {
			Debug.LogWarning("MessageLabel is not assigned! Please drag your UI_MessageLabel GameObject to the MessageLabel field.");
			yield break;
		}
		
		Text Message = MessageLabel.GetComponent<Text>();
		if (Message == null) {
			Debug.LogWarning("MessageLabel GameObject doesn't have a Text component!");
			yield break;
		}
		
		/* Message Line */
		Message.enabled = true;
		Message.color = PickupTextColor;
		Message.text = PickupTEXT;
		yield return new WaitForSeconds(3);
		Message.CrossFadeAlpha(0, 2.0f, false);
		yield return new WaitForSeconds(5);
		Message.enabled = false;
	}
}