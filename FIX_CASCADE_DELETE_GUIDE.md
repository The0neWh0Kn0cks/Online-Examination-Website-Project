# Fix Multiple Cascade Paths Error - Migration Guide

## ? Problem Fixed

The multiple cascade paths error has been resolved by configuring the `Attempt` entity relationships with `DeleteBehavior.Restrict`.

## ?? What Changed

### Updated File: `OnlineExaminationDbContext.cs`

Added Fluent API configuration in the `OnModelCreating` method:

```csharp
// === FIX: Resolve Multiple Cascade Paths Error for Attempts Table ===
modelBuilder.Entity<Attempt>()
    .HasOne(a => a.User)
    .WithMany(u => u.Attempts)
    .HasForeignKey(a => a.UserId)
    .OnDelete(DeleteBehavior.Restrict);  // Prevent automatic deletion

modelBuilder.Entity<Attempt>()
    .HasOne(a => a.Exam)
    .WithMany(e => e.Attempts)
    .HasForeignKey(a => a.ExamId)
    .OnDelete(DeleteBehavior.Restrict);  // Prevent automatic deletion
```

## ?? Next Steps - Apply Migration

### Option 1: Remove Old Migration and Create New One (Recommended for Fresh Database)

If you haven't applied the `InitialCreate` migration yet, follow these steps:

#### Step 1: Delete the Old Migration
```powershell
# Delete the migration file manually or use:
Remove-Migration
```

#### Step 2: Create a New Migration
```powershell
Add-Migration InitialCreateFixed
```

#### Step 3: Update Database
```powershell
Update-Database
```

---

### Option 2: Create a New Migration (If Database Already Exists)

If you've already applied the `InitialCreate` migration:

#### Step 1: Create a New Migration
```powershell
Add-Migration FixAttemptsCascadeDelete
```

This will generate a migration that modifies the existing foreign keys.

#### Step 2: Review the Generated Migration
The migration should contain SQL to drop and recreate the foreign keys:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropForeignKey(
        name: "FK_Attempts_Exams_ExamId",
        table: "Attempts");

    migrationBuilder.DropForeignKey(
        name: "FK_Attempts_Users_UserId",
        table: "Attempts");

    migrationBuilder.AddForeignKey(
        name: "FK_Attempts_Exams_ExamId",
        table: "Attempts",
        column: "ExamId",
        principalTable: "Exams",
        principalColumn: "Id",
        onDelete: ReferentialAction.Restrict);

    migrationBuilder.AddForeignKey(
        name: "FK_Attempts_Users_UserId",
        table: "Attempts",
        column: "UserId",
        principalTable: "Users",
        principalColumn: "Id",
        onDelete: ReferentialAction.Restrict);
}
```

#### Step 3: Update Database
```powershell
Update-Database
```

---

### Using .NET CLI (Alternative)

If you're using the .NET CLI instead of Package Manager Console:

```bash
# Remove migration
dotnet ef migrations remove

# Add new migration
dotnet ef migrations add FixAttemptsCascadeDelete

# Update database
dotnet ef database update
```

---

## ?? What Does This Fix?

### Before (Cascade Delete - Problem)
```
User (Id: 1) ©¤©¤CASCADE©¤©¤> Attempts
                              ¡ý
Exam (Id: 1) ©¤©¤CASCADE©¤©¤> Attempts (CONFLICT!)
```

**Problem**: SQL Server detects multiple cascade paths to `Attempts` table, which could cause unintended data deletion.

### After (Restrict Delete - Solution)
```
User (Id: 1) ©¤©¤RESTRICT©¤©¤> Attempts
                              ¡ý
Exam (Id: 1) ©¤©¤RESTRICT©¤©¤> Attempts (SAFE!)
```

**Solution**: 
- Deleting a `User` will **NOT** automatically delete their `Attempts`
- Deleting an `Exam` will **NOT** automatically delete related `Attempts`
- You must manually handle orphaned records if needed

---

## ?? Behavioral Changes

### Scenario 1: Deleting a User
**Before**: All attempts by that user would be automatically deleted.
**After**: You'll get an error if attempts exist. You must delete attempts first.

```csharp
// Handle user deletion properly
var userAttempts = await context.Attempts
    .Where(a => a.UserId == userId)
    .ToListAsync();

