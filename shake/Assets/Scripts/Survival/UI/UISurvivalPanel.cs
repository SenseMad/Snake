using UnityEngine;
using UnityEngine.UI;

public class UISurvivalPanel : MonoBehaviour
{
  [Header("Objects")]
  [SerializeField] private GameObject _diamondPanel;
  [SerializeField] private GameObject _energyPanel;

  [Header("Texts")]
  [SerializeField] private Text _diamondCountText;
  [SerializeField] private Text _energyCountText;

  private LevelManager levelManager;
  private SurvivalManager survivalManager;

  public void Initialize()
  {
    levelManager = GameManager.Instance.LevelManager;
  }

  public void SetActive(bool parValue)
  {
    _diamondPanel.SetActive(parValue);
    _energyPanel.SetActive(parValue);

    if (parValue)
    {
      GameManager.Instance.LevelManager.OnInitializeSurivalMode += LevelManager_OnInitializeSurivalMode;

      _diamondCountText.text = "0";
      _energyCountText.text = "0";
    }
    else
    {
      if (survivalManager != null)
      {
        survivalManager.OnChangeDiamond -= UpdateDiamondText;
        survivalManager.OnChangeEnergy -= UpdateEnergyText;

        survivalManager = null;
      }

      GameManager.Instance.LevelManager.OnInitializeSurivalMode -= LevelManager_OnInitializeSurivalMode;
    }
  }

  private void LevelManager_OnInitializeSurivalMode(SurvivalManager obj)
  {
    survivalManager = obj;

    survivalManager.OnChangeDiamond += UpdateDiamondText;
    survivalManager.OnChangeEnergy += UpdateEnergyText;
  }

  private void UpdateEnergyText(int arg1, int arg2)
  {
    if (survivalManager == null)
      return;

    _energyCountText.text = $"{arg1}/{arg2}";
  }

  private void UpdateDiamondText(int parDiamond)
  {
    if (survivalManager == null)
      return;

    _diamondCountText.text = $"{parDiamond}";
  }
}