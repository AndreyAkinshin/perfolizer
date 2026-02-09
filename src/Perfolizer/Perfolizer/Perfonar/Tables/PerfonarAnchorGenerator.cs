using System.Text;
using Perfolizer.Extensions;

namespace Perfolizer.Perfonar.Tables;

/// <summary>
/// Anchors are string ids for subsets of attributes.
/// Let's say we have multiple attributes defining a job (e.g., RuntimeMoniker, Jit, Platform, etc.).
/// Instead of listing all the attributes in the table, we extract this subset and mark it with an anchor.
/// For example, we can introduce "@MyAnchor: Runtime = Net70, Jit = RyuJit" above a table
///   and reference it in the Job column as "MyAnchor".
/// </summary>
/// <param name="maxAnchorLength"></param>
public class PerfonarAnchorGenerator(int? maxAnchorLength = null)
{
    private const int DefaultMaxAnchorLength = 20;
    private int MaxAnchorLength { get; } = maxAnchorLength ?? DefaultMaxAnchorLength;
    private readonly Dictionary<string, string> attributeIdToAnchor = new();
    private readonly HashSet<string> existingAnchors = [];

    public string GetAnchor(string attributeId)
    {
        if (attributeIdToAnchor.TryGetValue(attributeId, out string? existingAnchor))
            return existingAnchor;

        string anchor = Compress(attributeId);
        if (anchor.Length > MaxAnchorLength || existingAnchors.Contains(anchor))
            anchor = GenerateRandomAnchor(attributeId);

        attributeIdToAnchor[attributeId] = anchor;
        existingAnchors.Add(anchor);
        return anchor;
    }

    private static string Compress(string s)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (char.IsLetterOrDigit(c))
                builder.Append(c);
            if (c == '_' && i != s.Length - 1 && char.IsDigit(s[i + 1]))
                builder.Append(c);
        }
        return builder.ToString();
    }

    private string GenerateRandomAnchor(string attributeId)
    {
        int animalIndex = GetStableHashCode(attributeId).Abs() % animals.Count;
        string anchor = animals[animalIndex];
        int suffix = 1;
        while (existingAnchors.Contains(anchor))
            anchor = animals[animalIndex] + suffix++;
        return anchor;
    }

    // We do not use string.GetHashCode() because it is randomized on each runtime start
    private static int GetStableHashCode(string s) => s.Aggregate(0, (current, c) => current * 31 + c);

    // ReSharper disable StringLiteralTypo
    private readonly List<string> animals =
    [
        "Cat", "Dog", "Fox", "Bear", "Wolf", "Hawk", "Crow", "Ant", "Bee",
        "Deer", "Boar", "Lynx", "Mole", "Swan", "Frog", "Toad", "Duck", "Eel",
        "Carp", "Lion", "Moth", "Crab", "Emu", "Seal", "Mink", "Pike", "Rat",
        "Bat", "Yak", "Cod", "Hare", "Koi", "Elk", "Ram", "Bug", "Hen", "Owl",
        "Ape", "Gnu", "Pug", "Jay", "Doe", "Roe", "Kid", "Cub", "Colt", "Foal",
        "Calf", "Lamb", "Tuna", "Wasp", "Mite", "Lark", "Clam", "Sole", "Roach",
        "Squid", "Quail", "Vole", "Shrew", "Loon", "Rail", "Mussel", "Skink",
        "Goby", "Orca", "Mako", "Coati", "Zebu", "Tahr", "Egret", "Ibex", "Kudu",
        "Erne", "Dace", "Reed", "Tern", "Ruff", "Smelt", "Bison", "Brant", "Brook",
        "Dunlin", "Finch", "Gecko", "Ghoul", "Goral", "Grebe", "Grouse", "Guil",
        "Hoatzin", "Hyena", "Iguana", "Indri", "Jackal", "Jaguar", "Jerboa",
        "Kakapo", "Kestrel", "Kinkajou", "Koala", "Kob", "Kook", "Lapwing",
        "Lemur", "Leopard", "Liger", "Loris", "Lynx", "Macaw", "Magpie", "Mallard",
        "Manatee", "Marten", "Meerkat", "Minke", "Mongoose", "Monkey", "Moorhen",
        "Narwhal", "Ocelot", "Octopus", "Okapi", "Opossum", "Orang", "Oryx",
        "Osprey", "Otter", "Ouzel", "Panda", "Panther", "Parrot", "Peafowl",
        "Pelican", "Penguin", "Perch", "Petrel", "Pheasant", "Pigeon", "Pika",
        "Piranha", "Platypus", "Plover", "Polaris", "Pony", "Porpoise", "Possum",
        "Puffin", "Python", "Quagga", "Quokka", "Rabbit", "Raccoon", "Rail",
        "Ramora", "Ratel", "Raven", "Redpoll", "Reindeer", "Rhea", "Robin",
        "Rooster", "Sable", "Saiga", "Salamander", "Salmon", "Sandpiper", "Sardine",
        "Scallop", "Scarab", "Seahorse", "Serval", "Shark", "Sheep", "Shelduck",
        "Shiner", "Siskin", "Skate", "Skunk", "Sloth", "Snail", "Snake", "Snipe",
        "Sparrow", "Spider", "Spoonbill", "Sprat", "Squab", "Squirrel", "Starling",
        "Stilt", "Stingray", "Stoat", "Stork", "Sunfish", "Swallow", "Swan",
        "Tamarin", "Tapir", "Tarpon", "Tarsier", "Teal", "Tenrec", "Tetra",
        "Thrush", "Tiger", "Titmouse", "Toad", "Topi", "Toucan", "Turtle", "Uakari",
        "Urchin", "Vicuna", "Viper", "Vulture", "Walrus", "Warbler", "Waxwing",
        "Weasel", "Whale", "Whimbrel", "Wigeon", "Willet", "Wombat", "Woodcock",
        "Woodpecker", "Wren", "Yak", "Zander", "Zebra"
    ];
}