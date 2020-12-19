using System.Collections;
using GameManagement.FeedbackMenuScripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace FeedbackMenuScripts
{
    public class SendToGoogle : MonoBehaviour
    {
        private string[] _videogames_names = new string[23]
        {
            "AToonWorld",
            "Arend",
            "AstroGolf",
            "Blind Maze",
            "Collage Escape",
            "Devil's Little Helper",
            "Drunk Walking",
            "Escape from Knossos: Together!",
            "Euclid's Nightmare",
            "FurBrawl",
            "Hell's Chicken",
            "Hypodoche",
            "I will find you",
            "Junk Tower",
            "Keep It Lit!",
            "Kiyo and the Unreliable Hero",
            "Laser Golf!",
            "Magnetic Fieldball",
            "Play of Shadows",
            "Rogue Cleaner",
            "Save Tomato",
            "Sunday",
            "Time Flies"
        };

        enum VideoGamesName
        {
            AToonWorld = 0,
            Arend = 1,
            AstroGolf = 2,
            BlindMaze = 3,
            CollageEscape = 4,
            DevilsLittleHelper = 5,
            DrunkWalking = 6,
            EscapeFromKnossosTogether = 7,
            EuclidsNightmare = 8,
            FurBrawl = 9,
            HellsChicken = 10,
            Hypodoche = 11,
            IWillFindYou = 12,
            JunkTower = 13,
            KeepItLit = 14,
            KiyoAndTheUnreliableHero = 15,
            LaserGolf = 16,
            MagneticFieldball = 17,
            PlayOfShadows = 18,
            RogueCleaner = 19,
            SaveTomato = 20,
            Sunday = 21,
            TimeFlies = 22
        };

        [SerializeField] private VideoGamesName Videogame;
        [SerializeField] private Text Feedback;

        public void SendFeedback()
        {
            InputField inputText = Feedback.transform.GetComponentInParent<InputField>();
            string feedback = Feedback.text;
            feedback = inputText.text;
            StartCoroutine(PostFeedback(_videogames_names[(int) Videogame], feedback));
        }

        private IEnumerator PostFeedback(string videogame_name, string feedback)
        {
            // https://docs.google.com/forms/d/e/1FAIpQLSdyQkpRLzqRzADYlLhlGJHwhbKZvKJILo6vGmMfSePJQqlZxA/viewform?usp=pp_url&entry.631493581=Simple+Game&entry.1313960569=Very%0AGood!

            const string commonFeedbackFormURL =
                "https://docs.google.com/forms/d/e/1FAIpQLSdyQkpRLzqRzADYlLhlGJHwhbKZvKJILo6vGmMfSePJQqlZxA/formResponse";
            const string personalFormURL = "https://forms.gle/GPguuZx2UtbBmZos9";

            WWWForm form = new WWWForm();

            form.AddField("entry.631493581", videogame_name);
            form.AddField("entry.1313960569", feedback);

            UnityWebRequest www = UnityWebRequest.Post(commonFeedbackFormURL, form);

            CopyToClipboard(personalFormURL);

            yield return www.SendWebRequest();

            print(www.error);

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }

            MenuManager.Instance.OpenThanksMenu(); 
        }

        private void CopyToClipboard(string stringToCopy) => GUIUtility.systemCopyBuffer = stringToCopy;
    }
}