using Messages;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

public class SocketSystem : Singleton<SocketSystem>
{
    [SerializeField] private bool test;
    private string gameId;

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void sendMessageToMobileApp(string message);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UnityToReact(string message);
#endif



    public void Send(string message)
    {
        CustomDebug.Log($"Sending message: {message}");

        if (!test)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaClass jc = new AndroidJavaClass("com.azesmwayreactnativeunity.ReactNativeUnityViewManager"))
                {
                    jc.CallStatic("sendMessageToMobileApp", message);
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IOS && !UNITY_EDITOR
                sendMessageToMobileApp(message);
#endif
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            UnityToReact(message);
#endif
        }
    }

    public void SendToReact<T>(MessageFromUnity code, T data)
    {
        string jsonData = JsonUtility.ToJson(data);
        string fullMessage = $"{(int)code}:{jsonData}";
        Send(fullMessage);
        CustomDebug.Log($"Отправлено сообщение в React: {fullMessage}");
    }

    public void SendBoostGameMessage()
    {
        var message = new BoostGameMessage { game_id = gameId };
        SendToReact(MessageFromUnity.BoostGame, message);
    }

    public void SendLoseMessage(int points, string time)
    {
        var message = new LoseMessage { game_id = gameId, points = points, time = time };
        SendToReact(MessageFromUnity.Lose, message);
    }

    public void SendFinishMessage(int points)
    {
        var message = new FinishMessage { game_id = gameId, points = points };
        SendToReact(MessageFromUnity.Finish, message);
    }

    public void SendCheatMessage()
    {
        var message = new CheatMessage { game_id = gameId };
        SendToReact(MessageFromUnity.Cheat, message);
    }

    public void SendBackButtonMessage()
    {
        // var message = new FinishMessage { game_id = gameId, points = GameManager.Instance.currentScore};
        //  SendToReact(MessageFromUnity.BackButton, message);

    }

    private void SendGameIdMismatchError(string incomingGameId)
    {
        Debug.LogError($"Ошибка: game_id не совпадает. Ожидается {gameId}, получено {incomingGameId}");

        var errorMessage = new ErrorMessage
        {
            code = "2404",
            message = "game_id_not_match",
            data = new ErrorMessage.GameIdMismatchData { game_id = incomingGameId }
        };

        SendToReact(MessageFromUnity.GameIdNotMetch, errorMessage);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Нажать "1" для теста Init
        {
            string testMessage = "1000:{\"game_id\":123}";
            ReactToUnity(testMessage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // Нажать "2" для теста AppliedBoosts
        {
            string testMessage =
                "1001:{\"game_id\":123}";
            ReactToUnity(testMessage);
        }
   //не используем
        if (Input.GetKeyDown(KeyCode.Alpha3)) // Нажать "3" для теста Continue
        {
           // string testMessage = "1002:{\"game_id\":123}";
           // ReactToUnity(testMessage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) // Нажать "3" для теста Continue
        {
            string testMessage = "1003:{\"game_id\":123}";
            ReactToUnity(testMessage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            string testMessage = "1004:{\"game_id\":123}";
            ReactToUnity(testMessage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            string testMessage = "1005:{\"game_id\":123}";
            ReactToUnity(testMessage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            string testMessage = "1006:{\"game_id\":123}";
            ReactToUnity(testMessage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9)) // Нажать "4" для теста неизвестного кода
        {
            string testMessage = "9999:{}";
            ReactToUnity(testMessage);
        }
#endif
    }

    public void ReactToUnity(string message)
    {
        CustomDebug.Log($"Получено сообщение от сервера: {message}");

        // Разделяем строку на code и data
        int separatorIndex = message.IndexOf(':');
        if (separatorIndex == -1)
        {
            CustomDebug.LogError("Некорректное сообщение от сервера. Отсутствует разделитель ':'");
            return;
        }

        // Получаем code и JSON-строку
        string code = message.Substring(0, separatorIndex);
        string jsonData = message.Substring(separatorIndex + 1);

        CustomDebug.Log($"Код сообщения: {code}");
        CustomDebug.Log($"Данные JSON: {jsonData}");

        // В зависимости от кода определяем, как обработать JSON
        ProcessMessage(code, jsonData);
    }

    private void ProcessMessage(string code, string jsonData)
    {
        switch (code)
        {
            case "1000": // Init
                var initMessage = JsonUtility.FromJson<InitMessage>(jsonData);
                HandleInitMessage(initMessage);
                break;

            case "1001": // AppliedBoosts
                var appliedBoostsMessage = JsonUtility.FromJson<AppliedBoostsMessage>(jsonData);
                if (!IsGameIdValid(appliedBoostsMessage.game_id))
                {
                    SendGameIdMismatchError(appliedBoostsMessage.game_id); // Пример отправки ошибки
                    return;
                }

                HandleAppliedBoostsMessage(appliedBoostsMessage);
                break;

            case "1002": // Continue
                var continueMessage = JsonUtility.FromJson<GameIdMessage>(jsonData);
                if (!IsGameIdValid(continueMessage.game_id))
                {
                    SendGameIdMismatchError(continueMessage.game_id);
                    return;
                }

                HandleContinueMessage(continueMessage);
                break;
            case "1003": // restart
                var RestartSceneMessage = JsonUtility.FromJson<GameIdMessage>(jsonData);
                if (!IsGameIdValid(RestartSceneMessage.game_id))
                {
                    SendGameIdMismatchError(RestartSceneMessage.game_id);
                    return;
                }

                HandleRestartSceneMessage(RestartSceneMessage);
                break;
            case "1004": // pause
                var PauseSceneMessage = JsonUtility.FromJson<GameIdMessage>(jsonData);
                if (!IsGameIdValid(PauseSceneMessage.game_id))
                {
                    SendGameIdMismatchError(PauseSceneMessage.game_id);
                    return;
                }

                HandlePauseOrPlaySceneMessage(PauseSceneMessage, true);
                break;
            case "1005": // PlayGame
                var PlayGameSceneMessage = JsonUtility.FromJson<GameIdMessage>(jsonData);
                if (!IsGameIdValid(PlayGameSceneMessage.game_id))
                {
                    SendGameIdMismatchError(PlayGameSceneMessage.game_id);
                    return;
                }

                HandlePauseOrPlaySceneMessage(PlayGameSceneMessage, false);
                break;
            case "1006":
                var ExtraDeadMessage = JsonUtility.FromJson<GameIdMessage>(jsonData);
                if (!IsGameIdValid(ExtraDeadMessage.game_id))
                {
                    SendGameIdMismatchError(ExtraDeadMessage.game_id);
                    return;
                }

                HandleExtraDeadMessage(ExtraDeadMessage);
                break;
            default:
                CustomDebug.LogWarning($"Неизвестный код сообщения: {code}");
                break;
        }
    }

    private void HandleExtraDeadMessage(GameIdMessage extraDeadMessage)
    {
        // Player.Instance.Death();
    }

    private float timeScale;

    private void HandlePauseOrPlaySceneMessage(GameIdMessage pauseSceneMessage, bool pause)
    {
        if (pause)
        {
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = timeScale;
        }
    }

    private void HandleInitMessage(InitMessage message)
    {
        gameId = message.game_id;

        LevelManager.Instance.Init();
        CustomDebug.Log(
            $"Init: Game ID = {message.game_id}");
    }

    private bool IsGameIdValid(string incomingGameId)
    {
        return gameId == incomingGameId;
    }

    private void HandleAppliedBoostsMessage(AppliedBoostsMessage message)
    {
        LevelManager.Instance.StartGame();

    }

    private void HandleContinueMessage(GameIdMessage message)
    {
        CustomDebug.Log($"Continue: Game ID = {message.game_id}");
        //  GameManager.Instance.ContinueLevel();
    }

    private void HandleRestartSceneMessage(GameIdMessage restartSceneMessage)
    {
        SceneManager.LoadScene(0);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}