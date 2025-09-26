using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatteryPickup : MonoBehaviour {
	private Transform myTransform;	
	private bool isLookingAtItem = false;
	private GameObject player; // Added missing player variable declaration
	
	public bool EnableMessageMax = true;
	public bool Enabled;
	public float BatteryAdd = 0.01f;
	
	// Interaction Settings
	[Header("Interaction Prompt")]
	public GameObject InteractionPrompt; // UI Text object for prompt
	public string PromptText = "[E] To pick up battery";
	public float InteractionRange = 3f;
	public KeyCode PickupKey = KeyCode.E; // Customizable input key with E as default
	
	public AudioClip pickupSound;//sound to play when picking up this item
	
	[Header("Pickup Messages")]
	public GameObject MessageLabel; // Changed from private to public
	public string MaxBatteryText = "You have Max Batteries";
	public Color MaxBatteryTextColor = Color.white;	

	public bool PickupMessage;
	public string PickupTEXT = "Battery +1";
	public Color PickupTextColor = Color.white;	
	
	void Start () {
		myTransform = transform;//manually set transform for efficiency
		
		// Find player reference
		if (player == null) {
			player = GameObject.FindGameObjectWithTag("Player");
			// Remove PlayerController reference since it doesn't exist in your project
		}
	}
	
	void Update() {
		// Check if player is looking at the battery
		CheckPlayerLookingAtItem();
		
		// Check if player is looking and presses the pickup key
		if (isLookingAtItem && Input.GetKeyDown(PickupKey)) {
			UseObject();
		}
	}
	
	void CheckPlayerLookingAtItem() {
		// Find the player's camera
		Camera playerCamera = Camera.main;
		if (playerCamera == null) {
			// Try to find camera in FirstPersonController
			GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
			if (playerObj != null) {
				playerCamera = playerObj.GetComponentInChildren<Camera>();
			}
		}
		
		if (playerCamera != null) {
			Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			RaycastHit hit;
			
			// Cast ray and check if we hit this specific object
			if (Physics.Raycast(ray, out hit, InteractionRange)) {
				if (hit.collider.gameObject == this.gameObject) {
					// Player is looking at this battery
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
	 
	public void UseObject() {
		// Try to find BatteryUI component anywhere in the scene
		BatteryUI BatteryComponent = FindObjectOfType<BatteryUI>();
		if (BatteryComponent == null) {
			Debug.LogError("Could not find BatteryUI component anywhere in the scene! Make sure you have a BatteryUI script attached to a GameObject.");
			return;
		}
		
		// Hide interaction prompt
		HideInteractionPrompt();
		
		if(BatteryComponent.EnableBattery == true){
			Enabled = true;
		}

		if(BatteryComponent.EnableBattery == false){
			Enabled = false;
			if(EnableMessageMax){StartCoroutine(MaxBatteries());}
		}

		if(Enabled){
			if(PickupMessage && MessageLabel != null){StartCoroutine(SendMessage());}
			BatteryComponent.Batteries += BatteryAdd;
			if(pickupSound){AudioSource.PlayClipAtPoint(pickupSound, myTransform.position, 0.75f);}
			this.GetComponent<Renderer>().enabled = false;
			this.GetComponent<Collider>().enabled = false;
		}
	}
	  
 	public IEnumerator SendMessage (){
		if (MessageLabel == null) {
			Debug.LogWarning("MessageLabel is not assigned! Please drag your UI Text GameObject to the MessageLabel field.");
			yield break;
		}
		
		Text Message = MessageLabel.GetComponent<Text>();
		if (Message == null) {
			Debug.LogWarning("MessageLabel GameObject doesn't have a Text component!");
			yield break;
		}
		
		/* Message Line */
		EnableMessageMax = false;
		Message.enabled = true;
		Message.color = PickupTextColor;
		Message.text = PickupTEXT;
		yield return new WaitForSeconds(2);
		Message.enabled = false;
		EnableMessageMax = true;
	}
	
 	public IEnumerator MaxBatteries (){
		if (MessageLabel == null) {
			Debug.LogWarning("MessageLabel is not assigned for Max Battery message!");
			yield break;
		}
		
		Text Message = MessageLabel.GetComponent<Text>();
		if (Message == null) {
			Debug.LogWarning("MessageLabel GameObject doesn't have a Text component!");
			yield break;
		}
		
		/* Message Line */
		if(!Enabled){
			EnableMessageMax = false;
			Message.enabled = true;
			Message.color = MaxBatteryTextColor;
			Message.text = MaxBatteryText;
			yield return new WaitForSeconds(3);
			Message.CrossFadeAlpha(0f, 2.0f, false);
			yield return new WaitForSeconds(4);
			Message.enabled = false;
			EnableMessageMax = true;
		}
	}
}