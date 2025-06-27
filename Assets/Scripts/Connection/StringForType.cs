using System;
using Messages;

public class StringForType
{
    public static string CharacterTypeToString(CharacterType characterType)
    {
        return characterType.ToString();
    }

    public static CharacterType StringToCharacterType(string characterName)
    {
        if (Enum.TryParse(characterName, out CharacterType characterType))
        {
            return characterType;
        }

        throw new ArgumentException($"Невозможно преобразовать {characterName} в CharacterType");
    }
}