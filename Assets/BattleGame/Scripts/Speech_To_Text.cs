using UnityEngine;
using System.Threading.Tasks;
using System;
using System.IO;
using UnityEngine.Networking;
using BattleGame.Scripts;

public class Speech_To_Text : MonoBehaviour, ISpeechToText
{
    private AudioClip recordingClip;
    private bool isRecording = false;
    private string deviceName;
    private int maxRecordingTime = 30;
    private string apiKey = "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"; // Your HuggingFace API key
    private string endpoint = "https://api-inference.huggingface.co/models/openai/whisper-large-v3";

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.Log("Already recording...");
            return;
        }

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found!");
            return;
        }

        deviceName = Microphone.devices[0];
        Debug.Log($"Using microphone: {deviceName}");
        
        recordingClip = Microphone.Start(deviceName, false, maxRecordingTime, 44100);
        if (recordingClip == null)
        {
            Debug.LogError("Failed to start recording!");
            return;
        }

        isRecording = true;
        Debug.Log("Started recording...");
    }

    public async Task<string> StopRecordingAndRecognize()
    {
        if (!isRecording)
        {
            Debug.LogWarning("Not currently recording!");
            return string.Empty;
        }

        // Stop recording
        Microphone.End(deviceName);
        isRecording = false;
        Debug.Log("Stopped recording");

        if (recordingClip == null)
        {
            Debug.LogError("No recording clip available!");
            return "Error: No recording available";
        }

        // Check if we actually recorded any audio
        float[] samples = new float[recordingClip.samples];
        recordingClip.GetData(samples, 0);
        
        float maxVolume = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            maxVolume = Mathf.Max(maxVolume, Mathf.Abs(samples[i]));
        }

        Debug.Log($"Max recorded volume: {maxVolume}");
        if (maxVolume < 0.01f)
        {
            Debug.LogWarning("Recording volume too low - might be no audio input!");
            return "Error: No audio detected";
        }

        Debug.Log($"Converting audio clip to WAV (samples: {samples.Length}, frequency: {recordingClip.frequency})");

        try
        {
            // Convert AudioClip to WAV bytes
            byte[] wavData = AudioClipToWav(recordingClip);
            Debug.Log($"WAV data size: {wavData.Length} bytes");

            // Send to HuggingFace API
            string recognizedText = await SendAudioToHuggingFace(wavData);
            return recognizedText;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in speech recognition: {e.Message}");
            return "Error in speech recognition";
        }
    }

    private async Task<string> SendAudioToHuggingFace(byte[] audioData)
    {
        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(audioData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Content-Type", "audio/wav");

            Debug.Log("Sending audio to HuggingFace API...");
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"API request failed: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                throw new Exception($"API request failed: {request.error}");
            }

            // Parse the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Raw API Response: {jsonResponse}");

            try 
            {
                // The response should be in format [{"text": "recognized text"}]
                if (jsonResponse.StartsWith("["))
                {
                    jsonResponse = jsonResponse.Trim('[', ']');
                }
                
                int startIndex = jsonResponse.IndexOf("\"text\":\"") + "\"text\":\"".Length;
                int endIndex = jsonResponse.LastIndexOf("\"}");
                if (startIndex >= 0 && endIndex >= 0)
                {
                    string text = jsonResponse.Substring(startIndex, endIndex - startIndex);
                    Debug.Log($"Extracted text: {text}");
                    return text;
                }
                else
                {
                    Debug.LogError("Could not find text in response");
                    return "Error parsing response";
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing response: {e.Message}");
                return "Error parsing response";
            }
        }
    }

    private byte[] AudioClipToWav(AudioClip clip)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (Int16)(samples[i] * 32767);
        }

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                // WAV header
                writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
                writer.Write(36 + intData.Length * 2);
                writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
                writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)1);
                writer.Write(44100);
                writer.Write(44100 * 2);
                writer.Write((ushort)2);
                writer.Write((ushort)16);
                writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
                writer.Write(intData.Length * 2);

                // WAV data
                foreach (Int16 sample in intData)
                {
                    writer.Write(sample);
                }
            }
            return memoryStream.ToArray();
        }
    }
} 