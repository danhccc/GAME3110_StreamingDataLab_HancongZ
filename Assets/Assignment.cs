
/*
This RPG data streaming assignment was created by Fernando Restituto.
Pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

    public LinkedList<int> otherThings;
    public LinkedList<int> affliction;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion


#region Assignment Part 1

static public class AssignmentPart1
{

    //REVIEW
    static string path = Application.dataPath + Path.DirectorySeparatorChar + "PartySavedData.txt";
    const int PartyMemberSignifier = 1;
    const int EquipmentSignifer = 2;


    static public void SavePartyButtonPressed()
    {
        StreamWriter sw = new StreamWriter(path);

        Debug.Log("----START----");

        foreach (PartyCharacter pc in GameContent.partyCharacters)  // loop through this linklist (partyCharacters)
        {
            Debug.Log("PC class id == " + pc.classID);

            string pcStates = string.Join(",", PartyMemberSignifier, pc.classID.ToString(), pc.health, pc.mana, pc.strength, pc.agility, pc.wisdom);
            sw.WriteLine(pcStates);

            foreach(int equipID in pc.equipment)
            {
                string equipSaveData = string.Join(",", EquipmentSignifer, equipID);
                sw.WriteLine(equipSaveData);
            }

        }

        sw.Close(); // Proper way to close the file.

        Debug.Log("----END----");
    }

    static public void LoadPartyButtonPressed()
    {

        GameContent.partyCharacters.Clear();

        StreamReader sr = new StreamReader(path);

        string line;
        while((line = sr.ReadLine()) != null)
        {
            string[] csv = line.Split(',');

            int signifier = int.Parse(csv[0]);

            if (signifier == PartyMemberSignifier)
            {
                PartyCharacter pc = new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]));

                GameContent.partyCharacters.AddLast(pc);
            }

            if (signifier == EquipmentSignifer)
            {
                GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[1]));
            }
        }


        GameContent.RefreshUI();

    }

}


#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    // const int PartyCharacterSaveDataSignifier = 0;
    // const int EquipmentSaveDataSignifier = 1;


    public const string PartyMetaFile = "PartyIndicesAndNames.txt"; // okay

    private static LinkedList<PartySaveData> parties;
    private static uint lastUsedIndex;

    static public void GameStart()
    {

        GameContent.RefreshUI();

        LoadPartyMetaData();

        Debug.Log("start");

    }

    static public List<string> GetListOfPartyNames()
    {

        if (parties == null)
            return new List<string>();

        List<string> pNames = new List<string>();

        foreach (PartySaveData psd in parties)
        {
            pNames.Add(psd.name);
        }

        return pNames;

    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {

        foreach (PartySaveData psd in parties)
        {
            if (selectedName == psd.name)
                psd.LoadParty();
        }

        GameContent.RefreshUI();
        Debug.Log("l " + selectedName);

    }

    static public void SavePartyButtonPressed()
    {
        lastUsedIndex++;
        PartySaveData p = new PartySaveData(lastUsedIndex, GameContent.GetPartyNameFromInput());
        parties.AddLast(p);

        SavePartyMetaData();

        p.SaveParty();

        GameContent.RefreshUI();
        Debug.Log("s");

    }

    static public void NewPartyButtonPressed()
    {
        Debug.Log("n");
    }

    static public void DeletePartyButtonPressed()
    {
        Debug.Log("d");
    }

    static public void SavePartyMetaData()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + PartyMetaFile);


        sw.WriteLine("1," + lastUsedIndex);


        foreach (PartySaveData pData in parties)
        {
            sw.WriteLine("2," + pData.index + "," + pData.name);
        }

        sw.Close();

    }

    static public void LoadPartyMetaData()
    {
        parties = new LinkedList<PartySaveData>();

        string path = Application.dataPath + Path.DirectorySeparatorChar + PartyMetaFile;

        if (File.Exists(path))
        {
            string line = "";
            StreamReader sr = new StreamReader(path);

            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');

                //if(int.Parse(csv[0]))

                int saveDataSignifier = int.Parse(csv[0]);

                if (saveDataSignifier == 1)
                    lastUsedIndex = uint.Parse(csv[1]);
                else if (saveDataSignifier == 2)
                    parties.AddLast(new PartySaveData(uint.Parse(csv[1]), csv[2]));


            }

            sr.Close();
        }




    }


    static public void SendPartyDataToSever(NetworkedClient networkedClient)
    {
        const int PartyCharacterSaveDataSignifier = 0;
        const int EquipmentSaveDataSignifier = 1;

        LinkedList<string> data = new LinkedList<string>();

        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            data.AddLast(PartyCharacterSaveDataSignifier + "," + pc.classID + "," + pc.health
                         + "," + pc.mana + "," + pc.strength
                         + "," + pc.agility + "," + pc.wisdom);

            foreach (int equipID in pc.equipment)
            {
                data.AddLast(EquipmentSaveDataSignifier + "," + equipID);
            }
        }
        networkedClient.SendMessageToHost(ClientToServerSignifier.PartyDataTransferStart + "");

        foreach (string d in data)
        {
            networkedClient.SendMessageToHost(ClientToServerSignifier.PartyDataTransfer + "," + d);
        }

        networkedClient.SendMessageToHost(ClientToServerSignifier.PartyDataTransferEnd + "");

    }

    static public void LoadPartyFromReceivedData(LinkedList<string> data)
    {
        GameContent.partyCharacters.Clear();
        const int PartyCharacterSaveDataSignifier = 0;
        const int EquipmentSaveDataSignifier = 1;

        foreach (string line in data)
        {
            string[] csv = line.Split(',');

            int saveDataSignifier = int.Parse(csv[0]);

            if (saveDataSignifier == PartyCharacterSaveDataSignifier)
            {
                PartyCharacter pc = new PartyCharacter(int.Parse(csv[2]), int.Parse(csv[3]), int.Parse(csv[4]),
                    int.Parse(csv[5]), int.Parse(csv[6]), int.Parse(csv[7]));

                GameContent.partyCharacters.AddLast(pc);
            }
            else if (saveDataSignifier == EquipmentSaveDataSignifier)
            {
                GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[2]));
                //GameContent.partyCharacters.equipment.Last.Value.AddLast(int.Parse(csv[1]))
            }
        }
        GameContent.RefreshUI();
    }

}

#endregion

class PartySaveData
{
    const int PartyCharacterSaveDataSignifier = 0;
    const int EquipmentSaveDataSignifier = 1;

    public uint index; // we need the extra numbers!
    public string name;

    public PartySaveData(uint index, string name)
    {
        this.index = index;
        this.name = name;
    }

    public void SaveParty()
    {
        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + index + ".txt");

        Debug.Log("start of loop");
        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            sw.WriteLine(PartyCharacterSaveDataSignifier + "," + pc.classID + "," + pc.health
            + "," + pc.mana + "," + pc.strength
            + "," + pc.agility + "," + pc.wisdom);

            foreach (int equipID in pc.equipment)
            {
                sw.WriteLine(EquipmentSaveDataSignifier + "," + equipID);
            }
        }

        sw.Close();
        Debug.Log("end of loop");

    }

    public void LoadParty()
    {
        string path = Application.dataPath + Path.DirectorySeparatorChar + index + ".txt";

        if (File.Exists(path))
        {
            GameContent.partyCharacters.Clear();

            string line = "";
            StreamReader sr = new StreamReader(path);

            while ((line = sr.ReadLine()) != null)
            {
                string[] csv = line.Split(',');

                int saveDataSignifier = int.Parse(csv[0]);

                if (saveDataSignifier == PartyCharacterSaveDataSignifier)
                {
                    PartyCharacter pc = new PartyCharacter(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]),
                        int.Parse(csv[4]), int.Parse(csv[5]), int.Parse(csv[6]));

                    GameContent.partyCharacters.AddLast(pc);
                }
                else if (saveDataSignifier == EquipmentSaveDataSignifier)
                {
                    GameContent.partyCharacters.Last.Value.equipment.AddLast(int.Parse(csv[1]));
                    //GameContent.partyCharacters.equipment.Last.Value.AddLast(int.Parse(csv[1]))
                }
            }

            sr.Close();
        }

        GameContent.RefreshUI();

    }

    





}




// Task list!
//
// Create a class to package a file name and an index
// save party with dummy save file
// load party with dummy save file
// Write PartyIndicesAndNames.txt
// [file.index],[party.name]
// Declare the file name as a constant
// Open the file
// Write the index + "," + partyname.txt
// Sequentially(依次) create a new index for each new party - maintain a last used index(int)
// When we create a new index, ++ our last used index counter
// Save/Load our index last used counter
// Maintain a last used index (int)
// Save party with file name of given index

// Load party with file name of given index
// 
// 
// Also, what if there's two parties named the same thing?
// What if party name has comma in it?

/*********************************************************************************************************/

// What do we need to do, in small steps, to save/load data?
// [DONE]
// - Figure how we are formatting out data.
// - Where are we saving data?  -- Application.dataPath
// - Write data into a test file


// ..Loading stuffs..
// 1. Find the file
// 2. Open the file
// 3. Instantiate the reader
// 4. Goline by line, then stat by stat
//

//
//
//
//
//
// do something to manage version of save