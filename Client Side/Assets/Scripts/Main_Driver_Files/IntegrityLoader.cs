using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine.Networking;
using System.IO.Compression;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
public class Integrity_Loader : MonoBehaviour
{
    [SerializeField] Image CircleImg;
    [SerializeField] TextMeshProUGUI txtProgress;
    [SerializeField] TextMeshProUGUI LoadText;


    [SerializeField][Range(0, 1)] float progress = 0f;
    [SerializeField] float fadeOutTime = 1f;


    public bool End_Flag = false;
    public bool bypass;
    string[] assetPaths = new string[]
    {
        "Assets/Character",
        "Assets/Animations",
        "Assets/Fonts",
    };

   


    void Start()
    {
        if (!bypass)
        {
            CircleImg.fillAmount = 0f;
            txtProgress.text = "0";
            CircleImg.gameObject.SetActive(true);
            txtProgress.gameObject.SetActive(true);
            LoadText.gameObject.SetActive(true);

            StartCoroutine(CheckInternetAndLoadAssets());
        }
    }

    IEnumerator CheckInternetAndLoadAssets()
    {
        yield return new WaitForSeconds(3f);
        CircleImg.fillAmount = 0.05f;
        txtProgress.text = "5";
        yield return StartCoroutine(LoadAssetsAsync());
    }

    IEnumerator LoadAssetsAsync()
    {
        // Mapping of folders to their sources
        var folderSources = new Dictionary<string, string>
        {
            { "../Models/XLMRoberta-Alexa-Intents-Classification", "https://huggingface.co/qanastek/XLMRoberta-Alexa-Intents-Classification" }
        };

        // Check for missing folders
        var missingFolders = folderSources
            .Where(pair => !Directory.Exists(Path.Combine(Application.dataPath, pair.Key)))
            .ToList();

        if (missingFolders.Count > 0)
        {
            foreach (var missingFolder in missingFolders)
            {
                Debug.LogWarning($"Missing folder: {missingFolder.Key}");
                yield return StartCoroutine(DownloadFolder(missingFolder.Key, missingFolder.Value));
            }
        }

        // Proceed with asset loading as usual
        int totalFiles = 0;
        foreach (string folderPath in assetPaths)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                totalFiles += files.Length;
            }
        }

        float elapsedTime = 0f;
        float baseProgress = 0.05f;
        progress = baseProgress;
        LoadText.text = "Loading Assets";
        LoadText.fontSize = 36;

        foreach (string folderPath in assetPaths)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                foreach (string currentFilePath in files)
                {
                    yield return null;
                    elapsedTime += Time.deltaTime;
                    progress = baseProgress + (elapsedTime / totalFiles);
                    CircleImg.fillAmount = progress;
                    int percentage = Mathf.RoundToInt(progress * 100f);
                    txtProgress.text = percentage.ToString();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }


    IEnumerator DownloadFolder(string folderPath, string repoUrl)
    {
        Debug.Log($"Cloning repository: {repoUrl}");

        string fullPath = Path.Combine(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName, "models", folderPath);
        Debug.Log($"Full clone path: {fullPath}");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        // Run the git clone command using a process
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone --depth 1 {repoUrl} \"{fullPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        // Read output and error streams asynchronously to prevent blocking
        while (!process.HasExited)
        {
            Debug.Log(process.StandardOutput.ReadLine());
            Debug.LogError(process.StandardError.ReadLine());
            yield return null;
        }

        if (process.ExitCode == 0)
        {
            Debug.Log("Repository cloned successfully.");
        }
        else
        {
            Debug.LogError($"Failed to clone repository. Exit code: {process.ExitCode}");
        }
    }

    IEnumerator DummyIntegrityCheck(float startingProgress)
    {
        LoadText.text = "Verifying Files";
        float dummyCheckDuration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < dummyCheckDuration)
        {
            yield return null;

            progress = Mathf.Lerp(startingProgress, startingProgress + 0.2f, elapsedTime / dummyCheckDuration);
            CircleImg.fillAmount = progress;

            int percentage = Mathf.RoundToInt(progress * 100f);
            txtProgress.text = percentage.ToString();

            elapsedTime += Time.deltaTime;
        }
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(AdditionalUpdateCheck(progress));
        yield return new WaitForSeconds(3f);

    }

    IEnumerator AdditionalUpdateCheck(float startingProgress)
    {
        LoadText.text = "Checking For Updates";
        float updateCheckDuration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < updateCheckDuration)
        {
            yield return null;

            float updateCheckProgress = Mathf.Lerp(startingProgress, startingProgress + 0.2f, elapsedTime / updateCheckDuration);
            progress = updateCheckProgress;
            CircleImg.fillAmount = progress;

            int percentage = Mathf.RoundToInt(progress * 100f);
            txtProgress.text = percentage.ToString();

            elapsedTime += Time.deltaTime;
        }
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(CleanUp(progress));
        yield return new WaitForSeconds(2f);

    }
    IEnumerator CleanUp(float startingProgress)
    {
        LoadText.text = "Getting Things Ready";
        float CleanUpDuration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < CleanUpDuration)
        {
            yield return null;

            float CleanUpProgress = Mathf.Lerp(startingProgress, startingProgress + 0.3f, elapsedTime / CleanUpDuration);
            progress = CleanUpProgress;
            CircleImg.fillAmount = progress;

            int percentage = Mathf.RoundToInt(progress * 100f);
            txtProgress.text = percentage.ToString();

            elapsedTime += Time.deltaTime;
        }

    }
}