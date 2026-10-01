using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.ValueObjects;

public record Path
{
    public const char SEPARATOR = '.';

    private readonly List<Slug> _slugs = [];

    private Path(IEnumerable<Slug> slugs)
    {
        _slugs = slugs.ToList();
    }

    public string Value => string.Join(SEPARATOR, _slugs);
    public IReadOnlyList<Slug> Slugs => _slugs;


    public static Result<Path, Failure> Create(params IEnumerable<Slug> slugs) => 
        new Path(slugs);

    public static Result<Path, Failure> Create(string value)
    {
        var valueParts = value.Split(SEPARATOR);
        var slugs = new List<Slug>();

        foreach (var valuePart in valueParts)
        {
            slugs.Add(Slug.Create(valuePart).Value);
        }

        return new Path(slugs);
    }

    public bool IsChildren(Path parentPath)
    {
        if (parentPath.Slugs.Count > Slugs.Count)
            return false;

        for(int i = 0; i < parentPath.Slugs.Count; i++)
        {
            if (Slugs[i] != parentPath.Slugs[i])
                return false;
        }

        return true;
    }

    public override string ToString() => Value;

    public static implicit operator string(Path path) => path.Value;
}