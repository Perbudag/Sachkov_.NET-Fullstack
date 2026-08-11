using DirectoryService.Contracts.SharedDto;

namespace DirectoryService.Contracts.Locations;

public record LocationListItemDto
{
    public Guid Id { get; }
    public string Name { get; }
    public AddressDto Address { get; }
    public int DepartmentCount { get; }
    public DateTime CreatedAt { get; }

    public LocationListItemDto(Guid id, string name, AddressDto address, DateTime createdAt, int departmentCount)
    {
        Id = id;
        Name = name;
        Address = address;
        CreatedAt = createdAt;
        DepartmentCount = departmentCount;
    }

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
    private LocationListItemDto()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
    {
        
    }
}