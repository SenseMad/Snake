public class Button_ReturnToBase : ButtonBehaviour
{
  protected override void OnClick()
  {
	LevelManager.instance.TryLoadLevel(0);

	LevelManager.instance.SetSurvivalMode(false);
  }
}
