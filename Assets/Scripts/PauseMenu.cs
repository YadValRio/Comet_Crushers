using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
public class PauseMenu : MonoBehaviour
{
    static PauseMenu instance;

    public static bool IsPaused => instance != null && instance.paused;
    // avoid aiming or firing when game is paused
    public static bool BlockGameplayInput => IsPaused ||
        (instance != null && instance.inputBlockedFrame == Time.frameCount);

    int currentScore;
    int inputBlockedFrame = -1;
    bool paused;
    float previousTimeScale;
    bool previousAudioPause;
    CursorLockMode previousCursorLock;
    bool previousCursorVisible;
    GUIStyle titleStyle;
    GUIStyle textStyle;
    GUIStyle buttonStyle;

    public static void EnsureExists()
    {
        if (instance != null) return;
        if (UnityEngine.Object.FindAnyObjectByType<PauseMenu>() != null) return;
        new GameObject("Pause Menu").AddComponent<PauseMenu>();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused) Resume();
            else Pause();
        }
    }

    void Pause()
    {
        previousTimeScale = Time.timeScale;
        previousAudioPause = AudioListener.pause;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        paused = true;

        inputBlockedFrame = Time.frameCount;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AsteroidManager.Instance != null) 
        currentScore = AsteroidManager.Instance.AsteroidsDestroyed;
    }

    public void Resume()
    {
        if (!paused) return;
        RestoreGameState();
        inputBlockedFrame = Time.frameCount;
    }

    void RestoreGameState()
    {
        if (!paused) return;
        paused = false;
        Time.timeScale = previousTimeScale;
        AudioListener.pause = previousAudioPause;
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
    }

    void OnDestroy()
    {
        if (instance != this) return;
        RestoreGameState();
        instance = null;
    }

    void OnGUI()
    {
        if (!paused) return;
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 30, alignment = TextAnchor.MiddleCenter };
            titleStyle.normal.textColor = Color.white;
            textStyle = new GUIStyle(GUI.skin.label) { fontSize = 17, alignment = TextAnchor.MiddleCenter };
            textStyle.normal.textColor = Color.white;
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 20 };
        }

        GUI.depth = -100;
        Matrix4x4 oldMatrix = GUI.matrix;
        Color oldColor = GUI.color;
        Color oldBackground = GUI.backgroundColor;

        //fixed layout, which keeps the menu scaled correctly when window size changes
        float scale = Mathf.Min(Screen.width / 800f, Screen.height / 650f);
        GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width * .5f, Screen.height * .5f, 0f),
            Quaternion.identity, Vector3.one * scale);
        GUI.color = new Color(.01f, .02f, .04f, .92f);
        GUI.DrawTexture(new Rect(-Screen.width / scale, -Screen.height / scale,
            Screen.width * 2f / scale, Screen.height * 2f / scale), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.backgroundColor = new Color(.25f, .4f, .6f);

        GUILayout.BeginArea(new Rect(-260f, -300f, 520f, 600f), GUI.skin.box);
        GUILayout.Space(18f);
        GUILayout.Label("PAUSED", titleStyle);
        GUILayout.Label($"Asteroids destroyed this run: {currentScore}", textStyle);
        GUILayout.Space(16f);

        // placeholder buttons, esc resumes the game
        GUILayout.Button("Resume", buttonStyle, GUILayout.Height(52f));
        GUILayout.Space(10f);

        GUILayout.Button("Restart", buttonStyle, GUILayout.Height(52f));
        GUILayout.Space(10f);

        GUILayout.Button("Highscores", buttonStyle, GUILayout.Height(52f));
        GUILayout.Space(10f);

        GUILayout.Button("Quit", buttonStyle, GUILayout.Height(52f));
        GUILayout.FlexibleSpace();

        GUILayout.Label("Esc to resume", textStyle);
        GUILayout.Space(14f);
        GUILayout.EndArea();
        GUI.matrix = oldMatrix;
        GUI.color = oldColor;
        GUI.backgroundColor = oldBackground;
    }
}
