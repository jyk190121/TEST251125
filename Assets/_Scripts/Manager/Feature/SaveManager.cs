using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static string _savePath;
    private static readonly string SAVE_FILE = "gamedata.json";

    private static string SavePath // 저장 경로 캐싱
    {
        get
        {
            if (string.IsNullOrEmpty(_savePath))
            {
                _savePath = Application.persistentDataPath + "/saves/";
            }
            return _savePath;
        }
    }

    /// <summary>
    /// 게임 저장
    /// </summary>
    public void SaveGame(DataManager Data)
    {
        try
        {
            // 1단계: 디렉토리 생성
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            // 2단계: JSON 직렬화
            string json = JsonUtility.ToJson(Data, true);

            // 3단계: 전체 경로 생성
            string fullPath = Path.Combine(SavePath, SAVE_FILE);

            // 4단계: 파일 저장
            File.WriteAllText(fullPath, json);

            // 5단계: 저장 로그
            Debug.Log("게임 저장 완료: " + fullPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("저장 오류: " + e.Message);
        }
    }

    /// <summary>
    /// 플레이어 데이터 로드
    /// </summary>
    public DataManager LoadData()
    {
        try
        {
            // 1단계: 전체 경로 생성
            string fullPath = Path.Combine(SavePath, SAVE_FILE);

            // 2단계: 파일 존재 확인
            if (!File.Exists(fullPath))
            {
                Debug.Log("저장 데이터가 없습니다: " + fullPath);
                return null;
            }

            // 3단계: 파일 읽기
            string json = File.ReadAllText(fullPath);

            // 4단계: JSON 역직렬화
            DataManager Data = JsonUtility.FromJson<DataManager>(json);

            // 5단계: 로드 로그
            Debug.Log("게임 로드 완료: " + fullPath);

            // 6단계: 데이터 반환
            return Data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("로드 오류: " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// 저장 데이터 존재 확인
    /// </summary>
    public static bool HasSaveData()
    {
        string fullPath = Path.Combine(SavePath, SAVE_FILE);
        return File.Exists(fullPath);
    }

    /// <summary>
    /// 저장 데이터 삭제 (되돌릴 수 없음)
    /// </summary>
    public static void DeleteSaveData()
    {
        try
        {
            string fullPath = Path.Combine(SavePath, SAVE_FILE);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log("저장 파일 삭제 완료: " + fullPath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("저장 파일 삭제 오류: " + e.Message);
        }
    }

    public void Initialize()
    {

    }
}
