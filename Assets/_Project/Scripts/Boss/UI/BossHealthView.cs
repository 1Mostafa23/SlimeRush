using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BossHealthView : MonoBehaviour
{
    [SerializeField] private Image hpBarFill;

    private BossCombatant bossCombatant;

    [Inject]
    private void Construct(BossCombatant bossCombatant)
    {
        this.bossCombatant = bossCombatant;
    }

    private void Start()
    {
        ConfigureFillImage();

        if (bossCombatant == null)
        {
            Debug.LogError("BossHealthView: BossCombatant was not injected.");
            return;
        }

        bossCombatant.HpChanged += UpdateHp;
        UpdateHp(bossCombatant.CurrentHp, bossCombatant.MaxHp);
    }

    private void OnDestroy()
    {
        if (bossCombatant != null)
            bossCombatant.HpChanged -= UpdateHp;
    }

    private void UpdateHp(int currentHp, int maxHp)
    {
        if (hpBarFill == null)
            return;

        hpBarFill.fillAmount = maxHp > 0 ? (float)currentHp / maxHp : 0f;
    }

    private void ConfigureFillImage()
    {
        if (hpBarFill == null)
            return;

        hpBarFill.type = Image.Type.Filled;
        hpBarFill.fillMethod = Image.FillMethod.Horizontal;
        hpBarFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        hpBarFill.fillClockwise = true;
    }
}
