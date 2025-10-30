using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FlashlightPickup : MonoBehaviour {
	private Transform myTransform;
	private bool isLookingAtItem = false;
	private bool hasBeenPickedUp = false;
	
	public GameObject FlashLight;
	public GameObject UIPanel;
	public KeyCode PickupKey = KeyCode.E;
	
	[Header("Interaction Prompt")]
	public GameObject InteractionPrompt;
	public string PromptText = "[E] To pick up this item";
	public float InteractionRange = 3f;
	public LayerMask ItemLayerMask = -1;
	
	public AudioClip pickupSound;
	public bool removeOnUse = true;
	
	[Header("Pickup Message")]
	public bool PickupMessage;
	public GameObject MessageLabel;
	public string PickupTEXT = "You have picked up a Flashlight";
	public Color PickupTextColor = Color.white;	
	
	void Start () {
		myTransform = transform;
		
		if (FlashLight != null) {
			FlashLight.SetActive(false);
		}
		if (UIPanel != null) {
			UIPanel.SetActive(false);
		}
		
		if (InteractionPrompt != null) {
			InteractionPrompt.SetActive(false);
		}
	}

	void Update() {
		if (hasBeenPickedUp) {
			return;
		}
		
		CheckPlayerLookingAtItem();
		
		if (isLookingAtItem && Input.GetKeyDown(PickupKey)) {
			UseObject();
		}
	}
	
	void CheckPlayerLookingAtItem() {
		Camera playerCamera = Camera.main;
		if (playerCamera == null) {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null) {
				playerCamera = player.GetComponentInChildren<Camera>();
			}
		}
		
		if (playerCamera != null) {
			Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, InteractionRange)) {
				if (hit.collider.gameObject == this.gameObject) {
					if (!isLookingAtItem) {
						isLookingAtItem = true;
						ShowInteractionPrompt();
					}
				} else {
					if (isLookingAtItem) {
						isLookingAtItem = false;
						HideInteractionPrompt();
					}
				}
			} else {
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
			
			// Try Legacy Text first
			Text legacyText = InteractionPrompt.GetComponent<Text>();
			if (legacyText != null) {
				legacyText.text = PromptText;
			} else {
				// Try TextMeshPro
				TextMeshProUGUI tmpText = InteractionPrompt.GetComponent<TextMeshProUGUI>();
				if (tmpText != null) {
					tmpText.text = PromptText;
				}
			}
		}
	}
	
	void HideInteractionPrompt() {
		if (InteractionPrompt != null) {
			InteractionPrompt.SetActive(false);
		}
	}

	public void UseObject() {
		if (FlashLight == null) {
			Debug.LogError("FlashLight is not assigned!");
			return;
		}
		
		hasBeenPickedUp = true;
		
		FlashlightScript FlashlightComponent = FlashLight.GetComponent<FlashlightScript>();
		if (FlashlightComponent == null) {
			Debug.LogError("FlashlightScript not found on FlashLight GameObject!");
			return;
		}
		
		FlashLight.SetActive(true);
		
		if (UIPanel != null) {
			UIPanel.SetActive(true);
		}
		
		FlashlightComponent.PickedFlashlight = true;
		
		HideInteractionPrompt();
		isLookingAtItem = false;
		
		if (GetComponent<Renderer>() != null) {
			GetComponent<Renderer>().enabled = false;
		}
		if (GetComponent<Collider>() != null) {
			GetComponent<Collider>().enabled = false;
		}
		
		if (pickupSound) {
			AudioSource.PlayClipAtPoint(pickupSound, myTransform.position, 0.75f);
		}
		
		if (PickupMessage && MessageLabel != null) {
			StartCoroutine(SendMessage());
		}
		
		if (removeOnUse) {
			Destroy(gameObject, 0.5f);
		}
	}
	
	public IEnumerator SendMessage() {
		if (MessageLabel == null) {
			yield break;
		}
		
		// Try Legacy Text first
		Text legacyText = MessageLabel.GetComponent<Text>();
		TextMeshProUGUI tmpText = MessageLabel.GetComponent<TextMeshProUGUI>();
		
		if (legacyText != null) {
			legacyText.enabled = true;
			legacyText.color = PickupTextColor;
			legacyText.text = PickupTEXT;
			yield return new WaitForSeconds(3);
			legacyText.CrossFadeAlpha(0, 2.0f, false);
			yield return new WaitForSeconds(2);
			legacyText.enabled = false;
		} else if (tmpText != null) {
			tmpText.enabled = true;
			tmpText.color = PickupTextColor;
			tmpText.text = PickupTEXT;
			yield return new WaitForSeconds(3);
			tmpText.CrossFadeAlpha(0, 2.0f, false);
			yield return new WaitForSeconds(2);
			tmpText.enabled = false;
		}
	}
}