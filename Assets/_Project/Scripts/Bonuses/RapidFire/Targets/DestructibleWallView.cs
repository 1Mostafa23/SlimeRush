using TMPro;
using UnityEngine;
using Zenject;

public class DestructibleWallView : MonoBehaviour, IDestructibleTarget
{
    [SerializeField] private DestructibleWallSettings settings;
    [SerializeField] private int maxHp = 30;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private GameObject rootToDisable;
    [SerializeField] private bool damageSlimesOnContact = true;

    private int currentHp;

    public bool IsDestroyed => currentHp <= 0;
    public bool DamageSlimesOnContact => settings != null ? settings.DamageSlimesOnContact : damageSlimesOnContact;

    [Inject]
    private void Construct(SlimeDamageApplier slimeDamageApplier)
    {
    }

    private void Awake()
    {
        if (rootToDisable == null)
            rootToDisable = gameObject;

        currentHp = settings != null ? settings.MaxHp : Mathf.Max(1, maxHp);
        UpdateHpText();
    }

    public void TakeDamage(int amount)
    {
        if (IsDestroyed || amount <= 0)
            return;

        currentHp = Mathf.Max(0, currentHp - amount);
        UpdateHpText();

        if (IsDestroyed)
            Break();
    }

    private void Break()
    {
        if (rootToDisable != null)
            rootToDisable.SetActive(false);
    }

    private void UpdateHpText()
    {
        if (hpText != null)
            hpText.text = currentHp.ToString();
    }
}
