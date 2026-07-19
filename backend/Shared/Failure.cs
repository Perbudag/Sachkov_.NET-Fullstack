using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Shared;

public class Failure : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    public Failure(IEnumerable<Error> errors)
    {
        _errors = [.. errors];
    }

    public IEnumerator<Error> GetEnumerator() =>
        _errors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        _errors.GetEnumerator();

    public static implicit operator Failure(Collection<Error> errors) =>
        new(errors);

    public static implicit operator Failure(Error error) =>
        new([error]);
}
