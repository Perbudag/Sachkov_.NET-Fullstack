using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class ChangeDepartmentParentConcurrencyTests : DirectoryBaseTests
{
    public ChangeDepartmentParentConcurrencyTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Change_department_parent_Concurrently_To_different_parents_Should_leave_consistent_tree()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var parentXId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Родитель X",
            "parent-x",
            null,
            cancellationToken));

        var parentYId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Родитель Y",
            "parent-y",
            null,
            cancellationToken));

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение A",
            "department-a",
            null,
            cancellationToken));

        var childId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Потомок A1",
            "child-a1",
            departmentId,
            cancellationToken));

        var grandChildId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Потомок A2",
            "grand-child-a2",
            childId,
            cancellationToken));

        var moveToX = new MoveDepartmentRequest(parentXId);
        var moveToY = new MoveDepartmentRequest(parentYId);

        // act
        var moveToXTask = HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            moveToX,
            cancellationToken);

        var moveToYTask = HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            moveToY,
            cancellationToken);

        var responses = await Task.WhenAll(moveToXTask, moveToYTask);

        var envelopes = await Task.WhenAll(
            responses.Select(response =>
                response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(cancellationToken)));

        var state = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == parentXId ||
                            d.Id == parentYId ||
                            d.Id == departmentId ||
                            d.Id == childId ||
                            d.Id == grandChildId)
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth
                })
                .ToListAsync(cancellationToken));

        // assert
        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
        Assert.All(envelopes, envelope => Assert.False(envelope?.IsError));

        var movedDepartment = Assert.Single(state, d => d.Id == departmentId);
        var movedChild = Assert.Single(state, d => d.Id == childId);
        var movedGrandChild = Assert.Single(state, d => d.Id == grandChildId);
        var parentX = Assert.Single(state, d => d.Id == parentXId);
        var parentY = Assert.Single(state, d => d.Id == parentYId);

        Assert.True(
            movedDepartment.ParentId == parentXId ||
            movedDepartment.ParentId == parentYId);

        var expectedParentPath = movedDepartment.ParentId == parentXId
            ? parentX.Path
            : parentY.Path;

        var expectedPath = $"{expectedParentPath}.department-a";

        Assert.Equal(expectedPath, movedDepartment.Path);
        Assert.Equal(1, movedDepartment.Depth);

        Assert.Equal(departmentId, movedChild.ParentId);
        Assert.Equal($"{expectedPath}.child-a1", movedChild.Path);
        Assert.Equal(2, movedChild.Depth);

        Assert.Equal(childId, movedGrandChild.ParentId);
        Assert.Equal($"{expectedPath}.child-a1.grand-child-a2", movedGrandChild.Path);
        Assert.Equal(3, movedGrandChild.Depth);

        Assert.Null(parentX.ParentId);
        Assert.Equal(0, parentX.Depth);
        Assert.Null(parentY.ParentId);
        Assert.Equal(0, parentY.Depth);

        AssertNoParentCycle(
            state.Select(d => (d.Id, d.ParentId)).ToDictionary(x => x.Id, x => x.ParentId));
    }

    [Fact]
    public async Task Change_department_parent_Concurrently_into_each_other_Should_allow_one_move_and_reject_the_other_as_cycle()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var departmentAId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение A",
            "department-a",
            null,
            cancellationToken));

        var departmentBId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение B",
            "department-b",
            null,
            cancellationToken));

        var moveAToB = new MoveDepartmentRequest(departmentBId);
        var moveBToA = new MoveDepartmentRequest(departmentAId);

        // act
        var moveAToBTask = HttpClient.PutAsJsonAsync(
            $"departments/{departmentAId}/parent",
            moveAToB,
            cancellationToken);

        var moveBToATask = HttpClient.PutAsJsonAsync(
            $"departments/{departmentBId}/parent",
            moveBToA,
            cancellationToken);

        var responses = await Task.WhenAll(moveAToBTask, moveBToATask);

        var envelopes = await Task.WhenAll(
            responses.Select(response =>
                response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(cancellationToken)));

        var state = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentAId || d.Id == departmentBId)
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth
                })
                .ToListAsync(cancellationToken));

        // assert
        Assert.Equal(
            1,
            responses.Count(response => response.StatusCode == HttpStatusCode.OK));

        Assert.Equal(
            1,
            responses.Count(response => response.StatusCode == HttpStatusCode.Conflict));

        var successEnvelope = Assert.Single(envelopes, envelope => envelope?.IsError == false);

        var cycleEnvelope = Assert.Single(envelopes, envelope => envelope?.IsError == true);

        Assert.Contains(cycleEnvelope!.Errors!, error =>
            string.Equals(error.Code, "department.move.cycle", StringComparison.Ordinal));

        var departmentA = Assert.Single(state, d => d.Id == departmentAId);
        var departmentB = Assert.Single(state, d => d.Id == departmentBId);

        var successId = successEnvelope!.Result!.Id;

        if (successId == departmentAId)
        {
            Assert.Equal(departmentBId, departmentA.ParentId);
            Assert.Null(departmentB.ParentId);
            Assert.Equal("department-b.department-a", departmentA.Path);
            Assert.Equal(1, departmentA.Depth);
            Assert.Equal("department-b", departmentB.Path);
            Assert.Equal(0, departmentB.Depth);
        }
        else
        {
            Assert.Equal(departmentAId, departmentB.ParentId);
            Assert.Null(departmentA.ParentId);
            Assert.Equal("department-a.department-b", departmentB.Path);
            Assert.Equal(1, departmentB.Depth);
            Assert.Equal("department-a", departmentA.Path);
            Assert.Equal(0, departmentA.Depth);
        }

        AssertNoParentCycle(
            state.Select(d => (d.Id, d.ParentId)).ToDictionary(x => x.Id, x => x.ParentId));
    }

    private static void AssertNoParentCycle(IReadOnlyDictionary<Guid, Guid?> parentById)
    {
        foreach (var startId in parentById.Keys)
        {
            var visited = new HashSet<Guid>();
            var currentId = startId;

            while (true)
            {
                Assert.True(
                    visited.Add(currentId),
                    $"A parent cycle was detected starting from department '{startId}'.");

                if (!parentById.TryGetValue(currentId, out var parentId) || !parentId.HasValue)
                    break;

                Assert.True(
                    parentById.ContainsKey(parentId.Value),
                    $"Parent '{parentId}' for department '{currentId}' is missing from the test state.");

                currentId = parentId.Value;
            }
        }

    }
}