context.Attempts.RemoveRange(userAttempts);  // Delete attempts first
await context.SaveChangesAsync();

var user = await context.Users.FindAsync(userId);
context.Users.Remove(user);  // Then delete user
await context.SaveChangesAsync();
```

### Scenario 2: Deleting an Exam
**Before**: All attempts for that exam would be automatically deleted.
**After**: You'll get an error if attempts exist. You must delete attempts first.

```csharp
// Handle exam deletion properly
var examAttempts = await context.Attempts
    .Where(a => a.ExamId == examId)
    .ToListAsync();

context.Attempts.RemoveRange(examAttempts);  // Delete attempts first
await context.SaveChangesAsync();

var exam = await context.Exams.FindAsync(examId);
context.Exams.Remove(exam);  // Then delete exam
await context.SaveChangesAsync();
```

---

## ?? Verification Steps

After applying the migration, verify the changes:

### 1. Check Database Schema
Run this SQL query to verify the foreign keys:

```sql
SELECT 
    fk.name AS ForeignKeyName,
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn,
    fk.delete_referential_action_desc AS DeleteAction
FROM sys.foreign_keys AS fk
INNER JOIN sys.tables AS tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables AS tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns AS cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.columns AS cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
WHERE tp.name = 'Attempts';
```

**Expected Result**:
- `FK_Attempts_Users_UserId` ¡ú DeleteAction: **NO_ACTION** (Restrict)
- `FK_Attempts_Exams_ExamId` ¡ú DeleteAction: **NO_ACTION** (Restrict)

### 2. Test Deletion Behavior
```csharp
// This should now throw an exception if attempts exist
try 
{
    var user = await context.Users.FindAsync(1);
    context.Users.Remove(user);
    await context.SaveChangesAsync();  // Will fail if user has attempts
}
catch (DbUpdateException ex)
{
    Console.WriteLine("Cannot delete user with existing attempts");
}
```

---

## ?? Additional Recommendations

### Option 1: Soft Delete (Recommended for Production)
Instead of hard deleting records, mark them as deleted:

```csharp
// Add to BaseDomainModel
public bool IsDeleted { get; set; }
public DateTime? DeletedDate { get; set; }
```

### Option 2: Use SetNull Instead of Restrict
If you want to keep attempts but remove the relationship:

```csharp
modelBuilder.Entity<Attempt>()
    .HasOne(a => a.User)
    .WithMany(u => u.Attempts)
    .HasForeignKey(a => a.UserId)
    .OnDelete(DeleteBehavior.SetNull);  // Set UserId to NULL when user is deleted
```

**Note**: This requires `UserId` to be nullable (`int?`).

---

## ?? Delete Behavior Options

| Behavior | Description | When to Use |
|----------|-------------|-------------|
| `Cascade` | Automatically delete dependent records | When child records have no meaning without parent |
| `Restrict` | Prevent deletion if dependent records exist | When you want to preserve historical data |
| `SetNull` | Set foreign key to NULL when parent deleted | When relationship is optional |
| `NoAction` | Similar to Restrict (SQL Server default) | Database-level constraint |

---

## ? Summary

1. ? Modified `OnModelCreating` method
2. ? Build successful
3. ? Next: Create and apply migration
4. ? Next: Test deletion behavior
5. ? Next: Update any existing delete logic in services

---

## ?? Troubleshooting

### Error: "There is already an object named 'FK_Attempts_Users_UserId'"
**Solution**: The migration might be trying to create a constraint that already exists.
```powershell
# Drop the database and recreate
Drop-Database
Update-Database
```

### Error: "Could not drop constraint"
**Solution**: There might be existing data causing conflicts.
```sql
-- Temporarily disable constraints
ALTER TABLE Attempts NOCHECK CONSTRAINT ALL;
-- Apply migration
-- Re-enable constraints
ALTER TABLE Attempts CHECK CONSTRAINT ALL;
```

---

## ?? Done!

Your database schema is now properly configured to avoid multiple cascade paths errors. The `Attempts` table relationships are set to `Restrict`, ensuring data integrity and preventing accidental deletions.
