
using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        [SerializeField] float fadeInTime = 0.2f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update() {

            // TODO Maybe move these to a command pattern 
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            if(GetComponent<SavingSystem>() != null){
                GetComponent<SavingSystem>().Load(defaultSaveFile);
            }
        }

        public void Save()
        {
            if (GetComponent<SavingSystem>() != null){
            GetComponent<SavingSystem>().Save(defaultSaveFile);
            }
        }

        public void Delete()
        {
            if (GetComponent<SavingSystem>() != null){
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
            }
        }
    }
}