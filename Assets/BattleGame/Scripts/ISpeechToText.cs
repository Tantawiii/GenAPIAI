using System.Threading.Tasks;
using UnityEngine;

namespace BattleGame.Scripts
{
    public interface ISpeechToText
    {
        void StartRecording();
        Task<string> StopRecordingAndRecognize();
    }
} 