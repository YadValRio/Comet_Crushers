using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour{
    public static BaseHealth Instance { get; private set; }

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthSlider;

    public float CurrentHealth { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Only one BaseHealth component can be active at a time.", this);
            enabled = false;
            return;
        }

        Instance = this;
        CurrentHealth = maxHealth;

        if (healthSlider == null)
            healthSlider = FindFirstObjectByType<Slider>();

        if (healthSlider == null)
        {
            Debug.LogError("BaseHealth could not find the health bar Slider.", this);
            return;
        }

        healthSlider.minValue = 0f;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = CurrentHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f)
        {
            Debug.LogWarning("Damage must be greater than zero.", this);
            return;
        }

        if (CurrentHealth <= 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        

        if (healthSlider != null)
            healthSlider.value = CurrentHealth;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}