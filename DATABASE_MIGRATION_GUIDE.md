# Database Migration Guide - Multimedia & Reading Comprehension Support

## Overview
This guide explains how to update your database to support the new multimedia and reading comprehension features in the Online Examination System.

## Changes Made

### 1. Domain Model Update
**File**: `Online Examination\Domain\Question.cs`

**New Fields Added**:
- `ImageUrl` (nullable string) - For Math/Science questions with diagrams
- `ReadingPassage` (nullable string) - For Language questions with reading passages

## Database Migration Steps

### Option A: Using Entity Framework Core Migrations (Recommended)

#### Step 1: Add Migration
Open the Package Manager Console in Visual Studio and run:

```powershell
Add-Migration AddMultimediaAndReadingPassageToQuestions
```

Or using .NET CLI in terminal:

```bash
dotnet ef migrations add AddMultimediaAndReadingPassageToQuestions
```

#### Step 2: Review the Migration
The generated migration file will be created in your `Migrations` folder. It should contain:

```csharp
migrationBuilder.AddColumn<string>(
    name: "ImageUrl",
    table: "Questions",
    type: "nvarchar(max)",
    nullable: true);

migrationBuilder.AddColumn<string>(
    name: "ReadingPassage",
    table: "Questions",
    type: "nvarchar(max)",
    nullable: true);
```

#### Step 3: Update Database
Apply the migration to your database:

```powershell
Update-Database
```

Or using .NET CLI:

```bash
dotnet ef database update
```

---

### Option B: Manual SQL Script

If you prefer to update the database manually, execute this SQL script:

```sql
-- Add ImageUrl column to Questions table
ALTER TABLE Questions
ADD ImageUrl NVARCHAR(MAX) NULL;

-- Add ReadingPassage column to Questions table
ALTER TABLE Questions
ADD ReadingPassage NVARCHAR(MAX) NULL;

-- Verify columns were added
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Questions'
AND COLUMN_NAME IN ('ImageUrl', 'ReadingPassage');
```

---

## Sample Data Examples

### Example 1: Math Question with Diagram

```csharp
var mathQuestion = new Question
{
    Text = "Based on the triangle shown above, what is the value of angle x?",
    ImageUrl = "/images/questions/triangle-diagram.png",
    OptionA = "30 degrees",
    OptionB = "45 degrees",
    OptionC = "60 degrees",
    OptionD = "90 degrees",
    CorrectAnswer = "C",
    ExamId = 1
};
```

### Example 2: English Reading Comprehension Question

```csharp
var englishQuestion = new Question
{
    ReadingPassage = @"The Industrial Revolution was a period of major industrialization and innovation that took place during the late 1700s and early 1800s. The Industrial Revolution began in Great Britain and quickly spread throughout the world. This time period saw the mechanization of agriculture and textile manufacturing and a revolution in power, including steam ships and railroads, that effected social, cultural and economic conditions.",
    Text = "According to the passage, where did the Industrial Revolution begin?",
    OptionA = "United States",
    OptionB = "Great Britain",
    OptionC = "France",
    OptionD = "Germany",
    CorrectAnswer = "B",
    ExamId = 2
};
```

### Example 3: Science Question with Image

```csharp
var scienceQuestion = new Question
{
    Text = "What type of chemical reaction is shown in the diagram above?",
    ImageUrl = "/images/questions/chemical-reaction-diagram.jpg",
    OptionA = "Synthesis reaction",
    OptionB = "Decomposition reaction",
    OptionC = "Single replacement",
    OptionD = "Double replacement",
    CorrectAnswer = "A",
    ExamId = 3
};
```

### Example 4: Standard Question (No Special Content)

```csharp
var standardQuestion = new Question
{
    Text = "What is the capital of France?",
    ImageUrl = null,  // No image needed
    ReadingPassage = null,  // No passage needed
    OptionA = "London",
    OptionB = "Berlin",
    OptionC = "Paris",
    OptionD = "Madrid",
    CorrectAnswer = "C",
    ExamId = 4
};
```

---

## Frontend Display Logic

The `TakeExam.razor` page now follows this rendering order for each question:

1. **Reading Passage** (if present)
   - Displayed in a light gray box with book icon
   - Uses serif font for better readability

2. **Image** (if present)
   - Centered and responsive
   - Maximum height: 400px
   - Shadow and hover effects

3. **Question Text** (always present)
   - Standard bold text

4. **Answer Options** (always present)
   - Radio buttons A, B, C, D

---

## Image Storage Recommendations

### Option 1: wwwroot Folder (Simple)
Store images in your project:
```
Online Examination/wwwroot/images/questions/
```

Example ImageUrl: `/images/questions/math-diagram-1.png`

### Option 2: Azure Blob Storage (Production)
Store images in cloud storage:

Example ImageUrl: `https://youraccount.blob.core.windows.net/questions/math-diagram-1.png`

### Option 3: CDN (Best Performance)
Use a Content Delivery Network:

Example ImageUrl: `https://cdn.yoursite.com/questions/math-diagram-1.png`

---

## Testing Checklist

After applying the migration, test the following:

### ? Database Tests
- [ ] New columns exist in Questions table
- [ ] Existing questions are not affected (null values for new fields)
- [ ] Can insert questions with ImageUrl
- [ ] Can insert questions with ReadingPassage
- [ ] Can insert standard questions (both fields null)

### ? Frontend Tests
- [ ] Standard questions display correctly (no passage/image)
- [ ] Questions with images display properly
- [ ] Questions with reading passages display properly
- [ ] Questions with both image and passage display in correct order
- [ ] Images are responsive on mobile devices
- [ ] Radio buttons work correctly for all question types

---

## Rollback Instructions

If you need to revert the changes:

### Using EF Migrations:
```powershell
Update-Database -Migration <PreviousMigrationName>
```

### Using SQL:
```sql
ALTER TABLE Questions DROP COLUMN ImageUrl;
ALTER TABLE Questions DROP COLUMN ReadingPassage;
```

---

## Additional Notes

1. **Performance Consideration**: Store large images externally (cloud storage) rather than in the database
2. **Image Format**: Use `.jpg` for photos, `.png` for diagrams with transparency
3. **Reading Passage Length**: No hard limit, but keep passages reasonable (200-500 words)
4. **Accessibility**: Add alt text to images for screen readers
5. **Mobile Optimization**: Images automatically scale on smaller screens

---

## Support

If you encounter any issues during migration:
1. Check the database connection string in `appsettings.json`
2. Ensure your SQL Server is running
3. Verify you have permissions to alter tables
4. Review the migration logs for specific error messages
