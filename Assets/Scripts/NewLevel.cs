using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelCategory
{
    [EnumName("Beginner")]      BEGINNER    = 0 ,
    [EnumName("Original")]      ORIGINAL    = 1 ,
    [EnumName("Five Words")]    ADVANCED    = 2 ,
    [EnumName("Six Words")]     SIX_WORD    = 3 ,
    [EnumName("Daily")]         DAILY       = 4 ,
    [EnumName("null")]          NULL        = 5
};

[System.Serializable]
public class NewLevel
{
    public LevelCategory    Category;
    public string           Theme;
    public int              LevelNumber;
    public List<string>     Words;
    public string           SecretWord;
    public DateTime         Date;

    public bool             IsNull          { get { return Category == LevelCategory.NULL; } }
    public bool             HasSecretWord   { get { return !string.IsNullOrEmpty(SecretWord); } }

    public NewLevel()
    {
        Category            = LevelCategory.NULL;
        Theme               = string.Empty;
        LevelNumber         = -1;
        Words               = new List<string>();
        SecretWord          = string.Empty;
        Date                = new DateTime(1900, 1, 1);
    }

    public NewLevel(LevelCategory category, string theme, int levelNumber, List<string> words, string secretWord)
    {
        Category = category;
        Theme = theme;
        LevelNumber = levelNumber;
        Words = words;
        SecretWord = secretWord;
    }

    public NewLevel(LevelCategory category, string theme, int levelNumber, List<string> words, string secretWord, DateTime date)
    {
        Category            = category;
        Theme               = theme;
        LevelNumber         = levelNumber;
        Words               = words;
        SecretWord          = secretWord;

        if (Category == LevelCategory.DAILY)
            Date = date;
    }
}

public static class LevelDefinitions
{
    public static List<NewLevel> ALL_LEVELS = new List<NewLevel>()
    {
        new NewLevel(LevelCategory.BEGINNER, "Test", 0, new List<string>() { "aa", "bbb", "cccc", "dddd" }, "Bad"),
        new NewLevel(LevelCategory.BEGINNER, "Test2", 1, new List<string>() { "xx", "yyy", "zzzz", "adam" }, "Yam"),

        new NewLevel(LevelCategory.ORIGINAL, "Ocean", 1, new List<string>() { "Crab", "Beach", "Dolphin", "Seahorse" }, "Sea"),
        new NewLevel(LevelCategory.ORIGINAL, "Forest", 2, new List<string>() { "Bear", "Trail", "Hiking", "Campfire" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Desert", 3, new List<string>() { "Sand", "Snake", "Cactus", "Drought" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Weather", 4, new List<string>() { "Wind", "Cloud", "Sunny", "Drizzle" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "School", 5, new List<string>() { "Exam", "Friend", "Library", "Homework" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Music", 6, new List<string>() { "Bass", "Tempo", "Violin", "Trombone" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Sports", 7, new List<string>() { "Puck", "Track", "Lacrosse", "Football" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Nature", 8, new List<string>() { "Leaf", "Ocean", "Sunrise", "Rainbow" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Farm", 9, new List<string>() { "Bull", "Horse", "Tractor", "Chicken" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "City", 10, new List<string>() { "Tall", "Noisy", "Subway", "Building" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Home", 11, new List<string>() { "Yard", "Couch", "Kitchen", "Bedroom" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Transportation", 12, new List<string>() { "Bike", "Train", "Truck", "Airplane" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Kingdom", 13, new List<string>() { "Flag", "Crown", "Invader", "Warrior" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Earth", 14, new List<string>() { "Land", "World", "Ground", "Culture" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Food", 15, new List<string>() { "Soup", "Curry", "Citrus", "Sausage" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Technology", 16, new List<string>() { "Disk", "Pixel", "Binary", "Computer" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Science", 17, new List<string>() { "Atom", "Light", "Gravity", "Physics" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Animals", 18, new List<string>() { "Fish", "Panda", "Rabbit", "Raccoon" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Plants", 19, new List<string>() { "Herb", "Tulip", "Orchid", "Pumpkin" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Health", 20, new List<string>() { "Pill", "Nurse", "Doctor", "Hospital" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Emotions", 21, new List<string>() { "Pain", "Anger", "Lonely", "Grateful" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Clothing", 22, new List<string>() { "Vest", "Shoes", "Button", "Sweater" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Jobs", 23, new List<string>() { "Lift", "Miner", "Police", "Plumber" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Finance", 24, new List<string>() { "Loan", "Price", "Invest", "Banking" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Architecture", 25, new List<string>() { "Arch", "Tower", "Column", "Stadium" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Professions", 26, new List<string>() { "Cook", "Nurse", "Dentist", "Surgeon" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Family", 27, new List<string>() { "Aunt", "Niece", "Cousin", "Grandma" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Generation", 28, new List<string>() { "Year", "Alpha", "Elder", "Ancient" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Literature", 29, new List<string>() { "Poem", "Novel", "Prose", "Fiction" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Geography", 30, new List<string>() { "East", "Ocean", "Valley", "Meadow" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Government", 31, new List<string>() { "Bill", "Mayor", "Senate", "Embassy" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Industry", 32, new List<string>() { "Coal", "Steel", "Petrol", "Factory" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Science Fiction", 33, new List<string>() { "Beam", "Alien", "Galaxy", "Cyborg" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Philosophy", 34, new List<string>() { "Ethic", "Logic", "Dilemma", "Fallacy" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Arts", 35, new List<string>() { "Play", "Photo", "Canvas", "Gallery" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Warfare", 36, new List<string>() { "Bomb", "Rifle", "Bunker", "Barracks" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Crime", 37, new List<string>() { "Hack", "Fraud", "Kidnap", "Robbery" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Justice", 38, new List<string>() { "Fair", "Trurh", "Innocent", "Verdict" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Politics", 39, new List<string>() { "Vote", "Party", "Debate", "Congress" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Religion", 40, new List<string>() { "Pray", "Faith", "Belief", "Doctrine" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Mythology", 41, new List<string>() { "Zeus", "Totem", "Divine", "Fantasy" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Geology", 42, new List<string>() { "Rock", "Fault", "Crystal", "Mineral" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Astronomy", 43, new List<string>() { "Mars", "Venus", "Saturn", "Mercury" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Chemistry", 44, new List<string>() { "Salt", "Oxide", "Enzyme", "Protein" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Physics", 45, new List<string>() { "Heat", "Light", "Plasma", "Quantum" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Performing Arts", 46, new List<string>() { "Mime", "Opera", "Ballet", "Orchestra" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Biology", 47, new List<string>() { "Cell", "Nerve", "Organ", "Neuron" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Mathematics", 48, new List<string>() { "Sqrt", "Angle", "Vertex", "Formula" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Engineering", 49, new List<string>() { "Gear", "Laser", "Sensor", "Battery" }, "Abc"),
        new NewLevel(LevelCategory.ORIGINAL, "Technology", 50, new List<string>() { "Code", "Pixel", "Iphone", "Computer" }, "Abc"),

        new NewLevel(LevelCategory.ADVANCED, "Cars", 97, new List<string>() { "Car", "Auto", "Truck", "Driver", "Minivan", "Combustion", "Convertible" }, "Vroom"),
    };

    public static int LevelCount(LevelCategory category)
    {
        return ALL_LEVELS.FindAll(x => x.Category == category).Count;
    }

    public static int SecretCount(LevelCategory category)
    {
        return ALL_LEVELS.FindAll(x => x.Category == category && x.HasSecretWord).Count;
    }
}