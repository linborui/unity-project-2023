using UnityEngine;

public class WorldInfo : MonoBehaviour
{
    [SerializeField, TextArea] private string gameStory =   "Only when Alma successfully kills the Dream Dominator, " +
                                                            "the Ancient Stone Warden, in Stone's Dream, can she obtain the Dream Fragments known as Shattered Shadows. " +
                                                            "By acquiring the Shattered Shadows, she can then leave this dream realm and continue to fulfill the next sacred mission entrusted "+
                                                            "to her by the Church. This mission will lead Alma towards new adventures, as she continues to safeguard the world's peace and tranquility."; 
    [SerializeField, TextArea] private string gameWold =    "In Stone's Dream, there are elite monsters that engage in both close-range and "+
                                                            "long-range attacks, such as the Rock Shield Guardian and the Giant Boulder Fury. "+
                                                            "These elite monsters serve as the bosses of the game, known as the Dream Dominators."+
                                                            "The Dream Dominators are powerful stone creatures, with the leader named Ancient Stone Warden. "+
                                                            "Normally, the Ancient Stone Warden remains hidden deep within the dream realm. "+
                                                            "Alma must defeat the formidable army of stone creatures guarding the outer areas " +
                                                            "in order to confront the malevolent Dream Dominator, the Ancient Stone Warden." ;
    
    public string GetPrompt()
    {
        return $"Game Story: {gameStory}\n" +
               $"Game World: {gameWold}\n";
    }
}
