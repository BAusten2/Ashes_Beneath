using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class BatterySpritesClass{
    public Sprite battery_charge_0;   // Empty (red)
    public Sprite battery_charge_1;   // 1 bar
    public Sprite battery_charge_2;   // 2 bars
    public Sprite battery_charge_3;   // 3 bars
    public Sprite battery_charge_4;   // 4 bars
    public Sprite battery_charge_5;   // 5 bars
    public Sprite battery_charge_6;   // 6 bars
    public Sprite battery_charge_7;   // 7 bars
    public Sprite battery_charge_8;   // 8 bars
    public Sprite battery_charge_9;   // 9 bars
    public Sprite battery_charge_10;  // 10 bars
    public Sprite battery_charge_11;  // 11 bars
    public Sprite battery_charge_12;  // 12 bars
    public Sprite battery_charge_13;  // 13 bars
    public Sprite battery_charge_14;  // 14 bars
    public Sprite battery_charge_15;  // 15 bars
    public Sprite battery_charge_16;  // 16 bars
    public Sprite battery_charge_17;  // 17 bars
    public Sprite battery_charge_18;  // 18 bars
    public Sprite battery_full;       // 19 bars (full)
}

public class FlashlightScript : MonoBehaviour {
    public BatterySpritesClass BatterySprites = new BatterySpritesClass();
    public KeyCode FlashlightKey = KeyCode.F;
    public AudioClip ClickSound;
    public float batteryLifeInSec = 300f;
    public float batteryPercentage = 100;
    public GameObject FlashlightSprite;
    public bool PickedFlashlight = false;
    
    public bool on;
    private float timer;
    private Transform myTransform;    

    void Start () {
        myTransform = transform;//manually set transform for efficiency
    }

    void Update() {
        Image BatterySprite = FlashlightSprite.GetComponent<Image>();
        Light lite = this.GetComponent<Light>();
        timer += Time.deltaTime;
        
        if(PickedFlashlight){    
            if(Input.GetKeyDown(FlashlightKey) && timer >= 0.3f && batteryPercentage > 0) {
                on = !on;
                if(ClickSound){AudioSource.PlayClipAtPoint(ClickSound, myTransform.position, 0.75f);}
                timer = 0;
            }
        }    

        if(on) {
            lite.enabled = true;
            batteryPercentage -= Time.deltaTime * (100 / batteryLifeInSec);
        }
        else {
            lite.enabled = false;
        }
    
        batteryPercentage = Mathf.Clamp(batteryPercentage, 0, 100);
    
        // Calculate which battery sprite to show based on 19 bars
        // Each bar represents approximately 5.26% (100/19)
        float barPercentage = 100f / 19f; // ~5.26%
        
        if (batteryPercentage > 18 * barPercentage) // > 94.7%
        {
            BatterySprite.sprite = BatterySprites.battery_full;
            lite.intensity = Mathf.Lerp(lite.intensity, 3f, Time.deltaTime);
        }
        else if (batteryPercentage > 17 * barPercentage) // > 89.5%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_18;
            lite.intensity = Mathf.Lerp(lite.intensity, 2.8f, Time.deltaTime);
        }
        else if (batteryPercentage > 16 * barPercentage) // > 84.2%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_17;
            lite.intensity = Mathf.Lerp(lite.intensity, 2.6f, Time.deltaTime);
        }
        else if (batteryPercentage > 15 * barPercentage) // > 78.9%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_16;
            lite.intensity = Mathf.Lerp(lite.intensity, 2.4f, Time.deltaTime);
        }
        else if (batteryPercentage > 14 * barPercentage) // > 73.7%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_15;
            lite.intensity = Mathf.Lerp(lite.intensity, 2.2f, Time.deltaTime);
        }
        else if (batteryPercentage > 13 * barPercentage) // > 68.4%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_14;
            lite.intensity = Mathf.Lerp(lite.intensity, 2.0f, Time.deltaTime);
        }
        else if (batteryPercentage > 12 * barPercentage) // > 63.2%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_13;
            lite.intensity = Mathf.Lerp(lite.intensity, 1.8f, Time.deltaTime);
        }
        else if (batteryPercentage > 11 * barPercentage) // > 57.9%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_12;
            lite.intensity = Mathf.Lerp(lite.intensity, 1.6f, Time.deltaTime);
        }
        else if (batteryPercentage > 10 * barPercentage) // > 52.6%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_11;
            lite.intensity = Mathf.Lerp(lite.intensity, 1.4f, Time.deltaTime);
        }
        else if (batteryPercentage > 9 * barPercentage) // > 47.4%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_10;
            lite.intensity = Mathf.Lerp(lite.intensity, 1.2f, Time.deltaTime);
        }
        else if (batteryPercentage > 8 * barPercentage) // > 42.1%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_9;
            lite.intensity = Mathf.Lerp(lite.intensity, 1.0f, Time.deltaTime);
        }
        else if (batteryPercentage > 7 * barPercentage) // > 36.8%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_8;
            lite.intensity = Mathf.Lerp(lite.intensity, 0.9f, Time.deltaTime);
        }
        else if (batteryPercentage > 6 * barPercentage) // > 31.6%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_7;
            lite.intensity = Mathf.Lerp(lite.intensity, 0.8f, Time.deltaTime);
        }
        else if (batteryPercentage > 5 * barPercentage) // > 26.3%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_6;
            lite.intensity = Mathf.Lerp(lite.intensity, 0.7f, Time.deltaTime);
        }
        else if (batteryPercentage > 4 * barPercentage) // > 21.1%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_5;
            lite.intensity = Mathf.Lerp(lite.intensity, 0.6f, Time.deltaTime);
        }
        else if (batteryPercentage > 3 * barPercentage) // > 15.8%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_4;
            lite.intensity = Mathf.Lerp(lite.intensity, 0.5f, Time.deltaTime);
        }
        else if (batteryPercentage > 2 * barPercentage) // > 10.5%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_3;
            lite.intensity = Mathf.Lerp(lite.intensity, 0.4f, Time.deltaTime);
        }
        else if (batteryPercentage > 1 * barPercentage) // > 5.3%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_2;
            lite.intensity = Mathf.Lerp(lite.intensity, 0.3f, Time.deltaTime);
        }
        else if (batteryPercentage > 0.5f)
        {
            BatterySprite.sprite = BatterySprites.battery_charge_1;
            lite.intensity = Mathf.Lerp(lite.intensity, 0.2f, Time.deltaTime);
        }
        else // batteryPercentage <= 0.5%
        {
            BatterySprite.sprite = BatterySprites.battery_charge_0;
            lite.intensity = Mathf.Lerp(lite.intensity, 0, Time.deltaTime * 2);
        }
    }
}