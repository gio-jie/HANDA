using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections;
    using UnityEngine.SceneManagement; 

    public class Level8Part2Manager : MonoBehaviour
    {
        public static Level8Part2Manager instance;

        [Header("Timer Settings")]
        public float timeLimit = 120f; 
        public float penaltyTime = 5f;

        [Header("UI Feedback")]
        public TMP_Text timerTextUI;
        public GameObject checkIcon;
        public GameObject xIcon;
        
        [Header("Penalty Animation")]
        public TMP_Text penaltyTextUI;
        public float fallSpeed = 50f;
        public float fadeDuration = 1f;
        private Vector3 penaltyOriginalPos;
        
        // --- BAGONG DAGDAG: TAGATANDA NG KULAY ---
        private Color originalTimerColor; 
        // -----------------------------------------

        [Header("Slide Transitions")]
        public RectTransform[] patientUIs; 
        public RectTransform[] tableUIs;   
        public float slideDuration = 1f;   
        private int currentPatientIndex = 0;

        [Header("Scene Transition (Fade In)")]
        public Image fadeOverlay; // Ang itim na screen na liliwanag
        
        private float offScreenRight = 2500f; 
        private float offScreenLeft = -2500f;

        [Header("Star System")]
        public float goldStarThreshold = 60f;   // 3 Stars pag may natira pang 1 min (60s)
        public float silverStarThreshold = 30f; // 2 Stars pag may natira pang 30s

        [Header("UI Panels")]
        public GameObject winPanel;
        public GameObject losePanel; 
        public GameObject pausePanel;
        
        [Header("Win Panel Elements")]
        public Image star1; public Image star2; public Image star3;
        public TMP_Text timeFinishedText; 
        
        [Header("Lose Panel Elements")]
        public Image loseStar1; public Image loseStar2; public Image loseStar3;

        public Color earnedColor = Color.yellow;
        public Color missingColor = Color.gray;

        [HideInInspector] public bool isGameActive = true;

        void Awake() 
        { 
            instance = this; 
            Time.timeScale = 1; 

            // --- BAGONG DAGDAG: I-SAVE ANG KULAY MULA SA INSPECTOR ---
            if (timerTextUI != null)
            {
                originalTimerColor = timerTextUI.color; 
            }
        }

        void Start()
        {
            if (penaltyTextUI != null)
            {
                penaltyOriginalPos = penaltyTextUI.rectTransform.localPosition;
                penaltyTextUI.gameObject.SetActive(false);
            }
            
            if(checkIcon) checkIcon.SetActive(false);
            if(xIcon) xIcon.SetActive(false);

            // I-setup ang mga pasyente
            for (int i = 0; i < patientUIs.Length; i++)
            {
                if (patientUIs[i] == null || tableUIs[i] == null) continue;

                if (i == 0)
                {
                    patientUIs[i].anchoredPosition = new Vector2(0, patientUIs[i].anchoredPosition.y);
                    tableUIs[i].anchoredPosition = new Vector2(0, tableUIs[i].anchoredPosition.y);
                }
                else
                {
                    patientUIs[i].anchoredPosition = new Vector2(offScreenRight, patientUIs[i].anchoredPosition.y);
                    tableUIs[i].anchoredPosition = new Vector2(offScreenRight, tableUIs[i].anchoredPosition.y);
                }
            }

            // SIMULAN ANG FADE IN
            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                Color c = fadeOverlay.color;
                c.a = 1f; // Solid Black sa simula
                fadeOverlay.color = c;
                StartCoroutine(FadeInRoutine());
            }
        }

        void Update()
        {
            if (isGameActive && timeLimit > 0)
            {
                timeLimit -= Time.deltaTime;
                UpdateTimerDisplay(timeLimit);
                
                if (timeLimit <= 0) 
                {
                    FinalizeGameOver(); 
                }
            }
        }

        // --- BINAGO: GINAWANG MINUTES AND SECONDS + CUSTOM COLOR ---
        void UpdateTimerDisplay(float time)
        {
            if (timerTextUI != null)
            {
                if (time < 0) time = 0;

                int minutes = Mathf.FloorToInt(time / 60); 
                int seconds = Mathf.FloorToInt(time % 60); 

                // Format: MM:SS
                timerTextUI.text = string.Format("<mspace=0.6em>{0:00}:{1:00}</mspace>", minutes, seconds);
                
                // Babalik sa custom color mo imbis na laging white!
                timerTextUI.color = (time <= 10) ? Color.red : originalTimerColor;
            }
        }

        public void CorrectStep() 
        { 
            StartCoroutine(ShowIcon(checkIcon)); 
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        }
        
        public void WrongItem()
        {
            if (!isGameActive) return;

            StartCoroutine(ShowIcon(xIcon));
            
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
            if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 

            timeLimit -= penaltyTime;
            if (penaltyTextUI != null) StartCoroutine(AnimatePenaltyText());

            if (timeLimit <= 0) FinalizeGameOver();
            
            // I-update agad ang display pagkabawas!
            UpdateTimerDisplay(timeLimit);
        }

        IEnumerator ShowIcon(GameObject icon)
        {
            if(icon != null) { icon.SetActive(true); yield return new WaitForSeconds(1f); icon.SetActive(false); }
        }

        IEnumerator AnimatePenaltyText()
        {
            penaltyTextUI.gameObject.SetActive(true);
            penaltyTextUI.text = "-" + penaltyTime;
            penaltyTextUI.rectTransform.localPosition = penaltyOriginalPos;

            Color c = penaltyTextUI.color; c.a = 1f; penaltyTextUI.color = c;
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                penaltyTextUI.rectTransform.localPosition += Vector3.down * fallSpeed * Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                penaltyTextUI.color = c;
                yield return null;
            }
            penaltyTextUI.gameObject.SetActive(false);
        }

        public void NextPatient()
        {
            if (currentPatientIndex < patientUIs.Length - 1)
            {
                StartCoroutine(SlideRoutine(currentPatientIndex, currentPatientIndex + 1));
                currentPatientIndex++;
            }
            else
            {
                Debug.Log("LAHAT NG PASYENTE GAMOT NA! YOU WIN THE LEVEL!");
                isGameActive = false;
                Invoke("ShowWinScreen", 1.5f); 
            }
        }

        IEnumerator SlideRoutine(int currentIndex, int nextIndex)
        {
            float time = 0;
            RectTransform currentPatient = patientUIs[currentIndex];
            RectTransform currentTable = tableUIs[currentIndex];
            RectTransform nextPatient = patientUIs[nextIndex];
            RectTransform nextTable = tableUIs[nextIndex];

            while (time < slideDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, time / slideDuration); 

                currentPatient.anchoredPosition = new Vector2(Mathf.Lerp(0, offScreenLeft, t), currentPatient.anchoredPosition.y);
                currentTable.anchoredPosition = new Vector2(Mathf.Lerp(0, offScreenLeft, t), currentTable.anchoredPosition.y);

                nextPatient.anchoredPosition = new Vector2(Mathf.Lerp(offScreenRight, 0, t), nextPatient.anchoredPosition.y);
                nextTable.anchoredPosition = new Vector2(Mathf.Lerp(offScreenRight, 0, t), nextTable.anchoredPosition.y);

                yield return null;
            }
        }

        // ==========================================
        // --- WIN, LOSE, AT PAUSE PANEL COMMANDS ---
        // ==========================================

        void FinalizeGameOver()
        {
            timeLimit = 0; 
            isGameActive = false;
            
            // --- BINAGO: GINAWANG 00:00 ---
            if(timerTextUI != null) timerTextUI.text = "00:00";
            
            if(losePanel != null) losePanel.SetActive(true); 
            
            if (AudioManager.instance != null) { 
                AudioManager.instance.PlaySFX(AudioManager.instance.loseSound); 
                AudioManager.instance.PauseBGM(); 
            }
        }

        void ShowWinScreen()
        {
            if(winPanel != null) winPanel.SetActive(true);

            if (AudioManager.instance != null) { 
                AudioManager.instance.PlaySFX(AudioManager.instance.winSound); 
                AudioManager.instance.PauseBGM(); 
            }

            float scoreTime = timeLimit; 
            
            if(star1) star1.color = earnedColor;
            if (scoreTime >= silverStarThreshold && star2) star2.color = earnedColor;
            if (scoreTime >= goldStarThreshold && star3) star3.color = earnedColor;

            float min = Mathf.FloorToInt(scoreTime / 60); 
            float sec = Mathf.FloorToInt(scoreTime % 60);
            if(timeFinishedText != null) timeFinishedText.text = string.Format("Time Left: {0:00}:{1:00}", min, sec);

            PlayerPrefs.SetInt("Level9_Unlocked", 1);
            PlayerPrefs.Save();
        }

        public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
        public void PauseGame() { if(pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
        public void ResumeGame() { if(pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
        public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }

        IEnumerator FadeInRoutine()
        {
            float timer = 0f;
            Color c = fadeOverlay.color;
            float fadeTime = 1.5f; 

            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, timer / fadeTime); 
                fadeOverlay.color = c;
                yield return null;
            }

            fadeOverlay.gameObject.SetActive(false); 
        }
    }