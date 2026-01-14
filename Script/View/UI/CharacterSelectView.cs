using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Reflex.Scripts.Attributes;

namespace Game.ECS
{
    public class CharacterSelectView : MonoBehaviour
    {
        [Inject] ProjectContextModel projectContextModel;
        [Inject] TableDataHolder tableDataHolder;

        public Button kilonButton;
        public Button everitButton;
        public Button yeonButton;

        private void Start()
        {
            kilonButton.OnClickAsObservable().Subscribe(_ =>
            {
                SelectCharacter("1");
            }).AddTo(this);

            everitButton.OnClickAsObservable().Subscribe(_ =>
            {
                SelectCharacter("2");
            }).AddTo(this);

            yeonButton.OnClickAsObservable().Subscribe(_ =>
            {
                SelectCharacter("3");
            }).AddTo(this);
        }

        private void Update()
        {
            string id = "";
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                id = "1";
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                id = "2";
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                id = "3";

            if (string.IsNullOrEmpty(id) == false)
                SelectCharacter(id);
        }

        public void SelectCharacter(string id)
        {
            projectContextModel.selectedCharacterId = id;

            SceneManager.LoadScene("TilemapTest");
        }
    }

}
