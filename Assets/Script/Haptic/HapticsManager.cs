using Lofelt.NiceVibrations;
using UnityEngine;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    public void PlaySelectionImpactVibration()
    {
        if(IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
    }

    public void PlayWarningImpactVibration()
    {
        if (IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
    }

    public void PlaySuccessVibration()
    {
        if (IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }

    public void PlayFailureImpactVibration()
    {
        if (IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
    }

    public void PlayRigidImpactVibration()
    {
        if (IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
    }

    public void PlaySoftImpactVibration()
    {
        if (IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
    }

    public void PlayLightImpactVibration()
    {
        if (IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }

    public void PlayMediumImpactVibration()
    {
        if (IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
    }

    public void PlayHeavyImpactVibration()
    {
        if (IdleUIManager.Instance.isVibrationOn == false)
            return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
    }

    public void PlayUIFeedback(string sfxKey = "UI_Click", System.Action hapticAction = null)
    {
        AudioManager.Instance?.PlaySFX(sfxKey);
        hapticAction?.Invoke();
    }
}
