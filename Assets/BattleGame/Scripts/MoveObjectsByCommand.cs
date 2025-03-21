using System.Collections.Generic;
using System.Reflection;
using LLMUnity;
using UnityEngine;
using UnityEngine.UI;
using BattleGame.Scripts;

// Main class that handles the movement of objects based on text commands
public class MoveObjectsByCommand : MonoBehaviour
{
    // References to required components
    public LLMCharacter llmCharacter;        // Reference to the AI language model
    public InputField playerText;            // Input field where player types commands
    public InputField aiText;            // Input field where ai response is displayed
    
    // References to the UI objects that can be moved
    public RectTransform circularEnemy;
    public RectTransform TriangularEnemy;
    public RectTransform miniBossEnemy;
    public RectTransform bossEnemy;
    public RectTransform playerSpaceship;

    void Start()
    {
        // Set up the input field to listen for submissions
        playerText.onSubmit.AddListener(onInputFieldSubmit);
        playerText.Select();  // Automatically focus the input field

        // Assign player reference to all enemies
        Enemy[] enemies = new Enemy[] {
            circularEnemy?.GetComponent<Enemy>(),
            TriangularEnemy?.GetComponent<Enemy>(),
            miniBossEnemy?.GetComponent<Enemy>(),
            bossEnemy?.GetComponent<Enemy>()
        };

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.playerTarget = playerSpaceship;
            }
        }
    }

    // Helper method to get all function names from a class using reflection
    string[] GetFunctionNames<T>()
    {
        List<string> functionNames = new List<string>();
        foreach (var function in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)) 
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }

    // Constructs the prompt for the AI to understand direction commands
    string ConstructDirectionPrompt(string message)
    {
        string prompt = "From the input, which direction is mentioned? Choose from the following options:\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<DirectionFunctions>()) 
            prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice, focusing only on direction";
        return prompt;
    }

    // Constructs the prompt for the AI to understand color commands
    string ConstructColorPrompt(string message)
    {
        string prompt = "From the input, which color is mentioned? Choose from the following options:\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<ColorFunctions>()) 
            prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice, focusing only on color";
        return prompt;
    }

    // Constructs the prompt for the AI to understand commands
    string ConstructCommandPrompt(string message)
    {
        string prompt = "From the input, which command is mentioned? Choose from the following options:\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<PlayerCommands>()) 
            prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice, focusing only on commands";
        return prompt;
    }

    // Called when the player submits text in the input field
    async void onInputFieldSubmit(string message)
    {
        playerText.interactable = false;

        try
        {
            // Check for special commands first
            string getCommand = await llmCharacter.Chat(ConstructCommandPrompt(message));
            SpaceshipController spaceship = playerSpaceship.GetComponent<SpaceshipController>();

            if (spaceship != null)
            {
                switch (getCommand)
                {
                    case "Shield":
                        if (spaceship.IsShieldAvailable())
                        {
                            spaceship.ActivateShield();
                            aiText.text = "Shield activated!";
                            return;
                        }
                        else if (spaceship.IsShieldActive())
                        {
                            aiText.text = "Shield is already active!";
                            return;
                        }
                        else
                        {
                            aiText.text = "Shield is on cooldown!";
                            return;
                        }

                    case "Power":
                        spaceship.FirePowerBomb();
                        aiText.text = "Firing power bomb!";
                        return;
                }
            }

            // Check for targeting commands
            if (message.ToLower().Contains("target") || message.ToLower().Contains("aim"))
            {
                string enemyType = "";
                if (message.ToLower().Contains("circular")) enemyType = "circular";
                else if (message.ToLower().Contains("triangular")) enemyType = "triangular";
                else if (message.ToLower().Contains("miniboss")) enemyType = "miniboss";
                else if (message.ToLower().Contains("boss")) enemyType = "boss";

                if (!string.IsNullOrEmpty(enemyType) && spaceship != null)
                {
                    spaceship.TargetEnemy(enemyType);
                    aiText.text = $"Targeting {enemyType} enemy";
                    return;
                }
            }

            // Handle movement commands
            string getDirection = await llmCharacter.Chat(ConstructDirectionPrompt(message));
            if (getDirection == "MoveLeft" || getDirection == "MoveRight")
            {
                Vector3 direction = (Vector3)typeof(DirectionFunctions).GetMethod(getDirection).Invoke(null, null);
                Vector2 currentPos = playerSpaceship.anchoredPosition;
                float newX = currentPos.x + (direction.x * 140f);
                newX = Mathf.Clamp(newX, -140f, 140f);
                playerSpaceship.anchoredPosition = new Vector2(newX, currentPos.y);
                aiText.text = $"Moving spaceship {getDirection.Replace("Move", "").ToLower()}";
            }
            else
            {
                string aiResponse = await llmCharacter.Chat(message);
                aiText.text = aiResponse;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing command: {e.Message}");
            aiText.text = "I couldn't process that command. Please try again.";
        }
        finally
        {
            playerText.interactable = true;
            playerText.text = "";
            playerText.Select();
        }
    }

    // Helper method to get the correct square based on color
    private RectTransform GetObjectByColor(Color color)
    {
        if (color == Color.blue)
        {
            return circularEnemy;
        }
        else if (color == Color.red)
        {
            return TriangularEnemy;
        }
        else
        {
            return null;
        }
    }

    // Cancels any pending AI requests
    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
    }

    // Quits the application (called by UI button)
    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    // Editor-only validation to ensure the AI model is properly set up
    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
        {
            Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
            onValidateWarning = false;
        }
    }
}
