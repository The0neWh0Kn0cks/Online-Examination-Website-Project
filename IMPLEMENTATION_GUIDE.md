# Online Examination System - User Journey Implementation

## Overview
This implementation provides a complete user journey for the Online Examination System:
**Login ¡ú Select Exam ¡ú Take Exam ¡ú Auto-grading ¡ú View Result**

## Technical Stack
- **Framework**: .NET 8 Blazor Web App (Interactive Server Mode)
- **Database**: Entity Framework Core with SQL Server
- **State Management**: UserSession (Scoped Service)
- **Business Logic**: StudentService

---

## Components Implemented

### 1. Services

#### UserSession.cs
**Location**: `Online Examination\Services\UserSession.cs`

**Purpose**: Scoped service to maintain user session state across components.

**Key Properties**:
- `CurrentUserId`: Logged-in user's ID
- `CurrentUserName`: User's display name
- `CurrentUserEmail`: User's email
- `CurrentUserRole`: User role (Student/Admin)
- `IsLoggedIn`: Boolean property to check login status

**Key Methods**:
- `SetUser()`: Store user information after successful login
- `Logout()`: Clear session data

**Registration**: Added to `Program.cs` as a scoped service

---

### 2. Pages Flow

#### Page A: Login.razor (`/login`)
**Location**: `Online Examination\Components\Pages\Login.razor`

**Features**:
- Two-way data binding for email and password fields
- Asynchronous login validation via `StudentService.LoginAsync()`
- Session management using `UserSession.SetUser()`
- Role-based navigation (Admin ¡ú Dashboard, Student ¡ú Exams)
- Error message display for failed login attempts
- Loading state during authentication

**Data Flow**:
```
User Input ¡ú StudentService.LoginAsync() ¡ú UserSession.SetUser() ¡ú Navigation
```

---

#### Page B: ExamPage.razor (`/exams`)
**Location**: `Online Examination\Components\Pages\ExamPage.razor`

**Features**:
- Authentication guard (redirects to login if not authenticated)
- Fetches all available exams using `StudentService.GetAllExamsAsync()`
- Bootstrap card-based UI for exam display
- Shows exam details: Title, Description, Time Limit, Question Count
- "Start Exam" button navigates to `/take-exam/{examId}`
- Loading spinner during data fetch
- Empty state message when no exams available

**Data Flow**:
```
OnInitializedAsync() ¡ú Check UserSession ¡ú Fetch Exams ¡ú Display Cards
```

---

#### Page C: TakeExam.razor (`/take-exam/{examId}`)
**Location**: `Online Examination\Components\Pages\TakeExam.razor`

**Features**:
- Route parameter binding for ExamId
- Loads exam with questions using `StudentService.GetExamByIdAsync()`
- **Critical Implementation**: Unique radio button groups per question
  - Uses `name="question_{questionId}"` to prevent conflicts
  - Four options (A, B, C, D) per question
- Dictionary-based answer storage: `Dictionary<int, string>`
  - Key: QuestionId
  - Value: Selected Option
- Auto-grading on submission via `StudentService.SubmitExamAsync()`
- Navigation to result page with attempt ID

**Data Flow**:
```
Load Exam ¡ú Display Questions ¡ú Collect Answers ¡ú Submit ¡ú Auto-grade ¡ú Navigate to Result
```

**Key Code Pattern**:
```csharp
// Radio button with unique name attribute
<input type="radio" 
       name="question_@question.Id" 
       value="A"
       @onchange="@(() => RecordAnswer(question.Id, "A"))">
```

---

#### Page D: ExamResult.razor (`/result/{attemptId}`)
**Location**: `Online Examination\Components\Pages\ExamResult.razor`

**Features**:
- Route parameter binding for AttemptId
- Fetches attempt details from `StudentService.GetStudentHistoryAsync()`
- **Score Display**:
  - Raw score (e.g., 8 out of 10)
  - Percentage calculation
  - Performance-based feedback messages
  - Color-coded alerts (Success/Info/Warning)
- Timestamp of exam completion
- Navigation buttons:
  - Back to Exams list
  - View full exam history

**Performance Feedback Logic**:
- 90%+: Excellent (Green)
- 75-89%: Great job (Green)
- 60-74%: Good work (Blue)
- 50-59%: Passed (Blue)
- Below 50%: Review needed (Yellow)

**Data Flow**:
```
Load Attempt ¡ú Calculate Stats ¡ú Display Result ¡ú Provide Actions
```

---

#### Bonus: ExamHistory.razor (`/exam-history`)
**Location**: `Online Examination\Components\Pages\ExamHistory.razor`

**Features**:
- Lists all previous exam attempts for the logged-in user
- Table view with: Exam Title, Score, Percentage, Date
- Color-coded badges for performance levels
- "View Result" button for each attempt
- Responsive table design

---

## CSS Files

