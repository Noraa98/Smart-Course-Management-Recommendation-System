QUICK CHECKLIST - SMARTCOURSES ADMIN SETUP
==========================================

BEFORE RUNNING APP:
? SQL Server is running
? Connection string in appsettings.json is correct
? Project built successfully (Ctrl+Shift+B)

RUNNING APP - DO THESE STEPS IN ORDER:

1. DELETE OLD DATABASE (if exists)
   ? Open SQL Server Management Studio
   ? Right-click Databases ? New Database
   ? Or use SQL script in ADMIN_IMPLEMENTATION_COMPLETE.md

2. APPLY MIGRATIONS
   ? Package Manager Console ? Set Default Project: SmartCourses.DAL
   ? Run: Update-Database
   ? Or CLI: dotnet ef database update --project SmartCourses.DAL --startup-project SmartCourses.PL

3. START APPLICATION
   ? Click "Start" button (F5) in Visual Studio
   ? App opens to https://localhost:PORT/
   ? Check Console for: "Successfully seeded X courses"

4. LOGIN AS ADMIN
   ? Go to https://localhost:PORT/Account/Login
   ? Email: admin@smartcourses.com
   ? Password: Password@123
   ? You should be redirected to home page

5. GO TO ADMIN DASHBOARD
   ? Click profile dropdown (top right)
   ? Click "Admin Dashboard"
   ? OR: navigate to /Admin/Dashboard

6. CLICK MANAGE COURSES
   ? Button should be visible on dashboard
   ? Should show 10 courses in a table
   ? Buttons in "Actions" column should be clickable

7. TEST FUNCTIONALITY
   ? Details button ? Opens course with sections/lessons
   ? Edit button ? Opens form with pre-filled data
   ? Delete button ? Deletes course with confirmation
   ? Create button (top left) ? Opens new course form

IF NOTHING SHOWS UP:
? Database might be empty
? Check console output for seeding errors
? Run SQL query: SELECT COUNT(*) FROM Courses;
? If 0, re-run migrations

IF BUTTONS DON'T CLICK:
? Routing issue
? Verify [Area("Admin")] on CoursesController
? Verify asp-area="Admin" on all buttons
? Check Program.cs has area route before default route

IF PAGE SHOWS "NO COURSES FOUND":
? Courses weren't seeded
? Check categories exist: SELECT COUNT(*) FROM Categories;
? Check instructors exist: SELECT * FROM AspNetUsers WHERE Email LIKE '%instructor%';
? Check courses exist: SELECT * FROM Courses;

SUCCESS INDICATORS:
? Can see 10 courses in the list
? Can click buttons without JavaScript errors
? Can navigate to Details/Edit/Create pages
? Forms submit without errors
? Can see "Course created successfully" message
? Changes appear in database

BUILD STATUS: ? SUCCESS
All required files are in place and configured correctly.
Ready to deploy and test!
