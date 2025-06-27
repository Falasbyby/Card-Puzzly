using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    // Целевое соотношение сторон (например, 16:9)
    public float targetAspectRatio = 16f / 9f;
    // Базовый ВЕРТИКАЛЬНЫЙ угол обзора (FOV) для целевого соотношения
    public float baseVerticalFOV = 60f;

    private Camera cam;
    private float initialVerticalFOV;

    void Awake()
    {
    
    }
    void Start()
    {
            cam = GetComponent<Camera>();
        // Сохраняем начальное значение FOV, установленное в инспекторе, если оно отличается от baseVerticalFOV
        // Но для логики масштабирования будем использовать baseVerticalFOV
        initialVerticalFOV = cam.fieldOfView; 
        AdjustCameraSize();
    }

    // Вы можете вызывать это в Update, если соотношение сторон может меняться во время игры (например, при изменении размера окна на ПК)
     void Update()
     {
      
     }

    void AdjustCameraSize()
    {
        float currentAspectRatio = (float)Screen.width / Screen.height;

        if (currentAspectRatio < targetAspectRatio)
        {
            // Экран *уже* целевого, нужно увеличить вертикальный FOV,
            // чтобы сохранить видимую область по вертикали (имитируем отдаление)
            // Формула основана на сохранении относительного вертикального охвата:
            // tan(newFOV / 2) = tan(baseFOV / 2) * (targetAspect / currentAspect)
            float baseTan = Mathf.Tan(baseVerticalFOV * Mathf.Deg2Rad / 2f);
            float newTan = baseTan * (targetAspectRatio / currentAspectRatio);
            cam.fieldOfView = 2f * Mathf.Atan(newTan) * Mathf.Rad2Deg;
        }
        else
        {
            // Экран шире или равен целевому, используем базовый FOV
            cam.fieldOfView = baseVerticalFOV;
        }
    }
} 