### 1. take-exam.css
**Location**: `Online Examination\wwwroot\css\take-exam.css`
- Gradient header styling
- Card hover effects
- Radio button styling
- Responsive design

### 2. exam-results.css (Updated)
**Location**: `Online Examination\wwwroot\css\exam-results.css`
- Modern card design
- Animated success icon
- Gradient score display
- Button hover effects

### 3. exams-page.css (Updated)
**Location**: `Online Examination\wwwroot\css\exams-page.css`
- Bootstrap card integration
- Hover animations
- Loading states
- Alert styling

---

## Data Flow Summary

### Complete User Journey:
```
1. User enters email/password ¡ú Login.razor
   ¡ý
2. Authenticate via StudentService ¡ú Store in UserSession
   ¡ý
3. Navigate to /exams ¡ú ExamPage.razor
   ¡ý
4. Display available exams ¡ú User clicks "Start Exam"
   ¡ý
5. Navigate to /take-exam/{id} ¡ú TakeExam.razor
   ¡ý
6. Display questions ¡ú User selects answers
   ¡ý
7. Submit ¡ú Auto-grade via StudentService
   ¡ý
8. Create Attempt record ¡ú Get AttemptId
   ¡ý
9. Navigate to /result/{attemptId} ¡ú ExamResult.razor
   ¡ý
10. Display score and feedback
```

---

## Key Technical Decisions

### 1. Radio Button Implementation
**Problem**: Multiple questions with 4 options each could cause radio button conflicts.

**Solution**: Used unique `name` attribute per question:
```html
name="question_@question.Id"
```

This ensures each question has independent radio button groups.

### 2. Answer Storage
**Problem**: Need to track user answers for multiple questions.

**Solution**: Dictionary<int, string>
- Key: QuestionId (unique identifier)
- Value: Selected option ("A", "B", "C", or "D")

### 3. State Management
**Problem**: Need to maintain user session across multiple pages.

**Solution**: Scoped service `UserSession`
- Lives for the duration of the user's connection
- Shared across all components in the same circuit
- Automatically managed by Blazor Server

### 4. Auto-grading Logic
**Location**: `StudentService.SubmitExamAsync()`

**Process**:
1. Receives Dictionary of student answers
2. Loads exam with correct answers
3. Compares each answer (case-insensitive)
4. Calculates final score
5. Creates Attempt record
6. Returns Attempt with score

---

## Bootstrap Classes Used

### Cards
- `card`, `card-body`, `card-title`, `card-text`
- `shadow-sm`, `shadow-lg` (elevation)

### Buttons
- `btn`, `btn-primary`, `btn-success`, `btn-outline-secondary`
- `btn-lg`, `btn-sm` (sizing)

### Layout
- `container`, `row`, `col-md-4`
- `d-flex`, `justify-content-center`, `gap-3`

### Alerts
- `alert`, `alert-warning`, `alert-info`, `alert-danger`, `alert-success`

### Icons (Bootstrap Icons)
- `bi-check-circle-fill`, `bi-clock`, `bi-question-circle`
- `bi-arrow-left`, `bi-clock-history`, `bi-eye`

---

## Testing Checklist

1. **Login Flow**
   - [ ] Valid credentials ¡ú Navigate to /exams
   - [ ] Invalid credentials ¡ú Show error message
   - [ ] Empty fields ¡ú Show validation

2. **Exam List**
   - [ ] Display all exams from database
   - [ ] Show exam details (title, time, questions)
   - [ ] "Start Exam" button works

3. **Take Exam**
   - [ ] Load questions correctly
   - [ ] Radio buttons work independently per question
   - [ ] Can select all answers
   - [ ] Submit button triggers grading

4. **Results**
   - [ ] Correct score displayed
   - [ ] Percentage calculated properly
   - [ ] Feedback message matches score
   - [ ] Navigation buttons work

5. **Session Management**
   - [ ] Session persists across page navigation
   - [ ] Redirect to login when not authenticated

---

## Database Requirements

Ensure these tables exist with proper relationships:
- **Users**: Id, Username, Email, Password, Role
- **Exams**: Id, Title, Description, TimeLimitMinutes
- **Questions**: Id, ExamId, Text, OptionA/B/C/D, CorrectAnswer
- **Attempts**: Id, UserId, ExamId, Score, DateCreated

---

## Next Steps

1. **Run the application**: `dotnet run`
2. **Navigate to**: `https://localhost:xxxx/login`
3. **Test the complete flow** with sample data
4. **Optional enhancements**:
   - Add timer functionality during exam
   - Implement answer review after submission
   - Add exam categories/filters
   - Export results to PDF

---

## Notes

- All pages use `@rendermode InteractiveServer` for real-time updates
- Services injected using `@inject` directive
- Navigation handled by `NavigationManager`
- Error handling with try-catch blocks
- Responsive design for mobile devices
