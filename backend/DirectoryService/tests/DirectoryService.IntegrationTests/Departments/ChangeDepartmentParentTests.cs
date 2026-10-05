using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Entities;
using DirectoryService.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Shared.Framework.Endpoints;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class ChangeDepartmentParentTests : DirectoryBaseTests
{
    public ChangeDepartmentParentTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Change_department_parent_With_descendants_Should_move_whole_subtree()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var oldParentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Старый родитель",
            "old-parent",
            null,
            cancellationToken));

        var newParentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Новый родитель",
            "new-parent",
            null,
            cancellationToken));

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение",
            "department",
            oldParentId,
            cancellationToken));

        var childId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Дочернее подразделение",
            "child",
            departmentId,
            cancellationToken));

        var grandChildId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Внук",
            "grand-child",
            childId,
            cancellationToken));

        var request = new MoveDepartmentRequest(newParentId);

        // act
        var response = await HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            request,
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
            cancellationToken);

        var state = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId || d.Id == childId || d.Id == grandChildId)
                .OrderBy(d => d.Depth)
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth
                })
                .ToListAsync(cancellationToken));

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(envelope?.IsError);
        Assert.Equal(departmentId, envelope?.Result?.Id);
        Assert.Equal(newParentId, envelope?.Result?.ParentId);
        Assert.Equal("new-parent.department", envelope?.Result?.Path);
        Assert.Equal(1, envelope?.Result?.Depth);

        var movedDepartment = Assert.Single(state, d => d.Id == departmentId);
        var movedChild = Assert.Single(state, d => d.Id == childId);
        var movedGrandChild = Assert.Single(state, d => d.Id == grandChildId);

        Assert.Equal(newParentId, movedDepartment.ParentId);
        Assert.Equal("new-parent.department", movedDepartment.Path);
        Assert.Equal(1, movedDepartment.Depth);

        Assert.Equal(departmentId, movedChild.ParentId);
        Assert.Equal("new-parent.department.child", movedChild.Path);
        Assert.Equal(2, movedChild.Depth);

        Assert.Equal(childId, movedGrandChild.ParentId);
        Assert.Equal("new-parent.department.child.grand-child", movedGrandChild.Path);
        Assert.Equal(3, movedGrandChild.Depth);

        var oldParentPath = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == oldParentId)
                .Select(d => d.Path.ToString())
                .FirstAsync(cancellationToken));

        Assert.Equal("old-parent", oldParentPath);
    }

    [Fact]
    public async Task Change_department_parent_To_root_With_descendants_Should_move_whole_subtree_to_root()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var oldParentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Старый родитель",
            "old-parent",
            null,
            cancellationToken));

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение",
            "department",
            oldParentId,
            cancellationToken));

        var childId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Дочернее подразделение",
            "child",
            departmentId,
            cancellationToken));

        var grandChildId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Внук",
            "grand-child",
            childId,
            cancellationToken));

        var request = new MoveDepartmentRequest(null);

        // act
        var response = await HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            request,
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
            cancellationToken);

        var state = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId || d.Id == childId || d.Id == grandChildId)
                .OrderBy(d => d.Depth)
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth
                })
                .ToListAsync(cancellationToken));

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(envelope?.IsError);
        Assert.Equal(departmentId, envelope?.Result?.Id);
        Assert.Null(envelope?.Result?.ParentId);
        Assert.Equal("department", envelope?.Result?.Path);
        Assert.Equal(0, envelope?.Result?.Depth);

        Assert.Equal(3, state.Count);

        var rootDepartment = Assert.Single(state, d => d.Id == departmentId);
        var rootChild = Assert.Single(state, d => d.Id == childId);
        var rootGrandChild = Assert.Single(state, d => d.Id == grandChildId);

        Assert.Null(rootDepartment.ParentId);
        Assert.Equal("department", rootDepartment.Path);
        Assert.Equal(0, rootDepartment.Depth);

        Assert.Equal(departmentId, rootChild.ParentId);
        Assert.Equal("department.child", rootChild.Path);
        Assert.Equal(1, rootChild.Depth);

        Assert.Equal(childId, rootGrandChild.ParentId);
        Assert.Equal("department.child.grand-child", rootGrandChild.Path);
        Assert.Equal(2, rootGrandChild.Depth);

        var oldParentPath = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == oldParentId)
                .Select(d => d.Path.ToString())
                .FirstAsync(cancellationToken));

        Assert.Equal("old-parent", oldParentPath);
    }

    [Fact]
    public async Task Change_department_parent_When_new_parent_is_descendant_Should_return_cycle_error()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var rootId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Корень",
            "root",
            null,
            cancellationToken));

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение",
            "department",
            rootId,
            cancellationToken));

        var childId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Ребёнок",
            "child",
            departmentId,
            cancellationToken));

        var grandChildId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Внук",
            "grand-child",
            childId,
            cancellationToken));

        var stateBefore = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId || d.Id == childId || d.Id == grandChildId)
                .OrderBy(d => d.Depth)
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth
                })
                .ToListAsync(cancellationToken));

        var request = new MoveDepartmentRequest(grandChildId);

        // act
        var response = await HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            request,
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
            cancellationToken);

        var stateAfter = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId || d.Id == childId || d.Id == grandChildId)
                .OrderBy(d => d.Depth)
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth
                })
                .ToListAsync(cancellationToken));

        // assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e =>
            string.Equals(e.Code, "department.move.cycle", StringComparison.Ordinal));
        Assert.Equal(stateBefore.Count, stateAfter.Count);

        for (var i = 0; i < stateBefore.Count; i++)
        {
            Assert.Equal(stateBefore[i].Id, stateAfter[i].Id);
            Assert.Equal(stateBefore[i].ParentId, stateAfter[i].ParentId);
            Assert.Equal(stateBefore[i].Path, stateAfter[i].Path);
            Assert.Equal(stateBefore[i].Depth, stateAfter[i].Depth);
        }
    }

    [Fact]
    public async Task Change_department_parent_When_parent_is_self_Should_return_validation_error()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение",
            "department",
            null,
            cancellationToken));

        var pathBefore = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId)
                .Select(d => new
                {
                    Path = d.Path.ToString(),
                    d.ParentId,
                    d.Depth
                })
                .FirstAsync(cancellationToken));

        var request = new MoveDepartmentRequest(departmentId);

        // act
        var response = await HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            request,
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
            cancellationToken);

        var pathAfter = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId)
                .Select(d => new
                {
                    Path = d.Path.ToString(),
                    d.ParentId,
                    d.Depth
                })
                .FirstAsync(cancellationToken));

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e =>
            string.Equals(e.Code, "department.move.parent_is_self", StringComparison.Ordinal));

        Assert.Equal(pathBefore.Path, pathAfter.Path);
        Assert.Equal(pathBefore.ParentId, pathAfter.ParentId);
        Assert.Equal(pathBefore.Depth, pathAfter.Depth);
    }

    [Fact]
    public async Task Change_department_parent_When_parent_is_soft_deleted_Should_return_conflict()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var sourceParentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Исходный родитель",
            "source-parent",
            null,
            cancellationToken));

        var deletedParentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Удалённый родитель",
            "deleted-parent",
            null,
            cancellationToken));

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение",
            "department",
            sourceParentId,
            cancellationToken));

        await ExecuteInDbAsync(async db =>
        {
            var deletedParent = await db.Departments
                .FirstAsync(d => d.Id == deletedParentId, cancellationToken);

            deletedParent.SoftDelete();

            await db.SaveChangesAsync(cancellationToken);
        });

        var stateBefore = await ExecuteInDbAsync(db =>
            db.Departments
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(d => d.Id == departmentId)
                .Select(d => new
                {
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth
                })
                .FirstAsync(cancellationToken));

        var request = new MoveDepartmentRequest(deletedParentId);

        // act
        var response = await HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            request,
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
            cancellationToken);

        var stateAfter = await ExecuteInDbAsync(db =>
            db.Departments
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(d => d.Id == departmentId)
                .Select(d => new
                {
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth
                })
                .FirstAsync(cancellationToken));

        // assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e =>
            string.Equals(e.Code, "department.move.parent_deleted", StringComparison.Ordinal));

        Assert.Equal(stateBefore.ParentId, stateAfter.ParentId);
        Assert.Equal(stateBefore.Path, stateAfter.Path);
        Assert.Equal(stateBefore.Depth, stateAfter.Depth);
    }

    [Fact]
    public async Task Change_department_parent_When_department_not_found_Should_return_not_found()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = new MoveDepartmentRequest(null);

        // act
        var response = await HttpClient.PutAsJsonAsync(
            $"departments/{Guid.NewGuid()}/parent",
            request,
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
            cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e =>
            string.Equals(e.Code, "departments.not.found", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Change_department_parent_When_new_parent_not_found_Should_return_not_found()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var sourceParentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Исходный родитель",
            "source-parent",
            null,
            cancellationToken));

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение",
            "department",
            sourceParentId,
            cancellationToken));

        var request = new MoveDepartmentRequest(Guid.NewGuid());

        // act
        var response = await HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            request,
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
            cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e =>
            string.Equals(e.Code, "departments.not.found.parent", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Change_department_parent_When_parent_is_already_current_Should_be_idempotent()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var parentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Родитель",
            "parent",
            null,
            cancellationToken));

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Подразделение",
            "department",
            parentId,
            cancellationToken));

        var childId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Ребёнок",
            "child",
            departmentId,
            cancellationToken));

        var stateBefore = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId || d.Id == childId)
                .OrderBy(d => d.Depth)
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth,
                    d.UpdatedAt
                })
                .ToListAsync(cancellationToken));

        var request = new MoveDepartmentRequest(parentId);

        // act
        var response = await HttpClient.PutAsJsonAsync(
            $"departments/{departmentId}/parent",
            request,
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
            cancellationToken);

        var stateAfter = await ExecuteInDbAsync(db =>
            db.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId || d.Id == childId)
                .OrderBy(d => d.Depth)
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    Path = d.Path.ToString(),
                    d.Depth,
                    d.UpdatedAt
                })
                .ToListAsync(cancellationToken));

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(envelope?.IsError);
        Assert.Equal(departmentId, envelope?.Result?.Id);
        Assert.Equal(parentId, envelope?.Result?.ParentId);

        Assert.Equal(stateBefore.Count, stateAfter.Count);

        for (var i = 0; i < stateBefore.Count; i++)
        {
            Assert.Equal(stateBefore[i].Id, stateAfter[i].Id);
            Assert.Equal(stateBefore[i].ParentId, stateAfter[i].ParentId);
            Assert.Equal(stateBefore[i].Path, stateAfter[i].Path);
            Assert.Equal(stateBefore[i].Depth, stateAfter[i].Depth);
            Assert.Equal(stateBefore[i].UpdatedAt, stateAfter[i].UpdatedAt);
        }

        Assert.Equal("parent.department", envelope!.Result!.Path);
        Assert.Equal(1, envelope.Result.Depth);
    }

    [Fact]
    public async Task Change_department_parent_With_large_subtree_Should_use_set_based_update()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var oldParentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Старый родитель",
            "old-parent",
            null,
            cancellationToken));

        var newParentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Новый родитель",
            "new-parent",
            null,
            cancellationToken));

        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Большое поддерево",
            "large-subtree",
            oldParentId,
            cancellationToken));

        const int descendantCount = 500;

        await ExecuteInDbAsync(async db =>
        {
            var department = await db.Departments
                .FirstAsync(d => d.Id == departmentId, cancellationToken);

            for (var index = 1; index <= descendantCount; index++)
            {
                var descendant = Department.Create(
                    DirectoryService.Domain.ValueObjects.Name.Create($"Потомок {index:D3}").Value,
                    DirectoryService.Domain.ValueObjects.Slug.Create($"descendant-{index:D3}").Value,
                    department).Value;

                db.Departments.Add(descendant);
            }

            await db.SaveChangesAsync(cancellationToken);
        });

        await PrepareDepartmentUpdateStatementCounterAsync(cancellationToken);

        try
        {
            var request = new MoveDepartmentRequest(newParentId);

            // act
            var response = await HttpClient.PutAsJsonAsync(
                $"departments/{departmentId}/parent",
                request,
                cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<MoveDepartmentDto?>>(
                cancellationToken);

            var updateStatementCount = await GetDepartmentUpdateStatementCountAsync(cancellationToken);

            var movedDescendants = await ExecuteInDbAsync(db =>
                db.Departments
                    .AsNoTracking()
                    .Where(d => d.ParentId == departmentId)
                    .CountAsync(cancellationToken));

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.Equal(newParentId, envelope?.Result?.ParentId);
            Assert.Equal("new-parent.large-subtree", envelope?.Result?.Path);
            Assert.Equal(1, envelope?.Result?.Depth);

            Assert.Equal(descendantCount, movedDescendants);

            Assert.Equal(
                2,
                updateStatementCount);
        }
        finally
        {
            await DropDepartmentUpdateStatementCounterAsync(cancellationToken);
        }
    }

    private async Task PrepareDepartmentUpdateStatementCounterAsync(CancellationToken cancellationToken)
    {
        await ExecuteInDbAsync(async db =>
        {
            await db.Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS integration_test_department_update_counter
                (
                    id integer PRIMARY KEY,
                    update_count integer NOT NULL
                );
                """, cancellationToken);

            await db.Database.ExecuteSqlRawAsync("""
                TRUNCATE TABLE integration_test_department_update_counter;
                INSERT INTO integration_test_department_update_counter (id, update_count)
                VALUES (1, 0);
                """, cancellationToken);

            await db.Database.ExecuteSqlRawAsync("""
                CREATE OR REPLACE FUNCTION integration_test_count_department_updates()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    UPDATE integration_test_department_update_counter
                    SET update_count = update_count + 1
                    WHERE id = 1;

                    RETURN NULL;
                END;
                $$;
                """, cancellationToken);

            await db.Database.ExecuteSqlRawAsync("""
                DROP TRIGGER IF EXISTS integration_test_department_update_trigger ON departments;

                CREATE TRIGGER integration_test_department_update_trigger
                AFTER UPDATE ON departments
                FOR EACH STATEMENT
                EXECUTE FUNCTION integration_test_count_department_updates();
                """, cancellationToken);
        });
    }

    private async Task<int> GetDepartmentUpdateStatementCountAsync(CancellationToken cancellationToken)
    {
        return await ExecuteInDbAsync(async db =>
        {
            var connection = db.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT update_count
                FROM integration_test_department_update_counter
                WHERE id = 1;
                """;

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return (int)result!;
        });
    }

    private async Task DropDepartmentUpdateStatementCounterAsync(CancellationToken cancellationToken)
    {
        await ExecuteInDbAsync(async db =>
        {
            await db.Database.ExecuteSqlRawAsync(
                "DROP TRIGGER IF EXISTS integration_test_department_update_trigger ON departments;",
                cancellationToken);

            await db.Database.ExecuteSqlRawAsync(
                "DROP FUNCTION IF EXISTS integration_test_count_department_updates();",
                cancellationToken);

            await db.Database.ExecuteSqlRawAsync(
                "DROP TABLE IF EXISTS integration_test_department_update_counter;",
                cancellationToken);
        });
    }
}
